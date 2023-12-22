using System;
using System.IO;
using Exiled.API.Features;

namespace IcomMediaDisplay
{
    public class IcomMediaDisplay : Plugin<Config>
    {
        public static IcomMediaDisplay instance { get; private set; }
        private EventHandler ev { get; set; }

        public static string tempdir => Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED"), "Plugins"), "IcomMediaDisplay");

        // Plugin information
        public override string Name => "IcomMediaDisplay";
        public override string Prefix { get; } = "IcomMediaDisplay";
        public override string Author { get; } = "Mitsukia";
        public override Version Version { get; } = new Version(2, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(8, 3, 9);

        public override void OnEnabled()
        {
            instance = this;
            if (!Directory.Exists(tempdir))
            {
                Directory.CreateDirectory(tempdir);
            }
            Log.Info("\n_____________  __________ \r\n____  _/__   |/  /__  __ \\\r\n __  / __  /|_/ /__  / / /\r\n__/ /  _  /  / / _  /_/ / \r\n/___/  /_/  /_/  /_____/  ");
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            instance = null;
            base.OnDisabled();
        }
    }
}
