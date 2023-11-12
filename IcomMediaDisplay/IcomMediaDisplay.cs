using System;
using System.IO;
using Exiled.API.Features;

namespace IcomMediaDisplay
{
    public class IcomMediaDisplay : Plugin<Config>
    {
        public static IcomMediaDisplay instance;
        private EventHandler ev { get; set; }

        public static string tempdir => Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED"), "Plugins"), "IcomMediaDisplay");
        public static string FfmpegFolder => Path.Combine(tempdir, "put_ffmpeg_here");
        public static string GetFolder => Path.Combine(tempdir, "get");

        // Plugin information
        public override string Name => "IcomMediaDisplay";
        public override string Prefix { get; } = "IcomMediaDisplay";
        public override string Author { get; } = "AtomSnow";
        public override Version Version { get; } = new Version(1, 1, 0);
        public override Version RequiredExiledVersion { get; } = new Version(8, 3, 9);

        public override void OnEnabled()
        {
            instance = this;
            if (!Directory.Exists(tempdir))
            {
                Directory.CreateDirectory(tempdir);
                Directory.CreateDirectory(FfmpegFolder);
                Directory.CreateDirectory(GetFolder);
            }
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            instance = null;
            base.OnDisabled();
        }
    }
}
