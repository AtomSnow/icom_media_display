using System.Drawing;

namespace IcomMediaDisplay.Helpers
{
    public static class Converters
    {
        public static string RgbToHex(Color color)
        {
            return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }
}
