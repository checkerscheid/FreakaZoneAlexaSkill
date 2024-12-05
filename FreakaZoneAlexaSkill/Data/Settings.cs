//###################################################################################
//#                                                                                 #
//#                (C) FreakaZone GmbH                                              #
//#                =======================                                          #
//#                                                                                 #
//###################################################################################
//#                                                                                 #
//# Author       : Christian Scheid                                                 #
//# Date         : 05.12.2024                                                       #
//#                                                                                 #
//# Revision     : $Rev:: 145                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: Settings.cs 145 2024-12-05 19:12:44Z                     $ #
//#                                                                                 #
//###################################################################################
using System.Text;

namespace FreakaZoneAlexaSkill.Data {
	public class Settings {
		public string AppName { get; set; }
		public string IpAddr { get; set; }
		public string MacAddr { get; set; }
		public int Port { get; set; }
		public string Subnet { get; set; }
		public string? Token { get; set; }
		public Settings(string appName, string ipAddr, string subnet, string macAddr, int port, string? token) {
			byte[] bytes = Encoding.UTF8.GetBytes(appName);
			AppName = Convert.ToBase64String(bytes);
			IpAddr = ipAddr;
			MacAddr = macAddr.Replace("-", "");
			Port = port;
			Subnet = subnet;
			if(token != null && token.Equals(string.Empty)) {
				token = null;
			}

			Token = token;
		}
		public override string ToString() {
			return $"AppName: {AppName}\r\nIpAddr: {IpAddr}\r\nMacAddr: {MacAddr}\r\nPort: {Port}\r\nSubnet: {Subnet}\r\nToken: {Token}";
		}
	}
}
