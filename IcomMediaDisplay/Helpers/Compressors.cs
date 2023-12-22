using System.Drawing;

namespace IcomMediaDisplay.Helpers
{
    public class Compressors
    {
        public enum BitDepth
        {
            Bits1 = 1,
            Bits4 = 4,
            Bits8 = 8,
            Bits16 = 16
        }

        public static string CompressTMP(string unccode)
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

            string compressedCode = unccode;
            foreach (var replacement in replacements)
            {
                string[] parts = replacement.Split(':');
                compressedCode = compressedCode.Replace(parts[0], parts[1]);
            }

            return compressedCode;
        }

        public Bitmap QuantizeBitmap(Bitmap frame)
        {
            int height = frame.Height;
            int width = frame.Width;

            Bitmap quantizedFrame = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = frame.GetPixel(x, y);

                    // Quantize colors by rounding their components to reduce the number of distinct colors
                    Color quantizedColor = Color.FromArgb(
                        (pixelColor.R / IcomMediaDisplay.instance.Config.DivisorR) * IcomMediaDisplay.instance.Config.DivisorR,
                        (pixelColor.G / IcomMediaDisplay.instance.Config.DivisorG) * IcomMediaDisplay.instance.Config.DivisorG,
                        (pixelColor.B / IcomMediaDisplay.instance.Config.DivisorB) * IcomMediaDisplay.instance.Config.DivisorB
                    );

                    quantizedFrame.SetPixel(x, y, quantizedColor);
                }
            }
            return quantizedFrame;
        }
    }
}
