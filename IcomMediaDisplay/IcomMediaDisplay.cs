using Exiled.API.Features;
using IcomMediaDisplay.Logic;

namespace IcomMediaDisplay
{
    public class IcomMediaDisplay : Plugin<Config>
    {
        public static IcomMediaDisplay Instance { get; private set; }
        public static string PluginDirectory => Path.Combine(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED"), "Plugins"), "IcomMediaDisplay");
        private static PlaybackHandler _playbackHandler;

        // Plugin information
        public override string Name => "IcomMediaDisplay";
        public override string Prefix { get; } = "IcomMediaDisplay";
        public override string Author { get; } = "Skadi";
        public override Version Version { get; } = new Version(2, 0, 1);
        public override Version RequiredExiledVersion { get; } = new Version(8, 11, 0);

        public override void OnEnabled()
        {
            Instance = this;
            if (!Directory.Exists(PluginDirectory))
            {
                Directory.CreateDirectory(PluginDirectory);
            }
            //Log.Info("\n_____________  __________ \r\n____  _/__   |/  /__  __ \\\r\n __  / __  /|_/ /__  / / /\r\n__/ /  _  /  / / _  /_/ / \r\n/___/  /_/  /_/  /_____/  ");
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            _playbackHandler = null;
            base.OnDisabled();
        }

        public static PlaybackHandler GetPHInstance()
        {
            _playbackHandler ??= new PlaybackHandler();
            return _playbackHandler;
        }
    }
}
