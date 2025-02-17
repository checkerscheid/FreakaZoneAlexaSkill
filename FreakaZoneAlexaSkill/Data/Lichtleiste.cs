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
//# Revision     : $Rev:: 187                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: Lichtleiste.cs 187 2025-02-17 00:57:15Z                  $ #
//#                                                                                 #
//###################################################################################
using FreakaZone.Libraries.wpCommon;
using FreakaZone.Libraries.wpEventLog;
using System.Reflection;

namespace FreakaZoneAlexaSkill.Data {
	public class Lichtleiste : IData {
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
		public Lichtleiste(string Name, string Ip) {
			_name = Name;
			_ip = Ip;
		}
		private AlexaReturnType Set(LichtleisteParams param, out string returnmsg) {
			AlexaReturnType returns = AlexaReturnType.Error;
			returnmsg = "Da ist was schief gelaufen";
			if(param.einaus == null && param.prozent == null) {
				// case "rainbow":
					_ = hitUrl("setNeoPixelEffect?effect=3");
					returnmsg = $"<speak><amazon:emotion name=\"disappointed\" intensity=\"high\">Joo, {_name} rainbow is gemacht</amazon:emotion><amazon:effect name=\"whispered\">aber bitte schlag mich nicht schon wieder</amazon:effect></speak>";
					returns = AlexaReturnType.Ssml;
				//	break;
				//case "rainbow wheel":
				//	returnmsg = new SsmlOutputSpeech($"<speak><amazon:emotion name=\"disappointed\" intensity=\"high\">Joo, {_name} rainbow wheel is gemacht</amazon:emotion><amazon:effect name=\"whispered\">aber bitte schlag mich nicht schon wieder</amazon:effect></speak>");
				//	_ = hitUrl("setNeoPixelEffect?effect=4");
				//	returns = true;
				//	break;
			}
			if(param.einaus != null) {
				switch(param.einaus) {
					case "ein":
					case "an":
						_ = hitUrl("setNeoPixelOn");
						returnmsg = $"Joo, {_name} is an gemacht";
						returns = AlexaReturnType.String;
						break;
					case "aus":
						_ = hitUrl("setNeoPixelOff");
						returnmsg = $"Joo, {_name} is aus gemacht";
						returns = AlexaReturnType.String;
						break;
					case "arztzimmer":
						_ = hitUrl("setNeoPixelColor?r=0&g=0&b=0");
						_ = hitUrl("setNeoPixelWW?ww=0");
						_ = hitUrl("setNeoPixelCW?cw=75");
						returnmsg = $"<speak><amazon:emotion name=\"disappointed\" intensity=\"low\">Joo, {_name} arztzimmer is gemacht</amazon:emotion></speak>";
						returns = AlexaReturnType.Ssml;
						break;
					case "sonnenschein":
					case "sonne":
						_ = hitUrl("setNeoPixelColor?r=0&g=0&b=0");
						_ = hitUrl("setNeoPixelWW?ww=75");
						_ = hitUrl("setNeoPixelCW?cw=25");
						returnmsg = $"<speak><amazon:emotion name=\"disappointed\" intensity=\"low\">Joo, {_name} sonnenschein is gemacht</amazon:emotion></speak>";
						returns = AlexaReturnType.Ssml;
						break;
					case "gemütlich":
						_ = hitUrl("setNeoPixelColor?r=0&g=0&b=0");
						_ = hitUrl("setNeoPixelWW?ww=50");
						_ = hitUrl("setNeoPixelCW?cw=5");
						returnmsg = $"<speak>Psst, <amazon:effect name=\"whispered\">{_name} gemütlich is gemacht</amazon:effect></speak>";
						returns = AlexaReturnType.Ssml;
						break;
				}
			}
			if(param.prozent != null) {
				int p;
				if(Int32.TryParse(param.prozent, out p)) {
					if(p > 100)	p = 100;
					if(p < 0) p = 0;
					_ = hitUrl($"setNeoPixelBrightness?brightness={p * 2.55}");
					returnmsg = $"Joo, {_name} {p} prozent is gemacht";
					returns = AlexaReturnType.String;
				}
			}

			return returns;
		}
		public AlexaReturnType Set(IParams param, out string returnmsg) {
			if(param.GetType() == typeof(LichtleisteParams)) {
				return Set((LichtleisteParams)param, out returnmsg);
			}
			returnmsg = $"{_name} hat einen falschen Parameter";
			return AlexaReturnType.String;
		}
		private async Task hitUrl(string cmd) {
			HttpClient client = new HttpClient();
			string url = $"http://{_ip}/{cmd}";
			HttpResponseMessage response = await client.GetAsync(url);
			string responseBody = await response.Content.ReadAsStringAsync();
			Debug.Write(MethodBase.GetCurrentMethod(), $"{url}: {responseBody}");
		}
	}
	public class Lichtleisten: IList {
		private List<Lichtleiste> lichtleisten;
		public Lichtleisten() {
			lichtleisten = new List<Lichtleiste>();
		}
		public void init() {
			lichtleisten.Add(new Lichtleiste("wohnzimmer", "172.17.80.99"));
			lichtleisten.Add(new Lichtleiste("lautsprecher", "172.17.80.98"));
			lichtleisten.Add(new Lichtleiste("pflanze", "172.17.80.106"));
			lichtleisten.Add(new Lichtleiste("büro", "172.17.80.122"));
			lichtleisten.Add(new Lichtleiste("flur", "172.17.80.122"));
			lichtleisten.Add(new Lichtleiste("kinderzimmer", "172.17.80.169"));
			lichtleisten.Add(new Lichtleiste("pia", "172.17.80.169"));
		}
		public IData Get(string? name) {
			return lichtleisten.Find(ll => ll.name == name?.ToLower()) ?? new Lichtleiste("noDevice", "");
		}
	}
	public class LichtleisteParams : IParams {
		private string? _einaus;
		public string? einaus { get {  return _einaus; } }
		private string? _prozent;
		public string? prozent {  get { return _prozent; } }
		public LichtleisteParams(string? einaus, string? prozent) {
			_einaus = einaus;
			_prozent = prozent;
		}
		public override string ToString() {
			return $"einaus: {_einaus}, prozent: {_prozent}";
		}
	}
}