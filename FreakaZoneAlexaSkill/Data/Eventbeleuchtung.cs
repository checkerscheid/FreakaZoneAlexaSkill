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
//# File-ID      : $Id:: Eventbeleuchtung.cs 145 2024-12-05 19:12:44Z             $ #
//#                                                                                 #
//###################################################################################
using Alexa.NET.Response;
using FreakaZoneAlexaSkill.Src;
using System.Reflection;

namespace FreakaZoneAlexaSkill.Data {
	public class Eventbeleuchtung {
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
		public Eventbeleuchtung(string Name, string Ip) {
			_name = Name;
			_ip = Ip;
		}
		public bool Set(string? einaus, string? prozent, string? linksrechts, out IOutputSpeech returnmsg) {
			bool returns = false;
			returnmsg = new PlainTextOutputSpeech("Da ist was schief gelaufen");

			string target = linksrechts?? "";
			if(einaus != null) {
				switch(einaus) {
					case "ein":
					case "an":
						returnmsg = new PlainTextOutputSpeech($"Joo, {_name} {target} is an gemacht");
						if(target == "") _ = hitUrl("setCwWw?ww=50&cw=50");
						if(target == "links") _ = hitUrl("setCwWw?ww=50");
						if(target == "rechts") _ = hitUrl("setCwWw?cw=50");
						returns = true;
						break;
					case "aus":
						returnmsg = new PlainTextOutputSpeech($"Joo, {_name} {target} is aus gemacht");
						if(target == "") _ = hitUrl("setCwWw?ww=0&cw=0");
						if(target == "links") _ = hitUrl("setCwWw?ww=0");
						if(target == "rechts") _ = hitUrl("setCwWw?cw=0");
						returns = true;
						break;
				}
			}
			if(prozent != null) {
				int p;
				if(Int32.TryParse(prozent, out p)) {
					returnmsg = new PlainTextOutputSpeech($"Joo, {_name} {target} {p} prozent is gemacht");
					if(target == "")
						_ = hitUrl($"setCwWw?ww={p}&cw={p}");
					if(target == "links")
						_ = hitUrl($"setCwWw?ww={p}");
					if(target == "rechts")
						_ = hitUrl($"setCwWw?cw={p}");
					returns = true;
				}
			}
			if(einaus == null && prozent == null) {
				returnmsg = new PlainTextOutputSpeech($"Joo, {_name} {target} effect is gemacht");
				if(target == "")
					_ = hitUrl($"setCwWwEffect?effect=4");
				if(target == "links")
					_ = hitUrl($"setCwWwEffect?effect=5");
				if(target == "rechts")
					_ = hitUrl($"setCwWwEffect?effect=6");
				returns = true;
			}
			return returns;
		}
		private async Task hitUrl(string cmd) {
			HttpClient client = new HttpClient();
			string url = $"http://{_ip}/{cmd}";
			HttpResponseMessage response = await client.GetAsync(url);
			string responseBody = await response.Content.ReadAsStringAsync();
			Logger.Write(MethodBase.GetCurrentMethod(), $"{url}: {responseBody}");
		}
	}
	public class Eventbeleuchtungen {
		private List<Eventbeleuchtung> eventbeleuchtungen;
		public Eventbeleuchtungen() {
			eventbeleuchtungen = new List<Eventbeleuchtung>();
		}
		public void init() {
			eventbeleuchtungen.Add(new Eventbeleuchtung("wohnzimmer", "172.17.80.97"));
			eventbeleuchtungen.Add(new Eventbeleuchtung("küche", "172.17.80.142"));
			eventbeleuchtungen.Add(new Eventbeleuchtung("kinderzimmer", "172.17.80.164"));
			eventbeleuchtungen.Add(new Eventbeleuchtung("pia", "172.17.80.164"));
		}
		public Eventbeleuchtung Get(string? name) {
			return eventbeleuchtungen.Find(eb => eb.name == name?.ToLower()) ?? new Eventbeleuchtung("noDevice", "");
		}
	}
}