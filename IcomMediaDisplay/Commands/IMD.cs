using RemoteAdmin;
using CommandSystem;
using System;
using IcomMediaDisplay.Logic;
using IcomMediaDisplay.Helpers;
using Exiled.API.Features;
using MEC;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net;
using Exiled.Permissions.Extensions;

namespace IcomMediaDisplay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class IMD : ICommand
    {
        public string Command => "icommediadisplay";
        public string[] Aliases => new[] { "imd" };
        public string Description => "Play a Media on Intercom.";

        //private PlaybackHandler playbackHandler;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("imd.command"))
            {
                response = "You do not have permission to use this command!";
                return false;
            }

            PlaybackHandler playbackHandler = new PlaybackHandler();
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
                        playbackHandler.PlayFrames(IcomMediaDisplay.tempdir + "/" + arguments.At(1));
                        response = "Playback started (Keep an eye on Server console, if debug enabled).";
                        return false;
                    }
                    catch (Exception ex)
                    {
                        response = "Failed to start playback. Error: " + ex.Message;
                        return false;
                    }
                case "break":
                    playbackHandler.BreakFromPlayback();
                    response = "Stopped playback.";
                    return false;
                case "pause":
                    if (!playbackHandler.isPaused)
                    {
                        playbackHandler.PausePlayback();
                        response = "Paused playback.";
                    }
                    else
                    {
                        playbackHandler.ResumePlayback();
                        response = "Resumed playback.";
                    }
                    return false;
                case "help":
                    /*

--- Subcommands ---
imd play <folderID> - Plays frames from a directory/container.
imd pause - Pause Playback.
imd stop - Abort Playback.
imd help - This.

                    */
                    response = "--- Subcommands ---\r\nimd play <folderID> - Plays frames from a directory/container.\r\nimd pause - Pause Playback.\r\nimd stop - Abort Playback.\r\nimd help - This.";
                    return false;
                default:
                    response = "Unknown subcommand. Use 'imd help' for syntax.";
                    return false;
            }
        }
    }
}
