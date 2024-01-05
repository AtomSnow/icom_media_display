using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using IcomMediaDisplay.Helpers;
using MEC;
using UnityEngine;
using Color = System.Drawing.Color;

namespace IcomMediaDisplay.Logic
{
    public class PlaybackHandler
    {
        private int currentFrameIndex = 0;
        private string[] frames;
        private float targetFrameDurationInSeconds;
        public float delayTime;
        public int VideoFps = IcomMediaDisplay.instance.Config.PlaybackFps;
        private bool breakPlayback = false;
        public bool isPaused = false;
        private ConcurrentQueue<string> frameQueue = new ConcurrentQueue<string>();
        public int FrameCount { get; private set; }

        public void PlayFrames(string folderPath)
        {
            Log.Debug("Called PlayFrames");
            string[] unsortedFrames = Directory.GetFiles(folderPath, "*.png");

            frames = unsortedFrames
                .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)))
                .ToArray();

            FrameCount = frames.Length;

            if (frames.Length == 0)
            {
                string msg = "No frames found in the specified folder.";
                Log.Error(msg);
                throw new Exception(msg);
            }

            targetFrameDurationInSeconds = 1.0f / VideoFps; // Set the target frame duration
            Task.Run(async () => await ConvertAndEnqueueFrames(frames));
            Timing.RunCoroutine(PlayFramesCoroutine());
        }

        private IEnumerator<float> PlayFramesCoroutine()
        {
            float frameDuration = 1.0f / VideoFps;
            float startTime = Time.time;
            float nextFrameTime = startTime + frameDuration;

            while (currentFrameIndex < frames.Length)
            {
                if (breakPlayback)
                {
                    breakPlayback = false;
                    yield break; // Exit the coroutine
                }

                if (isPaused)
                {
                    yield return 0;
                    continue; // Skip to the next iteration if paused
                }

                if (frameQueue.TryDequeue(out string tmpRepresentation))
                {
                    Intercom.IntercomDisplay.Network_overrideText = tmpRepresentation;
                    currentFrameIndex++; // Move to the next frame
                }

                float currentTime = Time.time;
                float timeRemaining = nextFrameTime - currentTime;

                if (timeRemaining > 0)
                {
                    yield return Timing.WaitForSeconds(timeRemaining);
                }
                nextFrameTime += frameDuration;
            }
        }


        private async Task ConvertAndEnqueueFrames(string[] frames)
        {
            foreach (var framePath in frames)
            {
                if (breakPlayback || isPaused)
                    break;

                try
                {
                    await ConvertFrameAsync(framePath); // Convert frame asynchronously
                }
                catch (Exception ex)
                {
                    Log.Error($"Error loading frame: {framePath}. Details: {ex}");
                }
            }
        }

        private async Task ConvertFrameAsync(string framePath)
        {
            try
            {
                Compressors compressors = new Compressors();
                int codelen = 0;

                using (FileStream stream = new FileStream(framePath, FileMode.Open, FileAccess.Read))
                using (Bitmap frame = new Bitmap(stream))
                {
                    Bitmap frameToProcess = frame;

                    if (IcomMediaDisplay.instance.Config.QuantizeBitmap)
                    {
                        frameToProcess = compressors.QuantizeBitmap(frameToProcess);
                    }

                    string tmpRepresentation = await ConvertToTMPCodeAsync(frameToProcess);

                    if (IcomMediaDisplay.instance.Config.UseSmartDownscaler)
                    {
                        int maxSize = IcomMediaDisplay.instance.Config.Deadzone;

                        while (tmpRepresentation.Length > maxSize)
                        {
                            Task<Bitmap> compressorTask = Task.Run(() => compressors.DownscaleAsync(frameToProcess));
                            frameToProcess = await compressorTask;

                            Task<string> conversionTask = Task.Run(() => ConvertToTMPCode(frameToProcess));
                            tmpRepresentation = await conversionTask;

                            if (tmpRepresentation.Length > maxSize)
                            {
                                Log.Debug($"Frame {currentFrameIndex}/{FrameCount} exceeds deadzone after downscaling, retrying until it fits. ({tmpRepresentation.Length} < {maxSize})");
                            }
                        }
                    }

                    frameQueue.Enqueue(tmpRepresentation);
                    codelen = tmpRepresentation.Length;

                    Log.Debug($"Frame {currentFrameIndex}/{FrameCount} converted and enqueued. Code length: {codelen}");
                    currentFrameIndex++; // Increment the frame index after successfully enqueuing
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error loading frame: {framePath}. Details: {ex}");
            }
        }

        public async Task<string> ConvertToTMPCodeAsync(Bitmap frame)
        {
            return await Task.Run(() => ConvertToTMPCode(frame));
        }

        public string ConvertToTMPCode(Bitmap frame)
        {
            int height = frame.Height;
            int width = frame.Width;
            StringBuilder codeBuilder = new StringBuilder();

            Color previousColor = Color.Empty; // Track the previous color
            StringBuilder colorBlock = new StringBuilder();

            for (int y = 0; y < height; y++)
            {
                colorBlock.Clear(); // Clear the color block for each new row
                previousColor = Color.Empty; // Reset previous color for new row

                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = frame.GetPixel(x, y);

                    if (pixelColor != previousColor)
                    {
                        if (colorBlock.Length > 0)
                        {
                            codeBuilder.Append(GetColoredBlock(colorBlock.ToString(), previousColor));
                            colorBlock.Clear();
                        }
                    }

                    colorBlock.Append(IcomMediaDisplay.instance.Config.Pixel);
                    previousColor = pixelColor;
                }
                codeBuilder.Append(GetColoredBlock(colorBlock.ToString(), previousColor)).Append("\n");
            }
            string codeStr = IcomMediaDisplay.instance.Config.Prefix + codeBuilder.ToString() + IcomMediaDisplay.instance.Config.Suffix;
            string done = Compressors.CompressTMP(codeStr);
            return done;
        }

        private string GetColoredBlock(string content, Color color)
        {
            string hexValue = "#" + Converters.RgbToHex(color);
            return $"<color={hexValue}>{content}</color>";
        }

        public void BreakFromPlayback()
        {
            breakPlayback = true;
        }

        public void PausePlayback()
        {
            isPaused = true;
        }

        public void ResumePlayback()
        {
            isPaused = false;
        }

    }
}