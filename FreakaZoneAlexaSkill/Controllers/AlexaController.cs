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
//# Revision     : $Rev:: 206                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: AlexaController.cs 206 2025-05-03 00:08:15Z              $ #
//#                                                                                 #
//###################################################################################
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using FreakaZone.Libraries.wpCommon;
using FreakaZone.Libraries.wpEventLog;
using FreakaZone.Libraries.wpSamsungRemote;
using FreakaZoneAlexaSkill.Data;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace FreakaZoneAlexaSkill.Controllers {
	[ApiController]
	[Route("[controller]")]
	public class AlexaController: Controller {
		const string INTENT_SLEEPNOW = "sleepnow";
		const string INTENT_RGBCCT = "rgbcct";
		const string INTENT_EVENTLIGHTING = "lichterkette";
		const string INTENT_FERNSEHER = "fernseher";

		private IOutputSpeech defaultMsg = new PlainTextOutputSpeech("Da ist was schief gelaufen");

		[HttpPost, Route("/process")]
		public SkillResponse Process(SkillRequest input) {
			SkillResponse output = new SkillResponse();

			Lichtleisten lichtleisten = new Lichtleisten();
			Eventbeleuchtungen eventbeleuchtungen = new Eventbeleuchtungen();
			Tvs tvs = new Tvs(false);

			lichtleisten.init();
			eventbeleuchtungen.init();

			output.Version = "1.0";
			output.Response = new ResponseBody();

			Debug.Write(MethodBase.GetCurrentMethod(), $"New Request: {input.Request.Type} detected");

			switch(input.Request.Type) {
				case "LaunchRequest":
					output.Response.OutputSpeech = new SsmlOutputSpeech("<speak><amazon:emotion name=\"excited\" intensity=\"high\">waaas</amazon:emotion></speak>");
					output.Response.ShouldEndSession = false;
					Debug.Write(MethodBase.GetCurrentMethod(), $"LaunchRequest detected");
					break;
				case "IntentRequest":
					IntentRequest ir = (IntentRequest)input.Request;
					string? roomname;
					Lichtleiste lichtleiste;
					Eventbeleuchtung eventbeleuchtung;
					Tv tv;
					AlexaReturnType returns;
					string returnMsg;
					IOutputSpeech returnSpeech = defaultMsg;
					switch(ir.Intent.Name) {
						case INTENT_SLEEPNOW:
							Debug.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_SLEEPNOW} detected");

							/// special case for pia and mila
							DateTime tag1 = new DateTime(2025, 4, 12);
							DateTime tag2 = tag1 + new TimeSpan(36, 0, 0);
							if(DateTime.Now > tag1 && DateTime.Now < tag2)
								output.Response.OutputSpeech = new SsmlOutputSpeech("<speak><amazon:emotion name=\"disappointed\" intensity=\"high\">ooooh pia und mila!</amazon:emotion><amazon:effect name=\"whispered\">schlaft gut süße mäuse</amazon:effect></speak>");
							else
								output.Response.OutputSpeech = new SsmlOutputSpeech("<speak><amazon:emotion name=\"disappointed\" intensity=\"high\">ooooh pia!</amazon:emotion><amazon:effect name=\"whispered\">schlaf gut süße maus</amazon:effect></speak>");

							Task.Run(async () => await hitUrl("172.17.80.169", "setNeoPixelColor?r=100&g=5&b=0"));
							Task.Run(async () => await hitUrl("172.17.80.164", "setCwWw?cw=10&ww=0"));
							int h = 0;
							int m = 30;
							int sec = (h * 60 * 60) + (m * 60);
							Task.Run(async () => await hitUrl("172.17.80.169", $"setNeoPixelSleep?sleep={sec}"));
							Task.Run(async () => await hitUrl("172.17.80.164", $"setCwWwSleep?sleep={sec}"));
							Task.Run(async () => await hitUrl("wpLicht:turner@172.17.80.163", "color/0?turn=off"));
							Task.Run(async () => await hitUrl("wpLicht:turner@172.17.80.160", "relay/0?turn=off"));
							Task.Run(async () => await hitUrl("wpLicht:turner@172.17.80.161", "relay/0?turn=off"));
							Task.Run(async () => await hitUrl("wpLicht:turner@172.17.80.162", "relay/0?turn=off"));
							output.Response.ShouldEndSession = true;
							Debug.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_SLEEPNOW} finished");
							break;
						case INTENT_RGBCCT:
							Debug.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_RGBCCT} detected");
							roomname = ir.Intent.Slots?["lichtleiste"]?.SlotValue?.Value;
							lichtleiste = (Lichtleiste)lichtleisten.Get(roomname);
							if(lichtleiste.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Debug.Write(MethodBase.GetCurrentMethod(), $"Lichtleiste: '{roomname}' nicht gefunden");
							} else {
								LichtleisteParams llp = new LichtleisteParams(
									einaus: ir.Intent.Slots?["einaus"]?.SlotValue?.Value,
									prozent: ir.Intent.Slots?["prozent"]?.SlotValue?.Value);
								returns = lichtleiste.Set(llp, out returnMsg);
								output.Response.OutputSpeech = GetOutputSpeech(returns, returnMsg);
								Debug.Write(MethodBase.GetCurrentMethod(), llp.ToString());
							}
							output.Response.ShouldEndSession = true;
							Debug.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_RGBCCT} finished");
							break;
						case INTENT_EVENTLIGHTING:
							Debug.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_EVENTLIGHTING} detected");
							roomname = ir.Intent.Slots?["beleuchtung"]?.SlotValue?.Value;
							eventbeleuchtung = (Eventbeleuchtung)eventbeleuchtungen.Get(roomname);
							if(eventbeleuchtung.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Debug.Write(MethodBase.GetCurrentMethod(), $"Eventbeleuchtung: '{roomname}' nicht gefunden");
							} else {
								EventbeleuchtungParams ebp = new EventbeleuchtungParams(
									einaus: ir.Intent.Slots?["einaus"]?.SlotValue?.Value,
									prozent: ir.Intent.Slots?["prozent"]?.SlotValue?.Value,
									linksrechts: ir.Intent.Slots?["linksrechts"]?.SlotValue?.Value);
								returns = eventbeleuchtung.Set(ebp, out returnMsg);
								output.Response.OutputSpeech = GetOutputSpeech(returns, returnMsg);
								Debug.Write(MethodBase.GetCurrentMethod(), ebp.ToString());
							}
							output.Response.ShouldEndSession = true;
							Debug.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_EVENTLIGHTING} finished");
							break;
						case INTENT_FERNSEHER:
							Debug.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_FERNSEHER} detected");
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = (Tv)tvs.Get(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Debug.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								TVParams tvp = new TVParams(
									einaus: ir.Intent.Slots?["einaus"]?.SlotValue?.Value,
									tvbutton: ir.Intent.Slots?["tvbutton"]?.SlotValue?.Value,
									dienst: ir.Intent.Slots?["dienst"]?.SlotValue?.Value,
									richtung: ir.Intent.Slots?["richtung"]?.SlotValue?.Value);
								returns = tv.Set(tvp, out returnMsg);
								output.Response.OutputSpeech = GetOutputSpeech(returns, returnMsg);
								Debug.Write(MethodBase.GetCurrentMethod(), tvp.ToString());
							}
							output.Response.ShouldEndSession = true;
							Debug.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_FERNSEHER} finished");
							break;
						case "AMAZON.FallbackIntent":
							output.Response.OutputSpeech = new PlainTextOutputSpeech("zu doof zu sprechen?");
							output.Response.ShouldEndSession = true;
							Debug.Write(MethodBase.GetCurrentMethod(), "zu doof zu sprechen");
							break;
					}
					break;
				default:
					output.Response.OutputSpeech = new PlainTextOutputSpeech("ei alter");
					Debug.Write(MethodBase.GetCurrentMethod(), $"irgendein Request: {input.Request.Type}");
					break;
			}
			return output;
		}
		private IOutputSpeech GetOutputSpeech(AlexaReturnType t, string msg) {
			IOutputSpeech returns;
			switch(t) {
				case AlexaReturnType.String:
					returns = new PlainTextOutputSpeech(msg);
					break;
				case AlexaReturnType.Ssml:
					returns = new SsmlOutputSpeech(msg);
					break;
				default:
					returns = defaultMsg;
					break;
			}
			return returns;
		}
		private async Task hitUrl(string ip, string cmd) {
			HttpClient client = new HttpClient();
			string url = $"http://{ip}/{cmd}";
			HttpResponseMessage response = await client.GetAsync(url);
			string responseBody = await response.Content.ReadAsStringAsync();
			Debug.Write(MethodBase.GetCurrentMethod(), $"{url}:\r\n\t{responseBody}");
		}
	}
}
