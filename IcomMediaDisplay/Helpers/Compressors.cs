using IcomMediaDisplay.Enums;
using System.Drawing;

namespace IcomMediaDisplay.Helpers
{
    public static class Compressors
    {
        public enum BitDepth
        {
            Bits1 = 1,
            Bits4 = 4,
            Bits8 = 8,
            Bits16 = 16
        }

        public static string CompressTMP(string code)
        {
            string[] replacements = {
                "#ffffff:#fff",
                "#000000:#000",
                "#ff0000:red",
                "#ffff00:yellow",
                "#00ff00:green",
                "#00ffff:cyan",
                "#0000ff:blue"
            };

            foreach (var replacement in replacements)
            {
                string[] parts = replacement.Split(':');
                code = code.Replace(parts[0], parts[1]);
            }
            return code;
        }

        public static Bitmap QuantizeBitmap(Bitmap frame)
        {
            int height = frame.Height;
            int width = frame.Width;
            Bitmap quantizedFrame = new(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = frame.GetPixel(x, y);
                    Color quantizedColor = Color.FromArgb( // Quantize colors by rounding their components to reduce the number of distinct colors
                        (pixelColor.R / IcomMediaDisplay.Instance.Config.DivisorR) * IcomMediaDisplay.Instance.Config.DivisorR,
                        (pixelColor.G / IcomMediaDisplay.Instance.Config.DivisorG) * IcomMediaDisplay.Instance.Config.DivisorG,
                        (pixelColor.B / IcomMediaDisplay.Instance.Config.DivisorB) * IcomMediaDisplay.Instance.Config.DivisorB
                    );
                    quantizedFrame.SetPixel(x, y, quantizedColor);
                }
            }
            return quantizedFrame;
        }

        public static async Task<Bitmap> DownscaleAsync(Bitmap original)
        {
            return await Task.Run(() => Downscale(original));
        }

        public static Bitmap Downscale(Bitmap original)
        {
            double scaleFactor = IcomMediaDisplay.Instance.Config.ScalingFactor;

            int newWidth = (int)(original.Width * scaleFactor);
            int newHeight = (int)(original.Height * scaleFactor);

            Bitmap resizedImage = new(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(resizedImage))
            {
                switch (IcomMediaDisplay.Instance.Config.Resampling)
                {
                    case InterpolationMode.NearestNeighbor:
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                        break;
                    case InterpolationMode.HighQualityBicubic:
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        break;
                    default:
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        break;
                }
                g.DrawImage(original, 0, 0, newWidth, newHeight);
            }
            return resizedImage;
        }

    }
}
