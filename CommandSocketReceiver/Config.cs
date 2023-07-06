using Exiled.API.Interfaces;
using System.ComponentModel;

namespace CommandSocketReceiver
{
	public sealed class Config : IConfig
	{
		[Description("Whether or not the plugin is enabled.")]
		public bool IsEnabled { get; set; } = true;

		public bool Debug { get; set; } = false;

		public string IP { get; set; } = "0.0.0.0";
		public int Port { get; set; } = 6969;
	}
}
