using System;
using System.IO;
using Exiled.API.Features;
//using ServerHandler = Exiled.Events.Handlers.Server;

namespace IcomMediaDisplay
{
    public class IcomMediaDisplay : Plugin<Config>
    {
        public static IcomMediaDisplay instance;
        private EventHandler ev { get; set; }

        public static string tempdir { get; set; } = Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED"), "Plugins"), "IcomMediaDisplay");

        // Plugin information
        public override string Name => "IcomMediaDisplay";
        public override string Prefix { get; } = "IcomMediaDisplay";
        public override string Author { get; } = "AtomSnow";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(8, 3, 10); // Intercom.DisplayText fix

        public override void OnEnabled()
        {
            instance = this;
            ev = new EventHandler();
            if (!Directory.Exists(tempdir)) Directory.CreateDirectory(tempdir);
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            instance = null;
            ev = null;

            base.OnDisabled();
        }
    }
}
