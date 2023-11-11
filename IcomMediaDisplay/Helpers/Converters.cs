using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace IcomMediaDisplay.Helpers
{
    public class Converters
    {
        public static string RgbToHex(Color color)
        {
            return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }
}
