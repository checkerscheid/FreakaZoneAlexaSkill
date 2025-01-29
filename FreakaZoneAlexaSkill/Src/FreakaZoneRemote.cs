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
//# File-ID      : $Id:: FreakaZoneRemote.cs 154 2025-01-29 18:33:30Z             $ #
//#                                                                                 #
//###################################################################################
using FreakaZoneAlexaSkill.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using WebSocketSharp;

namespace FreakaZoneAlexaSkill.Src {
	public class FreakaZoneRemote: IDisposable {
		private string wsUrl;
		private Settings settings;
		private WebSocket? wsClient;
		private bool _disposed;
		public FreakaZoneRemote(Settings s) {
			settings = s;
			string protokol = settings.Port == 8002 ? "wss" : "ws";
			wsUrl = $"{protokol}://{settings.IpAddr}:{settings.Port}/api/v2/channels/samsung.remote.control?name={settings.AppName}";
			//Connect();
			_disposed = false;
		}
		public async void Connect() {
			await Task.Run(() => {
				if(settings.Token != null) {
					wsUrl += "&token=" + settings.Token;
				} else {
					GenerateNewToken();
				}
				try {
					wsClient = new WebSocket(wsUrl);
					wsClient.OnOpen += WsClient_OnOpen;
					wsClient.OnMessage += WsClient_OnMessage;
					wsClient.OnClose += WsClient_OnClose;
					wsClient.OnError += WsClient_OnError;
					Logger.Write(MethodBase.GetCurrentMethod(), $"websocket: {wsUrl}");
					wsClient?.Connect();
				} catch(Exception ex) {
					Logger.WriteError(MethodBase.GetCurrentMethod(), ex);
				}
			});
		}

		public void StartService(string id) {
			if(settings?.Token == "") {
				throw new ArgumentNullException("Token is ***null***");
			}
			string data = JsonConvert.SerializeObject(new SericeCommand(new ServiceParameters(new ServiceData(id)))).Replace("parameters", "params").Replace("pevent", "event");
			Logger.Write(MethodBase.GetCurrentMethod(), $"Sending key: {id}");
			wsClient?.Send(data);
		}
		public async Task Press(string key) {
			await Task.Run(() => {
				if(settings?.Token == "") {
					throw new ArgumentNullException("Token is ***null***");
				}
				string data = JsonConvert.SerializeObject(new KeyCommand(new KeyParameters(key))).Replace("parameters", "params");
				Logger.Write(MethodBase.GetCurrentMethod(), $"Sending key: {key}");
				wsClient?.Send(data);
			});
		}

		public bool IsTvOn() {
			Task<bool> task = Task.Run(() => IsTvOnAsync());
			task.Wait();
			return task.Result;
		}
		private async Task<bool> IsTvOnAsync() {
			Ping p = new Ping();
			PingReply pr = p.Send(settings.IpAddr, 1000);
			if(pr.Status == IPStatus.Success) {
				Logger.Write(MethodBase.GetCurrentMethod(), $"Ping success: {settings.IpAddr}");
				using HttpClient client = new HttpClient();
				string requestUri = "http://" + settings.IpAddr + ":8001/api/v2/";
				client.Timeout = TimeSpan.FromSeconds(1);
				HttpResponseMessage response;
				try {
					response = await client.GetAsync(requestUri);
				} catch(WebException) {
					Logger.Write(MethodBase.GetCurrentMethod(), $"!!! Status Page unavailable: {requestUri}");
					return false;
				}
				await response.Content.ReadAsStringAsync();
				if(response.StatusCode == HttpStatusCode.OK) {
					Logger.Write(MethodBase.GetCurrentMethod(), $"Status Page available: {requestUri}");
					return true;
				}
			}
			Logger.Write(MethodBase.GetCurrentMethod(), $"!!! Ping failed: {settings.IpAddr}");
			return false;
		}
		public async void TurnOn() {
			await WoL.WakeOnLan(settings.MacAddr);
		}
		public void GenerateNewToken() {
			Task.Run(() => GenerateNewTokenAsync()).Wait();
		}

		public async Task GenerateNewTokenAsync() {
			CancellationTokenSource tokenSource = new CancellationTokenSource();
			Logger.Write(MethodBase.GetCurrentMethod(), "Try to generate new Token");
			using(WebSocket gntClient = new WebSocket(wsUrl)) {
				gntClient.OnOpen += WsClient_OnOpen;
				gntClient.OnMessage += WsClient_OnMessage;
				gntClient.OnClose += WsClient_OnClose;
				gntClient.OnError += WsClient_OnError;
				Logger.Write(MethodBase.GetCurrentMethod(), $"websocket token: {wsUrl}");
				gntClient?.Connect();
				Logger.Write(MethodBase.GetCurrentMethod(), "Accept dialog for new connection on TV...");
				try {
					await Task.Delay(30000, tokenSource.Token);
				} catch(OperationCanceledException ex) {
					Logger.WriteError(MethodBase.GetCurrentMethod(), ex);
				}
			}
		}
		private void WsClient_OnOpen(object? sender, EventArgs e) {
			Logger.Write(MethodBase.GetCurrentMethod(), $"ServerConnected with token: '{settings.Token}'");
		}
		private void WsClient_OnMessage(object? sender, MessageEventArgs e) {
			//string s = Encoding.UTF8.GetString(bytes: e.Data.Array).Replace("\0", string.Empty);
			JObject json = JObject.Parse(e.Data);
			Logger.Write(MethodBase.GetCurrentMethod(), $"OnMessage data: '{e.Data.Trim()}'");
			string method = json["event"]?.ToString() ?? String.Empty;
			if(method.Equals("ms.channel.connect") && settings.Token == "") {
				string newToken = json["data"]?["token"]?.ToString() ?? String.Empty;
				settings.Token = newToken;
				Logger.Write(MethodBase.GetCurrentMethod(), $"New token: '{settings.Token}' generated");
			}
		}
		private void WsClient_OnClose(object? sender, CloseEventArgs e) {
			Logger.Write(MethodBase.GetCurrentMethod(), $"ServerConnection cloesd");
		}
		private void WsClient_OnError(object? sender, WebSocketSharp.ErrorEventArgs e) {
			Exception ex = e.Exception;
			Logger.WriteError(MethodBase.GetCurrentMethod(), ex);
		}

		public void Dispose() {
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if(!_disposed && disposing) {
				wsClient?.Close();
				_disposed = true;
			}
		}
		internal class SericeCommand {
			public string method { get; set; }
			public ServiceParameters parameters { get; set; }
			public SericeCommand(ServiceParameters p) {
				method = "ms.channel.emit";
				parameters = p;
			}
		}
		internal class ServiceParameters {
			public string pevent { get; set; }
			public string to { get; set; }
			public ServiceData data { get; set; }
			public ServiceParameters(ServiceData d) {
				pevent = "ed.apps.launch";
				to = "host";
				data = d;
			}
		}
		internal class ServiceData {
			public string appId { get; set; }
			public string action_type { get; set; }
			public ServiceData(string id) {
				appId = id;
				action_type = "DEEP_LINK";
			}
		}
		internal class KeyCommand {
			public string method { get; set; }
			public KeyParameters parameters { get; set; }
			public KeyCommand(KeyParameters p) {
				method = "ms.remote.control";
				parameters = p;
			}
		}
		internal class KeyParameters {
			public string Cmd { get; set; }
			public string DataOfCmd { get; set; }
			public string Option { get; set; }
			public string TypeOfRemote { get; set; }
			public KeyParameters(string key) {
				Cmd = "Click";
				DataOfCmd = key;
				Option = "false";
				TypeOfRemote = "SendRemoteKey";
			}
		}
	}
}
