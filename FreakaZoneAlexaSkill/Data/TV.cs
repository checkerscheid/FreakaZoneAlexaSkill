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
//# File-ID      : $Id:: TV.cs 145 2024-12-05 19:12:44Z                           $ #
//#                                                                                 #
//###################################################################################
using Alexa.NET.Response;
using FreakaZoneAlexaSkill.Src;
using System.Reflection;

namespace FreakaZoneAlexaSkill.Data {
	public class Tv {
		private string _name;
		public string name {
			get { return _name; }
			set { _name = value; }
		}
		private string _ip;
		public string ip {
			get { return _ip; }
			set { _ip = value; }
		}
		private int _port;
		public int port {
			get { return _port; }
			set { _port = value; }
		}

		private string _mac;
		public string mac {
			get { return _mac; }
			set { _mac = value; }
		}
		private string _token;
		public string token {
			get { return _token; }
			set { _token = value; }
		}
		private SamsungTv _tv;
		public Tv(string Name, string Ip, int Port, string Mac, string Token) {
			_name = Name;
			_ip = Ip;
			_port = Port;
			_mac = Mac;
			_token = Token;
			_tv = new SamsungTv(this);
		}
		public bool Set(string? einaus, string? dienst, string? leiserlauter, string? richtung, string? okquit, out IOutputSpeech returnmsg) {
			bool returns = false;
			returnmsg = new PlainTextOutputSpeech("Da ist was schief gelaufen");

			if(einaus == null && dienst == null && leiserlauter == null && richtung == null && okquit == null) {
				returnmsg = new PlainTextOutputSpeech($"{_name} wird eingeschaltet");
				_ = _tv.SimulateReturn();
				Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} eingeschaltet");
				returns = true;
			} else {
				if(einaus != null) {
					returnmsg = new PlainTextOutputSpeech($"{_name} wird ausgeschaltet");
					_ = _tv.SimulateOff();
					Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} ausgeschaltet");
					returns = true;
				}
				if(dienst != null) {
					switch(dienst) {
						case "netflix":
							returnmsg = new PlainTextOutputSpeech($"{dienst} auf {_name} t. v. wird gestartet");
							_ = _tv.SimulateNetflix();
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} Netflix gestartet");
							returns = true;
							break;
						case "disney":
							returnmsg = new PlainTextOutputSpeech($"{dienst} auf {_name} t. v. wird gestartet");
							_ = _tv.SimulateDisney();
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} Disney+ gestartet");
							returns = true;
							break;
						case "youtube":
							returnmsg = new PlainTextOutputSpeech($"{dienst} auf {_name} t. v. wird gestartet");
							_ = _tv.SimulateYouTube();
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} YouTube gestartet");
							returns = true;
							break;
					}
				}
				if(leiserlauter != null) {
					switch(leiserlauter) {
						case "lauter":
							returnmsg = new PlainTextOutputSpeech($"{name} lauter");
							_ = _tv.SimulateVolumeUp();
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} lauter");
							returns = true;
							break;
						case "leiser":
							returnmsg = new PlainTextOutputSpeech($"{name} leiser");
							_ = _tv.SimulateVolumeDown();
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} leiser");
							returns = true;
							break;
					}
				}
				if(richtung != null) {
					switch(richtung) {
						case "hoch":
						case "nach oben":
							returnmsg =  new PlainTextOutputSpeech($"{name} hoch");
							_ = _tv.SimulateDirection(Direction.UP);
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} hoch");
							returns = true;
							break;
						case "runter":
						case "nach unten":
							returnmsg =  new PlainTextOutputSpeech($"{name} runter");
							_ = _tv.SimulateDirection(Direction.DOWN);
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} runter");
							returns = true;
							break;
						case "links":
						case "nach links":
							returnmsg =  new PlainTextOutputSpeech($"{name} links");
							_ = _tv.SimulateDirection(Direction.LEFT);
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} links");
							returns = true;
							break;
						case "rechts":
						case "nach rechts":
							returnmsg =  new PlainTextOutputSpeech($"{name} rechts");
							_ = _tv.SimulateDirection(Direction.RIGHT);
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} rechts");
							returns = true;
							break;
					}
				}
				if(okquit != null) {
					switch(okquit) {
						case "o. k.":
						case "okey":
						case "okay":
						case "speichern":
						case "enter":
							returnmsg = new PlainTextOutputSpeech($"{name} okay");
							_ = _tv.SimulateOK();
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} okay");
							returns = true;
							break;
						case "abbrechen":
						case "zurück":
							returnmsg = new PlainTextOutputSpeech($"{name} zurück");
							_ = _tv.SimulateReturn();
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} zurück");
							returns = true;
							break;
					}
				}
			}

			return returns;
		}
	}
	public class Tvs {
		private List<Tv> tvs;
		public Tvs() {
			tvs = new List<Tv>();
		}
		public void init() {
			tvs.Add(new Tv("wohnzimmer", "172.17.80.40", 8001, "64-1C-AE-E7-BE-C9", ""));
			tvs.Add(new Tv("schlafzimmer", "172.17.80.43", 8001, "8C-EA-48-5E-75-98", ""));
			tvs.Add(new Tv("kinderzimmer", "172.17.80.45", 8001, "00-C3-F4-F1-99-A0", ""));
			tvs.Add(new Tv("pia", "172.17.80.45", 8001, "00-C3-F4-F1-99-A0", ""));
		}
		public Tv Get(string? name) {
			return tvs.Find(ll => ll.name == name?.ToLower()) ?? new Tv("noDevice", "", 0, "", "");
		}
	}
}