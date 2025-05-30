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
//# Revision     : $Rev:: 236                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: Lichtleiste.cs 236 2025-05-30 11:21:55Z                  $ #
//#                                                                                 #
//###################################################################################
using FreakaZone.Libraries.wpCommon;
using FreakaZone.Libraries.wpEventLog;
using System.Reflection;

namespace FreakaZoneAlexaSkill.Data {

	/// <summary>
	/// Represents a smart light strip that can be controlled programmatically.
	/// </summary>
	/// <remarks>The <see cref="Lichtleiste"/> class provides functionality to control a smart light strip,  including
	/// turning it on or off, adjusting brightness, and setting specific effects or modes. Instances of this class are
	/// initialized with a name and an IP address, which are used to identify and communicate with the light
	/// strip.</remarks>
	public class Lichtleiste: IData {
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

		/// <summary>
		/// Initializes a new instance of the <see cref="Lichtleiste"/> class with the specified name and IP address.
		/// </summary>
		/// <remarks>Use this constructor to create a new light strip instance with a specific name and IP address.
		/// Ensure that the provided IP address is reachable and correctly formatted.</remarks>
		/// <param name="Name">The name of the light strip. This value cannot be null or empty.</param>
		/// <param name="Ip">The IP address of the light strip. This value must be a valid IP address.</param>
		public Lichtleiste(string Name, string Ip) {
			_name = Name;
			_ip = Ip;
		}

		/// <summary>
		/// Configures the lighting system based on the specified parameters and returns a status message.
		/// </summary>
		/// <remarks>The method processes various lighting commands, including turning the lights on or off, setting
		/// specific effects,  or adjusting brightness levels. If both <c>param.einaus</c> and <c>param.prozent</c> are null,
		/// a default effect may be applied.</remarks>
		/// <param name="param">An object containing the parameters for the lighting configuration, such as on/off state or brightness level.</param>
		/// <param name="returnmsg">An output parameter that contains a message describing the result of the operation.  This message may include
		/// plain text or SSML (Speech Synthesis Markup Language) for Alexa responses.</param>
		/// <returns>An <see cref="AlexaReturnType"/> value indicating the type of response generated, such as a plain string or SSML.</returns>
		private AlexaReturnType Set(LichtleisteParams param, out string returnmsg) {
			AlexaReturnType returns = AlexaReturnType.Error;
			returnmsg = "Da ist was schief gelaufen";
			if(param.einaus == null && param.prozent == null) {
				// case "rainbow":
				_ = HitUrl("setNeoPixel?effect=3");
				returnmsg = $"<speak><amazon:emotion name=\"disappointed\" intensity=\"high\">Joo, {_name} rainbow is gemacht</amazon:emotion><amazon:effect name=\"whispered\">aber bitte schlag mich nicht schon wieder</amazon:effect></speak>";
				returns = AlexaReturnType.Ssml;
				//	break;
				//case "rainbow wheel":
				//	returnmsg = new SsmlOutputSpeech($"<speak><amazon:emotion name=\"disappointed\" intensity=\"high\">Joo, {_name} rainbow wheel is gemacht</amazon:emotion><amazon:effect name=\"whispered\">aber bitte schlag mich nicht schon wieder</amazon:effect></speak>");
				//	_ = hitUrl("setNeoPixel?effect=4");
				//	returns = true;
				//	break;
			}
			if(param.einaus != null) {
				switch(param.einaus) {
					case "ein":
					case "an":
						_ = HitUrl("setNeoPixel?turn=1");
						returnmsg = $"Joo, {_name} is an gemacht";
						returns = AlexaReturnType.String;
						break;
					case "aus":
						_ = HitUrl("setNeoPixel?turn=0");
						returnmsg = $"Joo, {_name} is aus gemacht";
						returns = AlexaReturnType.String;
						break;
					case "arztzimmer":
						_ = HitUrl("setNeoPixel?r=0&g=0&b=0&ww=0&cw=75");
						returnmsg = $"<speak><amazon:emotion name=\"disappointed\" intensity=\"low\">Joo, {_name} arztzimmer is gemacht</amazon:emotion></speak>";
						returns = AlexaReturnType.Ssml;
						break;
					case "sonnenschein":
					case "sonne":
						_ = HitUrl("setNeoPixel?r=0&g=0&b=0&ww=75&cw=25");
						returnmsg = $"<speak><amazon:emotion name=\"disappointed\" intensity=\"low\">Joo, {_name} sonnenschein is gemacht</amazon:emotion></speak>";
						returns = AlexaReturnType.Ssml;
						break;
					case "gemütlich":
						_ = HitUrl("setNeoPixel?r=0&g=0&b=0&ww=50&cw=5");
						returnmsg = $"<speak>Psst, <amazon:effect name=\"whispered\">{_name} gemütlich is gemacht</amazon:effect></speak>";
						returns = AlexaReturnType.Ssml;
						break;
				}
			}
			if(param.prozent != null) {
				int p;
				if(Int32.TryParse(param.prozent, out p)) {
					if(p > 100)
						p = 100;
					if(p < 0)
						p = 0;
					_ = HitUrl($"setNeoPixel?brightness={p * 2.55}");
					returnmsg = $"Joo, {_name} {p} prozent is gemacht";
					returns = AlexaReturnType.String;
				}
			}

			return returns;
		}

		/// <summary>
		/// Sets the state of the device based on the provided parameters.
		/// </summary>
		/// <param name="param">The parameters used to configure the device. Must be of a supported type.</param>
		/// <param name="returnmsg">An output message describing the result of the operation. This will contain an error message if the parameter type
		/// is unsupported.</param>
		/// <returns>An <see cref="AlexaReturnType"/> value indicating the result of the operation. Returns <see
		/// cref="AlexaReturnType.String"/> if the parameter type is invalid.</returns>
		public AlexaReturnType Set(IParams param, out string returnmsg) {
			if(param.GetType() == typeof(LichtleisteParams)) {
				return Set((LichtleisteParams)param, out returnmsg);
			}
			returnmsg = $"{_name} hat einen falschen Parameter";
			return AlexaReturnType.String;
		}

		/// <summary>
		/// Sends an HTTP GET request to a specified command endpoint on the configured IP address.
		/// </summary>
		/// <remarks>The method constructs a URL using the configured IP address and the provided command,  sends an
		/// HTTP GET request to the constructed URL, and logs the response.  Ensure that the <paramref name="cmd"/> parameter
		/// is a valid and URL-safe string.</remarks>
		/// <param name="cmd">The command to append to the base URL for the request.</param>
		/// <returns></returns>
		private async Task HitUrl(string cmd) {
			HttpClient client = new HttpClient();
			string url = $"http://{_ip}/{cmd}";
			HttpResponseMessage response = await client.GetAsync(url);
			string responseBody = await response.Content.ReadAsStringAsync();
			Debug.Write(MethodBase.GetCurrentMethod(), $"{url}: {responseBody}");
		}
	}

	/// <summary>
	/// Represents a collection of <see cref="Lichtleiste"/> objects, providing methods to initialize and retrieve them by
	/// name.
	/// </summary>
	/// <remarks>The <see cref="Lichtleisten"/> class manages a list of <see cref="Lichtleiste"/> instances, each
	/// representing a specific device. It provides functionality to initialize the collection with predefined devices and
	/// retrieve a device by its name.</remarks>
	public class Lichtleisten: IList {
		private List<Lichtleiste> lichtleisten;

		/// <summary>
		/// Initializes a new instance of the <see cref="Lichtleisten"/> class.
		/// </summary>
		/// <remarks>This constructor creates an empty collection of <see cref="Lichtleiste"/> objects.</remarks>
		public Lichtleisten() {
			lichtleisten = new List<Lichtleiste>();
		}

		/// <summary>
		/// Initializes the collection of light strips with predefined configurations.
		/// </summary>
		/// <remarks>This method populates the internal collection of light strips with a set of predefined  names and
		/// IP addresses. Each light strip is represented by a <see cref="Lichtleiste"/>  object. This method should be called
		/// before attempting to access or manipulate the  light strips in the collection.</remarks>
		public void Init() {
			lichtleisten.Add(new Lichtleiste("wohnzimmer", "172.17.80.99"));
			lichtleisten.Add(new Lichtleiste("lautsprecher", "172.17.80.98"));
			lichtleisten.Add(new Lichtleiste("pflanze", "172.17.80.106"));
			lichtleisten.Add(new Lichtleiste("büro", "172.17.80.122"));
			lichtleisten.Add(new Lichtleiste("flur", "172.17.80.122"));
			lichtleisten.Add(new Lichtleiste("kinderzimmer", "172.17.80.169"));
			lichtleisten.Add(new Lichtleiste("pia", "172.17.80.169"));
		}

		/// <summary>
		/// Retrieves an <see cref="IData"/> object with the specified name.
		/// </summary>
		/// <param name="name">The name of the object to retrieve. The comparison is case-insensitive. Can be <see langword="null"/>.</param>
		/// <returns>The <see cref="IData"/> object with the specified name if found; otherwise, a default <see cref="Lichtleiste"/>
		/// object with the name "noDevice".</returns>
		public IData Get(string? name) {
			return lichtleisten.Find(ll => ll.name == name?.ToLower()) ?? new Lichtleiste("noDevice", "");
		}
	}

	/// <summary>
	/// Represents the parameters for controlling a light strip, including its on/off state and brightness level.
	/// </summary>
	/// <remarks>This class encapsulates the state and configuration of a light strip, providing properties to
	/// access its on/off state and brightness percentage. Instances of this class are immutable after
	/// construction.</remarks>
	public class LichtleisteParams: IParams {
		private string? _einaus;
		public string? einaus { get { return _einaus; } }
		private string? _prozent;
		public string? prozent { get { return _prozent; } }

		/// <summary>
		/// Initializes a new instance of the <see cref="LichtleisteParams"/> class with the specified on/off state and
		/// brightness percentage.
		/// </summary>
		/// <param name="einaus">The on/off state of the light strip. This can be a string representation of the state, such as "on" or "off".</param>
		/// <param name="prozent">The brightness level of the light strip as a percentage. This can be a string representation of the percentage,
		/// such as "50".</param>
		public LichtleisteParams(string? einaus, string? prozent) {
			_einaus = einaus;
			_prozent = prozent;
		}

		/// <summary>
		/// Returns a string representation of the current object, including the values of the  <c>_einaus</c> and
		/// <c>_prozent</c> fields.
		/// </summary>
		/// <returns>A string in the format "einaus: {value}, prozent: {value}", where the placeholders  are replaced with the
		/// respective values of the <c>_einaus</c> and <c>_prozent</c> fields.</returns>
		public override string ToString() {
			return $"einaus: {_einaus}, prozent: {_prozent}";
		}
	}
}