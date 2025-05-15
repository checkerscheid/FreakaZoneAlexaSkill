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
//# Revision     : $Rev:: 214                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: AlexaController.cs 214 2025-05-15 14:51:30Z              $ #
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

	/// <summary>
	/// Provides an API controller for handling Alexa skill requests and processing intents.
	/// </summary>
	/// <remarks>This controller is designed to handle various Alexa skill requests, such as intent requests and
	/// launch requests. It processes specific intents, including controlling lights, event lighting, and TVs, and provides
	/// appropriate responses based on the input. The controller also includes default error handling for unsupported or
	/// unrecognized requests.</remarks>
	[ApiController]
	[Route("[controller]")]
	public class AlexaController: Controller {
		const string INTENT_SLEEPNOW = "sleepnow";
		const string INTENT_RGBCCT = "rgbcct";
		const string INTENT_EVENTLIGHTING = "lichterkette";
		const string INTENT_FERNSEHER = "fernseher";

		private IOutputSpeech defaultMsg = new PlainTextOutputSpeech("Da ist was schief gelaufen");

		/// <summary>
		/// Processes an incoming Alexa skill request and generates an appropriate response.
		/// </summary>
		/// <remarks>This method handles various Alexa request types, such as "LaunchRequest", "IntentRequest", and
		/// others.  It processes intents like "SleepNow", "RGBCCT", "EventLighting", and "Fernseher", performing actions
		/// based on the intent and its parameters.  For example: - The "SleepNow" intent triggers specific lighting and
		/// device shutdown actions. - The "RGBCCT" intent adjusts lighting settings for a specified room. - The
		/// "EventLighting" intent configures event-specific lighting. - The "Fernseher" intent controls TV-related actions. 
		/// If an unrecognized request type or intent is received, a fallback response is generated.</remarks>
		/// <param name="input">The <see cref="SkillRequest"/> object representing the incoming request, including the request type and any
		/// associated data.</param>
		/// <returns>A <see cref="SkillResponse"/> object containing the response to be sent back to Alexa, including speech output,
		/// session state, and other response details.</returns>
		[HttpPost, Route("/process")]
		public SkillResponse Process(SkillRequest input) {
			SkillResponse output = new SkillResponse();

			Lichtleisten lichtleisten = new Lichtleisten();
			Eventbeleuchtungen eventbeleuchtungen = new Eventbeleuchtungen();
			Tvs tvs = new Tvs(false);

			lichtleisten.Init();
			eventbeleuchtungen.Init();

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
					Debug.Write(MethodBase.GetCurrentMethod(), $"New Intent: {input.Request}");
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

							Task.Run(async () => await HitUrl("172.17.80.169", "setNeoPixelColor?r=100&g=5&b=0"));
							Task.Run(async () => await HitUrl("172.17.80.164", "setCwWw?cw=10&ww=0"));
							int h = 0;
							int m = 30;
							int sec = (h * 60 * 60) + (m * 60);
							Task.Run(async () => await HitUrl("172.17.80.169", $"setNeoPixelSleep?sleep={sec}"));
							Task.Run(async () => await HitUrl("172.17.80.164", $"setCwWwSleep?sleep={sec}"));
							Task.Run(async () => await HitUrl("wpLicht:turner@172.17.80.163", "color/0?turn=off"));
							Task.Run(async () => await HitUrl("wpLicht:turner@172.17.80.160", "relay/0?turn=off"));
							Task.Run(async () => await HitUrl("wpLicht:turner@172.17.80.161", "relay/0?turn=off"));
							Task.Run(async () => await HitUrl("wpLicht:turner@172.17.80.162", "relay/0?turn=off"));
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
									dienst: ir.Intent.Slots?["dienst"]?.SlotValue?.Value);
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

		/// <summary>
		/// Creates an instance of <see cref="IOutputSpeech"/> based on the specified return type and message.
		/// </summary>
		/// <param name="t">The type of output speech to generate. Must be either <see cref="AlexaReturnType.String"/> or <see
		/// cref="AlexaReturnType.Ssml"/>.</param>
		/// <param name="msg">The message content to include in the output speech. Cannot be null or empty.</param>
		/// <returns>An instance of <see cref="IOutputSpeech"/> containing the specified message.  Returns a <see
		/// cref="PlainTextOutputSpeech"/> if <paramref name="t"/> is <see cref="AlexaReturnType.String"/>,  or a <see
		/// cref="SsmlOutputSpeech"/> if <paramref name="t"/> is <see cref="AlexaReturnType.Ssml"/>.  If the return type is
		/// unrecognized, a default output speech is returned.</returns>
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

		/// <summary>
		/// Sends an HTTP GET request to the specified IP address and command endpoint.
		/// </summary>
		/// <remarks>The method constructs a URL using the provided <paramref name="ip"/> and <paramref name="cmd"/>,
		/// sends an asynchronous GET request to the constructed URL, and logs the response content for debugging
		/// purposes.</remarks>
		/// <param name="ip">The IP address of the target server. Must be a valid IPv4 or IPv6 address.</param>
		/// <param name="cmd">The command to append to the URL path. This determines the specific endpoint being accessed.</param>
		/// <returns></returns>
		private async Task HitUrl(string ip, string cmd) {
			HttpClient client = new HttpClient();
			string url = $"http://{ip}/{cmd}";
			HttpResponseMessage response = await client.GetAsync(url);
			string responseBody = await response.Content.ReadAsStringAsync();
			Debug.Write(MethodBase.GetCurrentMethod(), $"{url}:\r\n\t{responseBody}");
		}
	}
}
