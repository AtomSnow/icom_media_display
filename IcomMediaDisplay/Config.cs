using Exiled.API.Interfaces;
using IcomMediaDisplay.Enums;
using System.ComponentModel;

namespace IcomMediaDisplay
{
    public class Config : IConfig
    {
        [Description("Whether or not the plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not the debug info should be printed out.")]
        public bool Debug { get; set; } = false;

        [Description("A pixel character to display.")]
        public char Pixel { get; set; } = '█';

        [Description("Icomtxt code Prefix and Suffix.")]
        public string Prefix { get; set; } = "<line-height=89%>";
        public string Suffix { get; set; } = "";

        [Description("Default FPS of playback.")]
        public ushort PlaybackFps { get; set; } = 20;

        [Description("Quantizes Bitmap before converting to code, might decrease code size, increases conversion time.")]
        public bool QuantizeBitmap { get; set; } = false;
        public int DivisorR { get; set; } = 128;
        public int DivisorG { get; set; } = 128;
        public int DivisorB { get; set; } = 128;

        [Description("Smart downscaler settings, increases conversion time by a lot. [EXPERTS ONLY]")]
        public bool UseSmartDownscaler { get; set; } = true;
        public int Deadzone { get; set; } = 50000;
        public double ScalingFactor { get; set; } = 0.75;
        public InterpolationMode Resampling { get; set; } = InterpolationMode.HighQualityBicubic;
    }
}
