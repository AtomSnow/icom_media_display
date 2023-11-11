using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcomMediaDisplay.Helpers
{
    public class Compressors
    {
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
    }
}
