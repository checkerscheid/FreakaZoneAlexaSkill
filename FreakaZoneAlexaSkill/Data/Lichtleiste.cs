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
//# Revision     : $Rev:: 146                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: Lichtleiste.cs 146 2024-12-07 12:43:11Z                  $ #
//#                                                                                 #
//###################################################################################
using Alexa.NET.Response;
using FreakaZoneAlexaSkill.Src;
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
		private bool Set(LichtleisteParams param, out IOutputSpeech returnmsg) {
			bool returns = false;
			returnmsg = new PlainTextOutputSpeech("Da ist was schief gelaufen");
			if(param.einaus == null && param.prozent == null) {
				// case "rainbow":
					returnmsg = new SsmlOutputSpeech($"<speak><amazon:emotion name=\"disappointed\" intensity=\"high\">Joo, {_name} rainbow is gemacht</amazon:emotion><amazon:effect name=\"whispered\">aber bitte schlag mich nicht schon wieder</amazon:effect></speak>");
					_ = hitUrl("setNeoPixelEffect?effect=3");
					returns = true;
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
						returnmsg = new PlainTextOutputSpeech($"Joo, {_name} is an gemacht");
						_ = hitUrl("setNeoPixelOn");
						returns = true;
						break;
					case "aus":
						returnmsg = new PlainTextOutputSpeech($"Joo, {_name} is aus gemacht");
						_ = hitUrl("setNeoPixelOff");
						returns = true;
						break;
					case "arztzimmer":
						returnmsg = new SsmlOutputSpeech($"<speak><amazon:emotion name=\"disappointed\" intensity=\"low\">Joo, {_name} arztzimmer is gemacht</amazon:emotion></speak>");
						_ = hitUrl("setNeoPixelColor?r=0&g=0&b=0");
						_ = hitUrl("setNeoPixelWW?ww=0");
						_ = hitUrl("setNeoPixelCW?cw=75");
						returns = true;
						break;
					case "sonnenschein":
					case "sonne":
						returnmsg = new SsmlOutputSpeech($"<speak><amazon:emotion name=\"disappointed\" intensity=\"low\">Joo, {_name} sonnenschein is gemacht</amazon:emotion></speak>");
						_ = hitUrl("setNeoPixelColor?r=0&g=0&b=0");
						_ = hitUrl("setNeoPixelWW?ww=75");
						_ = hitUrl("setNeoPixelCW?cw=25");
						returns = true;
						break;
					case "gemütlich":
						returnmsg = new SsmlOutputSpeech($"<speak>Psst, <amazon:effect name=\"whispered\">{_name} gemütlich is gemacht</amazon:effect></speak>");
						_ = hitUrl("setNeoPixelColor?r=0&g=0&b=0");
						_ = hitUrl("setNeoPixelWW?ww=50");
						_ = hitUrl("setNeoPixelCW?cw=5");
						returns = true;
						break;
				}
			}
			if(param.prozent != null) {
				int p;
				if(Int32.TryParse(param.prozent, out p)) {
					returnmsg = new PlainTextOutputSpeech($"Joo, {_name} {p} prozent is gemacht");
					if(p > 100)	p = 100;
					if(p < 0) p = 0;
					_ = hitUrl($"setNeoPixelBrightness?brightness={p * 2.55}");
					returns = true;
				}
			}

			return returns;
		}
		public bool Set(IParams param, out IOutputSpeech returnmsg) {
			if(param.GetType() == typeof(LichtleisteParams)) {
				return Set((EventbeleuchtungParams) param, out returnmsg);
			}
			returnmsg = new PlainTextOutputSpeech($"{_name} hat einen falschen Parameter");
			return false;
		}
		private async Task hitUrl(string cmd) {
			HttpClient client = new HttpClient();
			string url = $"http://{_ip}/{cmd}";
			HttpResponseMessage response = await client.GetAsync(url);
			string responseBody = await response.Content.ReadAsStringAsync();
			Logger.Write(MethodBase.GetCurrentMethod(), $"{url}: {responseBody}");
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