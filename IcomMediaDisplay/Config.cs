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
        [Description("An pixel character to display.")]
        public string Pixel { get; set; } = "█";
        [Description("Line height modifier.")]
        public string Height { get; set; } = "<line-height=90%>";
        [Description("Whether or not the Network Message should be limited.")]
        public bool NetworkOverflowLimit { get; set; } = false;
        [Description("Set limit (in bytes), already set to Maximum limitation to Networking library.")]
        public int NetworkOverflowLimitMax { get; set; } = 65534;
        [Description("FPS of playback.")]
        public int PlaybackFps { get; set; } = 20;
    }
}
