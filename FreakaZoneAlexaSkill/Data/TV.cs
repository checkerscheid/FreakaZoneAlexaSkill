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
//# Revision     : $Rev:: 150                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: TV.cs 150 2024-12-14 16:20:21Z                           $ #
//#                                                                                 #
//###################################################################################
using Alexa.NET.Response;
using FreakaZoneAlexaSkill.Src;
using System.Reflection;

namespace FreakaZoneAlexaSkill.Data {
	public class Tv : IData {
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
		private bool Set(TVParams param, out IOutputSpeech returnmsg) {
			bool returns = false;
			returnmsg = new PlainTextOutputSpeech("Da ist was schief gelaufen");

			if(param.einaus == null && param.tvbutton == null && param.dienst == null && param.richtung == null) {
				returnmsg = new PlainTextOutputSpeech($"{_name} wird eingeschaltet");
				_ = _tv.SimulateReturn();
				Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} eingeschaltet");
				returns = true;
			} else {
				if(param.einaus != null) {
					returnmsg = new PlainTextOutputSpeech($"{_name} twist power");
					_ = _tv.SimulateOff();
					Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} geschaltet");
					returns = true;
				}
				if(param.tvbutton != null) {
					switch(param.tvbutton) {
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
				if(param.dienst != null) {
					switch(param.dienst) {
						case "netflix":
							returnmsg = new PlainTextOutputSpeech($"{param.dienst} auf {_name} t. v. wird gestartet");
							_ = _tv.SimulateNetflix();
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} Netflix gestartet");
							returns = true;
							break;
						case "disney":
							returnmsg = new PlainTextOutputSpeech($"{param.dienst} auf {_name} t. v. wird gestartet");
							_ = _tv.SimulateDisney();
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} Disney+ gestartet");
							returns = true;
							break;
						case "youtube":
							returnmsg = new PlainTextOutputSpeech($"{param.dienst} auf {_name} t. v. wird gestartet");
							_ = _tv.SimulateYouTube();
							Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} YouTube gestartet");
							returns = true;
							break;
					}
				}
				if(param.richtung != null) {
					switch(param.richtung) {
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
			}
			return returns;
		}
		public bool Set(IParams param, out IOutputSpeech returnmsg) {
			if(param.GetType() == typeof(TVParams)) {
				return Set((TVParams) param, out returnmsg);
			}
			returnmsg = new PlainTextOutputSpeech($"{_name} hat einen falschen Parameter");
			return false;
		}
		public bool Set(IParams param) {
			IOutputSpeech returnmsg; // dummy
			if(param.GetType() == typeof(TVParams)) {
				return Set((TVParams)param, out returnmsg);
			}
			Logger.Write(MethodBase.GetCurrentMethod(), $"{_name} hat einen falschen Parameter");
			return false;
		}
	}
	public class Tvs : IList {
		private List<Tv> tvs;
		public Tvs() {
			tvs = new List<Tv>();
		}
		public void init() {
			tvs.Add(new Tv("wohnzimmer", "172.17.80.40", 8002, "64-1C-AE-E7-BE-C9", "87551621")); //15550794
			tvs.Add(new Tv("schlafzimmer", "172.17.80.43", 8002, "8C-EA-48-5E-75-98", ""));
			tvs.Add(new Tv("kinderzimmer", "172.17.80.45", 8002, "00-C3-F4-F1-99-A0", "82894561"));
			tvs.Add(new Tv("pia", "172.17.80.45", 8002, "00-C3-F4-F1-99-A0", "82894561"));
		}
		public IData Get(string? name) {
			return tvs.Find(ll => ll.name == name?.ToLower()) ?? new Tv("noDevice", "", 0, "", "");
		}
	}
	public class TVParams : IParams {
		private string? _einaus;
		public string? einaus { get { return _einaus; } }
		private string? _tvbutton;
		public string? tvbutton { get { return _tvbutton; } }
		private string? _dienst;
		public string? dienst { get { return _dienst; } }
		private string? _richtung;
		public string? richtung { get { return _richtung; } }
		public TVParams(string? einaus, string? tvbutton, string? dienst, string? richtung) {
			_einaus = einaus;
			_tvbutton = tvbutton;
			_dienst = dienst;
			_richtung = richtung;
		}
		public override string ToString() {
			return $"einaus: {_einaus}, tvbutton: {_tvbutton}, dienst: {_dienst}, richtung: {_richtung}";
		}

	}
}