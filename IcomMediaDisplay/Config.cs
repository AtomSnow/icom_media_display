using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcomMediaDisplay
{
    public sealed class Config : IConfig
    {
        [Description("Whether or not the plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;
        [Description("Whether or not the debug info should be printed out.")]
        public bool Debug { get; set; } = false;
        [Description("Whether or not should be TMP Code be compressed. [Disable if frames are not synced correctly.]")]
        public bool Compression { get; set; } = true;
        [Description("Whether or not the exported frames should be saved to jpg.")]
        public bool Jpeg { get; set; } = false;
        [Description("[UNUSED] JPG Quality.")]
        public int JpegQuality { get; set; } = 75;
        [Description("[UNUSED] PNG Compression (Lossless).")]
        public int PngCompression { get; set; } = 8;
        [Description("An pixel character to display.")]
        public string Pixel { get; set; } = "█";
        [Description("Line height.")]
        public string Height { get; set; } = "<line-height=90%>";
    }
}
