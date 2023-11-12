using RemoteAdmin;
using CommandSystem;
using System;
using IcomMediaDisplay.Logic;
using Exiled.API.Features;
using MEC;
using System.Collections.Generic;

namespace IcomMediaDisplay.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class IMD : ICommand
    {
        public string Command => "icommediadisplay";

        public string[] Aliases => new[] { "imd" };

        public string Description => "Play a Media on Intercom.";

        public int i2 { get; set; } = 0;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = "Provide video path.";
                return false;
            }
            response = "Okay";
            string path = arguments.At(0);
            PlaybackHandler playbackHandler = new PlaybackHandler();
            playbackHandler.PlayFrames(path);
            return false;
        }
    }
}
