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
//# Revision     : $Rev:: 154                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: SamsungTv.cs 154 2025-01-29 18:33:30Z                    $ #
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
		public void SimulateVolumeUp() {
			Task.Run(async () => {
				await SimulateVolumeUpAsync();
			});
		}
		public async Task SimulateVolumeUpAsync() {
			using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
				if(!remote.IsTvOn()) {
					remote.TurnOn();
					await Task.Delay(2000);
				}
				int delay = 250;
				int count = 5;
				remote.Connect();
				for(int i = 0; i < count; i++) {
					await remote.Press(Keys.VOLUP);
					await Task.Delay(delay);
				}
			}
		}
		public void SimulateVolumeDown() {
			Task.Run(async () => {
				await SimulateVolumeDownAsync();
			});
		}
		public async Task SimulateVolumeDownAsync() {
			using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
				if(!remote.IsTvOn()) {
					remote.TurnOn();
					await Task.Delay(2000);
				}
				int delay = 250;
				int count = 5;
				remote.Connect();
				for(int i = 0; i < count; i++) {
					await remote.Press(Keys.VOLDOWN);
					await Task.Delay(delay);
				}
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
							await remote.Press(Keys.UP);
							break;
						case Direction.RIGHT:
							await remote.Press(Keys.RIGHT);
							break;
						case Direction.DOWN:
							await remote.Press(Keys.DOWN);
							break;
						case Direction.LEFT:
							await remote.Press(Keys.LEFT);
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
				await remote.Press(Keys.ENTER);
			}
		}
		public async Task SimulateReturn() {
			using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
				if(!remote.IsTvOn()) {
					remote.TurnOn();
					await Task.Delay(2000);
				}
				remote.Connect();
				await remote.Press(Keys.RETURN);
			}
		}
		public async Task<string> SimulateOff() {
			using(FreakaZoneRemote remote = new FreakaZoneRemote(settings)) {
				if(remote.IsTvOn()) {
					remote.Connect();
					await remote.Press(Keys.POWER);
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