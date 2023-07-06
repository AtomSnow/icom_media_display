using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Enums;

namespace CommandSocketReceiver.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class listen : ICommand
    {
        public string Command => "listen";

        public string[] Aliases => new[] { "lt" };

        public string Description => "Listen for commands indefinitely.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
        
        int port = CommandSocketReceiver.instance.Config.Port;
        TcpListener listener = null;

        while (true)
        {
        try
        {
            IPAddress localAddr = IPAddress.Parse(CommandSocketReceiver.instance.Config.IP);
            listener = new TcpListener(localAddr, port);

            listener.Start();

            TcpClient client = listener.AcceptTcpClient();

            byte[] buffer = new byte[1024];
            StringBuilder message = new StringBuilder();
            NetworkStream stream = client.GetStream();
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                message.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
            }

			string messagestring = message.ToString();
            // message
			Log.Info(messagestring);
			Server.RunCommand(messagestring);

            // cls
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Wystąpił błąd: " + ex.Message);
        }
        finally
        {
            // stop listening
            listener?.Stop();
			
		}
        }
        response = $"!";
        return true;
        }
    }
}