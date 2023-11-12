using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Exiled.API.Features;

namespace IcomMediaDisplay.Helpers
{
    public class Converters
    {
        public static string RgbToHex(Color color)
        {
            return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public void ExportFrames(string videopath, string id)
        {
            string ffmpegPath = IcomMediaDisplay.FfmpegFolder + "/ffmpeg";
            string path = IcomMediaDisplay.tempdir + "/" + id;
            string arguments = $"-i \"{videopath}\" -r {IcomMediaDisplay.instance.Config.PlaybackFps} \"{path}\\%d.png\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Log.Error($"FFmpeg Error: {e.Data}");
                    }
                };

                process.Start();
                process.BeginErrorReadLine();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Log.Error("FFmpeg execution failed.");
                }
            }
        }
    }
}
