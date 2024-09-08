namespace IcomMediaDisplay.API
{
    public static class IcomMediaDisplayAPI
    {
        public static void Play(string containerName)
        {
            IcomMediaDisplay.GetPHInstance().PlayFrames(IcomMediaDisplay.PluginDirectory + "/" + containerName);
        }
        public static void Break()
        {
            IcomMediaDisplay.GetPHInstance().BreakFromPlayback();
        }
        public static void Pause()
        {
            IcomMediaDisplay.GetPHInstance().PausePlayback();
        }
    }
}
