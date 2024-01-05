using Exiled.API.Interfaces;
using System.ComponentModel;

namespace IcomMediaDisplay
{
    public class Config : IConfig
    {
        [Description("Whether or not the plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not the debug info should be printed out.")]
        public bool Debug { get; set; } = false;

        [Description("An pixel character to display.")]
        public string Pixel { get; set; } = "█";

        [Description("Icomtxt code Prefix and Suffix.")]
        public string Prefix { get; set; } = "<line-height=89%>";
        public string Suffix { get; set; } = "";

        [Description("Default FPS of playback.")]
        public int PlaybackFps { get; set; } = 20;

        [Description("Quantizes Bitmap before converting to code, might decrease code size, increases conversion time.")]
        public bool QuantizeBitmap { get; set; } = true;
        public int DivisorR { get; set; } = 128;
        public int DivisorG { get; set; } = 128;
        public int DivisorB { get; set; } = 128;

        [Description("Image loader buffer size. (Not implemented yet)")]
        public int BufferSize { get; set; } = 512;
    }
}
