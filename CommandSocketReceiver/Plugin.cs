using System;
using Exiled.API.Features;
using Exiled.API.Enums;
using ServerHandler = Exiled.Events.Handlers.Server;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace CommandSocketReceiver
{
	public class CommandSocketReceiver : Plugin<Config>
	{
		public static CommandSocketReceiver instance;

		private EventHandler ev { get; set; }

		public override string Name => "CommandSocketReceiver";

		public override string Author { get; } = "AtomSnow";

        public override Version RequiredExiledVersion { get; } = new Version(3, 0, 0);

        public override string Prefix { get; } = "CommandSocketReceiver";

        public override Version Version { get; } = new Version(0, 1, 0);

		public override PluginPriority Priority { get; } = PluginPriority.First;

		public override void OnEnabled()
		{
			instance = this;
			ev = new EventHandler();
			ServerHandler.WaitingForPlayers += ev.OnWaitingForPlayers;
			
			base.OnEnabled();
		}

		public override void OnDisabled() 
		{
		    ServerHandler.WaitingForPlayers -= ev.OnWaitingForPlayers;
			
			ev = null;
			base.OnDisabled();
		}

	}
}
