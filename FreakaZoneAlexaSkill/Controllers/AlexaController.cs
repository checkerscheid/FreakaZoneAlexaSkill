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
//# File-ID      : $Id:: AlexaController.cs 146 2024-12-07 12:43:11Z              $ #
//#                                                                                 #
//###################################################################################
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using FreakaZoneAlexaSkill.Data;
using FreakaZoneAlexaSkill.Src;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
			if(!Directory.Exists("Log")) Directory.CreateDirectory("Log");
			TextWriterTraceListener twtl = new TextWriterTraceListener($"\\Log\\FreakaZoneAlexaSkill.log");
			Trace.Listeners.Add(twtl);

			SkillResponse output = new SkillResponse();

			Lichtleisten lichtleisten = new Lichtleisten();
			Eventbeleuchtungen eventbeleuchtungen = new Eventbeleuchtungen();
			Tvs tvs = new Tvs();

			lichtleisten.init();
			eventbeleuchtungen.init();
			tvs.init();

			output.Version = "1.0";
			output.Response = new ResponseBody();

			Logger.Write(MethodBase.GetCurrentMethod(), $"New Request: {input.Request.Type} detected");

			switch(input.Request.Type) {
				case "LaunchRequest":
					output.Response.OutputSpeech = new SsmlOutputSpeech("<speak><amazon:emotion name=\"excited\" intensity=\"high\">waaas</amazon:emotion></speak>");
					output.Response.ShouldEndSession = false;
					Logger.Write(MethodBase.GetCurrentMethod(), $"LaunchRequest detected");
					break;
				case "IntentRequest":
					IntentRequest ir = (IntentRequest)input.Request;
					string? roomname;
					Lichtleiste lichtleiste;
					Eventbeleuchtung eventbeleuchtung;
					Tv tv;
					bool returns;
					IOutputSpeech returnmsg = defaultMsg;
					switch(ir.Intent.Name) {
						case INTENT_SLEEPNOW:
							Logger.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_SLEEPNOW} detected");
							output.Response.OutputSpeech = new SsmlOutputSpeech("<speak><amazon:emotion name=\"disappointed\" intensity=\"high\">ooooh pia!</amazon:emotion><amazon:effect name=\"whispered\">schlaf gut süße maus</amazon:effect></speak>");
							_ = hitUrl("172.17.80.169", "setNeoPixelBrightness?brightness=15");
							_ = hitUrl("172.17.80.169", "setNeoPixelColor?r=254&g=34&b=0");
							_ = hitUrl("172.17.80.164", "setCwWw?cw=10&ww=0");
							int h = 0;
							int m = 30;
							int sec = (h * 60 * 60) + (m * 60);
							_ = hitUrl("172.17.80.169", $"setNeoPixelSleep?sleep={sec}");
							_ = hitUrl("172.17.80.164", $"setCwWwSleep?sleep={sec}");
							_ = hitUrl("wpLicht:turner@172.17.80.163", "color/0?turn=off");
							_ = hitUrl("wpLicht:turner@172.17.80.160", "relay/0?turn=off");
							_ = hitUrl("wpLicht:turner@172.17.80.161", "relay/0?turn=off");
							_ = hitUrl("wpLicht:turner@172.17.80.162", "relay/0?turn=off");
							output.Response.ShouldEndSession = true;
							Logger.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_SLEEPNOW} finished");
							break;
						case INTENT_RGBCCT:
							Logger.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_RGBCCT} detected");
							roomname = ir.Intent.Slots?["lichtleiste"]?.SlotValue?.Value;
							lichtleiste = (Lichtleiste)lichtleisten.Get(roomname);
							if(lichtleiste.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"Lichtleiste: '{roomname}' nicht gefunden");
							} else {
								LichtleisteParams llp = new LichtleisteParams(
									einaus: ir.Intent.Slots?["einaus"]?.SlotValue?.Value,
									prozent: ir.Intent.Slots?["prozent"]?.SlotValue?.Value);
								returns = lichtleiste.Set(llp, out returnmsg);
								if(returns) {
									output.Response.OutputSpeech = returnmsg;
								} else {
									output.Response.OutputSpeech = defaultMsg;
								}
								Logger.Write(MethodBase.GetCurrentMethod(), llp.ToString());
							}
							output.Response.ShouldEndSession = true;
							Logger.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_RGBCCT} finished");
							break;
						case INTENT_EVENTLIGHTING:
							Logger.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_EVENTLIGHTING} detected");
							roomname = ir.Intent.Slots?["beleuchtung"]?.SlotValue?.Value;
							eventbeleuchtung = (Eventbeleuchtung)eventbeleuchtungen.Get(roomname);
							if(eventbeleuchtung.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"Eventbeleuchtung: '{roomname}' nicht gefunden");
							} else {
								EventbeleuchtungParams ebp = new EventbeleuchtungParams(
									einaus: ir.Intent.Slots?["einaus"]?.SlotValue?.Value,
									prozent: ir.Intent.Slots?["prozent"]?.SlotValue?.Value,
									linksrechts: ir.Intent.Slots?["linksrechts"]?.SlotValue?.Value);
								returns = eventbeleuchtung.Set(ebp, out returnmsg);
								if(returns) {
									output.Response.OutputSpeech = returnmsg;
								} else {
									output.Response.OutputSpeech = defaultMsg;
								}
								Logger.Write(MethodBase.GetCurrentMethod(), ebp.ToString());
							}
							output.Response.ShouldEndSession = true;
							Logger.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_EVENTLIGHTING} finished");
							break;
						case INTENT_FERNSEHER:
							Logger.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_FERNSEHER} detected");
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = (Tv)tvs.Get(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								TVParams tvp = new TVParams(
									einaus: ir.Intent.Slots?["einaus"]?.SlotValue?.Value,
									tvbutton: ir.Intent.Slots?["tvbutton"]?.SlotValue?.Value,
									dienst: ir.Intent.Slots?["dienst"]?.SlotValue?.Value,
									richtung: ir.Intent.Slots?["richtung"]?.SlotValue?.Value);
								returns = tv.Set(tvp, out returnmsg);
								if(returns) {
									output.Response.OutputSpeech = returnmsg;
								} else {
									output.Response.OutputSpeech = defaultMsg;
								}
								Logger.Write(MethodBase.GetCurrentMethod(), tvp.ToString());
							}
							output.Response.ShouldEndSession = true;
							Logger.Write(MethodBase.GetCurrentMethod(), $"Intent {INTENT_FERNSEHER} finished");
							break;

						// #####################################################
/* old stuff
						#region tv

						case "startNetflix":
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = tvs.GetTv(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{tv.name} Netflix wird gestartet");
								SamsungTv samsungTv = new SamsungTv(tv);
								_ = samsungTv.SimulateNetflix();
								Logger.Write(MethodBase.GetCurrentMethod(), $"{tv.name} Netflix wird gestartet");
							}
							output.Response.ShouldEndSession = true;
							break;
						case "startDisney":
							Logger.Write(MethodBase.GetCurrentMethod(), $"startDisney detected");
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = tvs.GetTv(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{tv.name} Disney wird gestartet");
								SamsungTv samsungTv = new SamsungTv(tv);
								_ = samsungTv.SimulateDisney();
								Logger.Write(MethodBase.GetCurrentMethod(), $"{tv.name} Disney wird gestartet");
							}
							output.Response.ShouldEndSession = true;
							break;
						case "startYouTube":
							Logger.Write(MethodBase.GetCurrentMethod(), $"startYouTube detected");
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = tvs.GetTv(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{tv.name} YouTube wird gestartet");
								SamsungTv samsungTv = new SamsungTv(tv);
								_ = samsungTv.SimulateYouTube();
								Logger.Write(MethodBase.GetCurrentMethod(), $"{tv.name} YouTube wird gestartet");
							}
							output.Response.ShouldEndSession = true;
							break;
						case "tvlauter":
							Logger.Write(MethodBase.GetCurrentMethod(), $"tvlauter detected");
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = tvs.GetTv(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{tv.name} ist lauter");
								SamsungTv samsungTv = new SamsungTv(tv);
								_ = samsungTv.SimulateVolumeUp();
								Logger.Write(MethodBase.GetCurrentMethod(), $"{tv.name} wird lauter");
							}
							output.Response.ShouldEndSession = true;
							break;
						case "tvleiser":
							Logger.Write(MethodBase.GetCurrentMethod(), $"tvleiser detected");
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = tvs.GetTv(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{tv.name} ist leiser");
								SamsungTv samsungTv = new SamsungTv(tv);
								_ = samsungTv.SimulateVolumeDown();
								Logger.Write(MethodBase.GetCurrentMethod(), $"{tv.name} wird leiser");
							}
							output.Response.ShouldEndSession = true;
							break;
						case "tventer":
							Logger.Write(MethodBase.GetCurrentMethod(), $"tventer detected");
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = tvs.GetTv(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{tv.name} OK");
								SamsungTv samsungTv = new SamsungTv(tv);
								_ =	samsungTv.SimulateOK();
								Logger.Write(MethodBase.GetCurrentMethod(), $"{tv.name} OK");
							}
							output.Response.ShouldEndSession = true;
							break;
						case "tvreturn":
							Logger.Write(MethodBase.GetCurrentMethod(), $"tvreturn detected");
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = tvs.GetTv(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{tv.name} zurück");
								SamsungTv samsungTv = new SamsungTv(tv);
								_ = samsungTv.SimulateReturn();
								Logger.Write(MethodBase.GetCurrentMethod(), $"{tv.name} Zurück");
							}
							output.Response.ShouldEndSession = true;
							break;
						case "tvmoveup":
							Logger.Write(MethodBase.GetCurrentMethod(), $"tvmoveup detected");
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = tvs.GetTv(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{tv.name} hoch");
								SamsungTv samsungTv = new SamsungTv(tv);
								_ = samsungTv.SimulateDirection(Direction.UP);
								Logger.Write(MethodBase.GetCurrentMethod(), $"{tv.name} Hoch");
							}
							output.Response.ShouldEndSession = true;
							break;
						case "tvmoveright":
							Logger.Write(MethodBase.GetCurrentMethod(), $"tvmoveright detected");
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = tvs.GetTv(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{tv.name} rechts");
								SamsungTv samsungTv = new SamsungTv(tv);
								_ = samsungTv.SimulateDirection(Direction.RIGHT);
								Logger.Write(MethodBase.GetCurrentMethod(), $"{tv.name} Rechts");
							}
							output.Response.ShouldEndSession = true;
							break;
						case "tvmovedown":
							Logger.Write(MethodBase.GetCurrentMethod(), $"tvmovedown detected");
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = tvs.GetTv(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{tv.name} runter");
								SamsungTv samsungTv = new SamsungTv(tv);
								_ = samsungTv.SimulateDirection(Direction.DOWN);
								Logger.Write(MethodBase.GetCurrentMethod(), $"{tv.name} Runter");
							}
							output.Response.ShouldEndSession = true;
							break;
						case "tvmoveleft":
							Logger.Write(MethodBase.GetCurrentMethod(), $"tvmoveleft detected");
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = tvs.GetTv(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{tv.name} links");
								SamsungTv samsungTv = new SamsungTv(tv);
								_ = samsungTv.SimulateDirection(Direction.LEFT);
								Logger.Write(MethodBase.GetCurrentMethod(), $"{tv.name} Links");
							}
							output.Response.ShouldEndSession = true;
							break;
						case "tvoff":
							Logger.Write(MethodBase.GetCurrentMethod(), $"tvoff detected");
							roomname = ir.Intent.Slots?["tv"]?.SlotValue?.Value;
							tv = tvs.GetTv(roomname);
							if(tv.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{tv.name} wird ausgeschaltet");
								SamsungTv samsungTv = new SamsungTv(tv);
								_ = samsungTv.SimulateOff();
								Logger.Write(MethodBase.GetCurrentMethod(), $"{tv.name} wird ausgeschaltet");
							}
							output.Response.ShouldEndSession = true;
							break;

						#endregion

						#region lichtleiste

						case "setOn":
							Logger.Write(MethodBase.GetCurrentMethod(), $"setOn detected");
							roomname = ir.Intent.Slots?["lichtleiste"]?.SlotValue?.Value;
							ll = lichtleisten.GetLichtleiste(roomname);
							if(ll.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"Lichtleiste an: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"Joo, {ll.name} an is gemacht");
								_ = hitUrl(ll.ip, "setNeoPixelOn");
								Logger.Write(MethodBase.GetCurrentMethod(), $"Lichtleiste {ll.name} an");
							}
							output.Response.ShouldEndSession = true;
							break;
						case "setOff":
							Logger.Write(MethodBase.GetCurrentMethod(), $"setOff detected");
							roomname = ir.Intent.Slots?["lichtleiste"]?.SlotValue?.Value;
							ll = lichtleisten.GetLichtleiste(roomname);
							if(ll.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"Lichtleiste an: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"Joo, {ll.name} aus is gemacht");
								_ = hitUrl(ll.ip, "setNeoPixelOff");
								Logger.Write(MethodBase.GetCurrentMethod(), $"Lichtleiste {ll.name} aus");
							}
							output.Response.ShouldEndSession = true;
							break;
						case "rainbow":
							Logger.Write(MethodBase.GetCurrentMethod(), $"rainbow detected");
							roomname = ir.Intent.Slots?["lichtleiste"]?.SlotValue?.Value;
							ll = lichtleisten.GetLichtleiste(roomname);
							if(ll.name == "noDevice") {
								output.Response.OutputSpeech = new PlainTextOutputSpeech($"{roomname} gibts nicht");
								Logger.Write(MethodBase.GetCurrentMethod(), $"Lichtleiste genervter Effect: '{roomname}' nicht gefunden");
							} else {
								output.Response.OutputSpeech = new SsmlOutputSpeech($"<speak><amazon:emotion name=\"disappointed\" intensity=\"high\">Joo, {ll.name} rainbow is gemacht</amazon:emotion><amazon:effect name=\"whispered\">aber bitte schlag mich nicht schon wieder</amazon:effect></speak>");
								_ = hitUrl(ll.ip, "setNeoPixelEffect?effect=3");
								Logger.Write(MethodBase.GetCurrentMethod(), $"Lichtleiste genervter Effect: '{ll.name}'");
							}
							output.Response.ShouldEndSession = true;
							break;

						#endregion
*/

						case "AMAZON.FallbackIntent":
							output.Response.OutputSpeech = new PlainTextOutputSpeech("zu doof zu sprechen?");
							output.Response.ShouldEndSession = true;
							Logger.Write(MethodBase.GetCurrentMethod(), "zu doof zu sprechen");
							break;
					}
					break;
				default:
					output.Response.OutputSpeech = new PlainTextOutputSpeech("ei alter");
					Logger.Write(MethodBase.GetCurrentMethod(), $"irgendein Request: {input.Request.Type}");
					break;
			}
			return output;
		}
		private async Task hitUrl(string ip, string cmd) {
			//Task.Run(() => {
			HttpClient client = new HttpClient();
			string url = $"http://{ip}/{cmd}";
			HttpResponseMessage response = await client.GetAsync(url);
			string responseBody = await response.Content.ReadAsStringAsync();
			Logger.Write(MethodBase.GetCurrentMethod(), $"{url}: {responseBody}");
			//});
		}
	}
}
