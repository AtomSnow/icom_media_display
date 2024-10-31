using CommandSystem;
using IcomMediaDisplay.Logic;
using Exiled.Permissions.Extensions;

namespace IcomMediaDisplay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class IMD : ICommand
    {
        public string Command => "icommediadisplay";
        public string[] Aliases => ["imd"];
        public string Description => "Play a Media on Intercom.";

        PlaybackHandler playbackHandler;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("imd.command"))
            {
                response = "You do not have permission to use this command!";
                return false;
            }
            playbackHandler = IcomMediaDisplay.GetPHInstance();
            switch (arguments.At(0))
            {
                case "play":
                    if (arguments.Count < 2)
                    {
                        response = "Not enough arguments for 'play' command.";
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(arguments.At(1)))
                    {
                        response = "Invalid path for playback.";
                        return false;
                    }
                    try
                    {
                        playbackHandler.PlayFrames(IcomMediaDisplay.PluginDirectory + "/" + arguments.At(1));
                        response = "Playback started (Keep an eye on Server console, if debug enabled).";
                        return true;
                    }
                    catch (Exception ex)
                    {
                        response = "Failed to start playback. Error: " + $"{ex.Message}\n{ex.StackTrace}";
                        return false;
                    }
                case "break":
                    playbackHandler.BreakFromPlayback();
                    response = "Stopped playback.";
                    return true;
                case "pause":
                    if (!playbackHandler.IsPaused)
                    {
                        playbackHandler.PausePlayback();
                        response = "Paused playback.";
                    }
                    else
                    {
                        playbackHandler.ResumePlayback();
                        response = "Resumed playback.";
                    }
                    return true;
                case "mod":
                    string field = arguments.At(1).ToLower();
                    string value = arguments.At(2).ToLower();
                    if (field == "fps")
                    {
                        try
                        {
                            playbackHandler.VideoFps = ushort.Parse(value);
                        }
                        catch (Exception ex)
                        {
                            response = $"Failed to modify field: " + ex.Message;
                            return false;
                        }
                    }
                    response = $"Field {field} has been modified to {value}";
                    return true;

                case "help":
                    /*

--- Subcommands ---
imd play <folderID> - Plays frames from a directory/container.
imd pause - Pause Playback.
imd stop - Abort Playback.
imd help - This.
imd mod <value> - Modify value of some config fields.

                    */
                    response = "--- Subcommands ---\r\nimd play <folderID> - Plays frames from a directory/container.\r\nimd pause - Pause Playback.\r\nimd stop - Abort Playback.\r\nimd help - This.\r\nimd mod <value> - Modify value of some config fields.";
                    return true;
                default:
                    response = "Unknown subcommand. Use 'imd help' for syntax.";
                    return false;
            }
        }
    }
}
