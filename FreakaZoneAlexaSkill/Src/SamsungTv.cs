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
//# File-ID      : $Id:: SamsungTv.cs 145 2024-12-05 19:12:44Z                    $ #
//#                                                                                 #
//###################################################################################
using FreakaZoneAlexaSkill.Data;
using System.Reflection;

namespace FreakaZoneAlexaSkill.Src {
	class SamsungTv {
		private Settings settings;
		const string Netflix = "11101200001";
		const string Disney = "3201901017640";
		const string YouTube = "111299001912";
		public SamsungTv(Tv tv) {
			settings = new Settings(
				appName: "FreakaZoneRemoteTV", // converted to base64 string as ID for TV
				ipAddr: tv.ip, // IP of TV
				subnet: "255.255.0.0", // Subnet (required for TurnOn() function)
				macAddr: tv.mac, // MAC address of TV (required for TurnOn() function)

				//55000 (< 2014), 8080 (2014 & 2015), 8001 & 8002 (>= 2016)
				port: tv.port, // Port for WebSocket communication
				token: tv.token
			);
		}
		public async Task SimulateNetflix() {
			using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
				if(!remote.IsTvOn()) {
					remote.TurnOn();
					await Task.Delay(2000);
				}
				remote.Connect();
				remote.StartService(Netflix);
			}
		}
		public async Task SimulateDisney() {
			using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
				if(!remote.IsTvOn()) {
					remote.TurnOn();
					await Task.Delay(2000);
				}
				remote.Connect();
				remote.StartService(Disney);
			}
		}
		public async Task SimulateYouTube() {
			using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
				if(!remote.IsTvOn()) {
					remote.TurnOn();
					await Task.Delay(2000);
				}
				remote.Connect();
				remote.StartService(YouTube);
			}
		}
		public async Task SimulateVolumeUp() {
			using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
				if(!remote.IsTvOn()) {
					remote.TurnOn();
					await Task.Delay(2000);
				}
				remote.Connect();
				remote.Press(Remotekeys.VOLUP);
				await Task.Delay(300);
				remote.Press(Remotekeys.VOLUP);
				await Task.Delay(300);
				remote.Press(Remotekeys.VOLUP);
				await Task.Delay(300);
				remote.Press(Remotekeys.VOLUP);
			}
		}
		public async Task SimulateVolumeDown() {
			using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
				if(!remote.IsTvOn()) {
					remote.TurnOn();
					await Task.Delay(2000);
				}
				remote.Connect();
				remote.Press(Remotekeys.VOLDOWN);
				await Task.Delay(300);
				remote.Press(Remotekeys.VOLDOWN);
				await Task.Delay(300);
				remote.Press(Remotekeys.VOLDOWN);
				await Task.Delay(300);
				remote.Press(Remotekeys.VOLDOWN);
			}
		}
		public async Task SimulateDirection(int direction) {
			if(direction >= 0 && direction <= 3) {
				using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
					if(!remote.IsTvOn()) {
						remote.TurnOn();
						await Task.Delay(2000);
					}
					remote.Connect();
					switch(direction) {
						case Direction.UP:
							remote.Press(Remotekeys.UP);
							break;
						case Direction.RIGHT:
							remote.Press(Remotekeys.RIGHT);
							break;
						case Direction.DOWN:
							remote.Press(Remotekeys.DOWN);
							break;
						case Direction.LEFT:
							remote.Press(Remotekeys.LEFT);
							break;
					}
				}
			} else {
				Logger.Write(MethodBase.GetCurrentMethod(), $"unknown Direction: {direction}");
			}
		}
		public async Task SimulateOK() {
			using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
				if(!remote.IsTvOn()) {
					remote.TurnOn();
					await Task.Delay(2000);
				}
				remote.Connect();
				remote.Press(Remotekeys.ENTER);
			}
		}
		public async Task SimulateReturn() {
			using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
				if(!remote.IsTvOn()) {
					remote.TurnOn();
					await Task.Delay(2000);
				}
				remote.Connect();
				remote.Press(Remotekeys.RETURN);
			}
		}
		public async Task<string> SimulateOff() {
			using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
				if(remote.IsTvOn()) {
					remote.Connect();
					remote.Press(Remotekeys.POWER);
					return "aus";
				} else {
					remote.TurnOn();
					await Task.Delay(2000);
					return "ein";
				}
			}
		}
		public async Task SimulateOn() {
			using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
				if(!remote.IsTvOn()) {
					remote.TurnOn();
					await Task.Delay(2000);
				}
			}
		}
	}
	public class Direction {
		public const int UP = 0;
		public const int RIGHT = 1;
		public const int DOWN = 2;
		public const int LEFT = 3;
	}
}