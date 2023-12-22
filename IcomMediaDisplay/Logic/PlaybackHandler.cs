using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Exiled.API.Features;
using IcomMediaDisplay.Helpers;
using MEC;
using Time = UnityEngine;

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
        //private const float WarningThreshold = 0.95f;

        public void PlayFrames(string folderPath)
        {
            Log.Debug("Called PlayFrames");
            string[] unsortedFrames = Directory.GetFiles(folderPath, "*.png");

            frames = unsortedFrames
                .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)))
                .ToArray();

            if (frames.Length == 0)
            {
                string msg = "No frames found in the specified folder.";
                Log.Error(msg);
                throw new Exception(msg);
            }

            targetFrameDurationInSeconds = 1.0f / VideoFps; // Set the target frame duration
            Timing.RunCoroutine(PlayFramesCoroutine()); // Start coroutine to play frames
        }

        private IEnumerator<float> PlayFramesCoroutine()
        {
            float frameDuration = 1.0f / VideoFps;
            var startTime = Time.Time.time;
            var nextFrameTime = startTime + frameDuration;

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

                string framePath = frames[currentFrameIndex];
                LoadAndDisplayFrame(framePath);

                float currentTime = Time.Time.time;

                if (currentTime < nextFrameTime)
                {
                    yield return Timing.WaitForSeconds(nextFrameTime - currentTime);
                }

                nextFrameTime += frameDuration;
                currentFrameIndex++; // Move to the next frame
            }
        }

        private void LoadAndDisplayFrame(string framePath)
        {
            try
            {
                using (FileStream stream = new FileStream(framePath, FileMode.Open, FileAccess.Read))
                using (Bitmap frame = new Bitmap(stream))
                {
                    Bitmap frameToProcess = frame;

                    if (IcomMediaDisplay.instance.Config.QuantizeBitmap)
                    {
                        Compressors compressors = new Compressors();
                        frameToProcess = compressors.QuantizeBitmap(frameToProcess);
                    }

                    string tmpRepresentation = ConvertToTMPCode(frameToProcess);
                    Intercom.IntercomDisplay.Network_overrideText = tmpRepresentation;
                    Log.Debug($"Frame {currentFrameIndex} displayed. Code length: {tmpRepresentation.Length}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error loading frame: {framePath}. Details: {ex}");
            }
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
