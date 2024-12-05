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
//# File-ID      : $Id:: WoL.cs 145 2024-12-05 19:12:44Z                          $ #
//#                                                                                 #
//###################################################################################
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Net.Mail;

namespace FreakaZoneAlexaSkill.Src {
	public static class WoL {
		public static async Task WakeOnLan(string macAddress) {
			byte[] magicPacket = BuildMagicPacket(macAddress);
			foreach(NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces().Where((n) =>
				n.NetworkInterfaceType != NetworkInterfaceType.Loopback && n.OperationalStatus == OperationalStatus.Up)) {
				IPInterfaceProperties iPInterfaceProperties = networkInterface.GetIPProperties();
				foreach(MulticastIPAddressInformation multicastIPAddressInformation in iPInterfaceProperties.MulticastAddresses) {
					IPAddress multicastIpAddress = multicastIPAddressInformation.Address;
					if(multicastIpAddress.ToString().StartsWith("ff02::1%", StringComparison.OrdinalIgnoreCase)) {
						UnicastIPAddressInformation? unicastIPAddressInformation = iPInterfaceProperties.UnicastAddresses.Where((u) =>
							u.Address.AddressFamily == AddressFamily.InterNetworkV6 && !u.Address.IsIPv6LinkLocal).FirstOrDefault();
						if(unicastIPAddressInformation != null) {
							await SendWakeOnLan(unicastIPAddressInformation.Address, multicastIpAddress, magicPacket);
						}
					} else if(multicastIpAddress.ToString().Equals("224.0.0.1")) {
					} else if(multicastIpAddress.ToString().Equals("224.0.0.1")){
						UnicastIPAddressInformation? unicastIPAddressInformation = iPInterfaceProperties.UnicastAddresses.Where((u) =>
							u.Address.AddressFamily == AddressFamily.InterNetwork && !iPInterfaceProperties.GetIPv4Properties().IsAutomaticPrivateAddressingActive).FirstOrDefault();
						if(unicastIPAddressInformation != null) {
							await SendWakeOnLan(unicastIPAddressInformation.Address, multicastIpAddress, magicPacket);
						}
					}
				}
			}
		}
		private static byte[] BuildMagicPacket(string macAddress) {
			Logger.Write(MethodBase.GetCurrentMethod(), $"Build Magic Packet '{macAddress}'");
			macAddress = Regex.Replace(macAddress, "[: -]", "");
			byte[] macBytes = Convert.FromHexString(macAddress);

			IEnumerable<byte> header = Enumerable.Repeat((byte)0xff, 6); //First 6 times 0xff
			IEnumerable<byte> data = Enumerable.Repeat(macBytes, 16).SelectMany(m => m); // then 16 times MacAddress
			return header.Concat(data).ToArray();
		}
		private static async Task SendWakeOnLan(IPAddress localIpAddress, IPAddress multicastIpAddress, byte[] magicPacket) {
			Logger.Write(MethodBase.GetCurrentMethod(), $"Send WoL");
			using UdpClient client = new(new IPEndPoint(localIpAddress, 0));
			await client.SendAsync(magicPacket, magicPacket.Length, new IPEndPoint(multicastIpAddress, 9));
		}
	}
}
