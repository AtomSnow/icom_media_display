using System.Collections.Concurrent;
using System.Drawing;
using System.Text;
using Exiled.API.Features;
using IcomMediaDisplay.Helpers;
using MEC;
using UnityEngine;
using Color = System.Drawing.Color;

namespace IcomMediaDisplay.Logic
{
    public class PlaybackHandler
    {
        private ulong currentFrameIndex;
        private string[] frames;
        private bool breakPlayback;
        private readonly ConcurrentQueue<string> frameQueue;
        // Public
        public ulong FrameCount { get; private set; }
        public ushort VideoFps { get; set; }
        public bool IsPaused { get; private set; }

        public PlaybackHandler()
        {
            breakPlayback = false;
            frameQueue = new();
            currentFrameIndex = 0;
            IsPaused = false;
            VideoFps = IcomMediaDisplay.Instance.Config.PlaybackFps;
        }

        public void PlayFrames(string folderPath)
        {
            Log.Debug("Called PlayFrames");
            string[] unsortedFrames = Directory.GetFiles(folderPath, "*.png");

            frames = [.. unsortedFrames.OrderBy(f => long.Parse(Path.GetFileNameWithoutExtension(f)))];

            FrameCount = (ulong)frames.Length;

            if (frames.Length == 0)
            {
                string msg = "No frames found in the specified folder.";
                Log.Error(msg);
                throw new Exception(msg);
            }

            Task.Run(async () => await ConvertAndEnqueueFrames(frames));
            Timing.RunCoroutine(PlayFramesCoroutine());
        }

        private IEnumerator<float> PlayFramesCoroutine()
        {
            float frameDuration = 1.0f / VideoFps;
            float startTime = Time.time;
            float nextFrameTime = startTime + frameDuration;

            while (currentFrameIndex < (ulong)frames.Length)
            {
                if (breakPlayback)
                {
                    breakPlayback = false;
                    yield break; // Exit the coroutine
                }

                if (IsPaused)
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
                if (breakPlayback || IsPaused) break;

                try
                {
                    await ConvertFrameAsync(framePath); // Async convert (cooler than previous method)
                }
                catch (Exception ex)
                {
                    Log.Error($"Error loading frame: {framePath}. Details: {ex}");
                }
            }
        }

        private async Task ConvertFrameAsync(string framePath)
        {
            long codelen = 0;
            using (FileStream stream = new(framePath, FileMode.Open, FileAccess.Read))
            using (Bitmap frame = new(stream))
            {
                Bitmap frameToProcess = frame;

                if (IcomMediaDisplay.Instance.Config.QuantizeBitmap)
                {
                    frameToProcess = Compressors.QuantizeBitmap(frameToProcess);
                }

                string tmpRepresentation = await ConvertToTMPCodeAsync(frameToProcess);

                if (IcomMediaDisplay.Instance.Config.UseSmartDownscaler)
                {
                    long maxSize = IcomMediaDisplay.Instance.Config.Deadzone;

                    while (tmpRepresentation.Length > maxSize)
                    {
                        Task<Bitmap> compressorTask = Task.Run(() => Compressors.DownscaleAsync(frameToProcess));
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
                currentFrameIndex++;
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
            StringBuilder codeBuilder = new();
            StringBuilder colorBlock = new();
            Color previousColor = Color.Empty;
            
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

                    colorBlock.Append(IcomMediaDisplay.Instance.Config.Pixel);
                    previousColor = pixelColor;
                }
                codeBuilder.Append(GetColoredBlock(colorBlock.ToString(), previousColor)).Append("\n");
            }
            string codeStr = IcomMediaDisplay.Instance.Config.Prefix + codeBuilder.ToString() + IcomMediaDisplay.Instance.Config.Suffix;
            return Compressors.CompressTMP(codeStr);
        }

        private string GetColoredBlock(string content, Color color)
        {
            return $"<color=#{Converters.RgbToHex(color)}>{content}</color>";
        }

        public void BreakFromPlayback()
        {
            breakPlayback = true;
        }

        public void PausePlayback()
        {
            IsPaused = true;
        }

        public void ResumePlayback()
        {
            IsPaused = false;
        }
    }
}
