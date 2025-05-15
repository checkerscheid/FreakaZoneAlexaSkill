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
//# File-ID      : $Id:: Eventbeleuchtung.cs 214 2025-05-15 14:51:30Z             $ #
//#                                                                                 #
//###################################################################################
using FreakaZone.Libraries.wpCommon;
using FreakaZone.Libraries.wpEventLog;
using System.Reflection;

namespace FreakaZoneAlexaSkill.Data {

	/// <summary>
	/// Represents an event lighting device that can be controlled via network commands.
	/// </summary>
	/// <remarks>This class provides functionality to control the lighting device by sending commands over the
	/// network. It supports operations such as turning the lights on or off, adjusting brightness levels, and applying
	/// effects.</remarks>
	public class Eventbeleuchtung : IData {
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
		/// Initializes a new instance of the <see cref="Eventbeleuchtung"/> class with the specified name and IP address.
		/// </summary>
		/// <remarks>Use this constructor to create an instance of the <see cref="Eventbeleuchtung"/> class with a
		/// specific name and IP address. Ensure that the provided IP address is valid and reachable for proper
		/// operation.</remarks>
		/// <param name="Name">The name of the event lighting system. Cannot be null or empty.</param>
		/// <param name="Ip">The IP address of the event lighting system. Must be a valid IP address.</param>
		public Eventbeleuchtung(string Name, string Ip) {
			_name = Name;
			_ip = Ip;
		}

		/// <summary>
		/// Configures the lighting settings based on the specified parameters.
		/// </summary>
		/// <remarks>The method adjusts the lighting based on the provided parameters. It supports turning lights on
		/// or off, setting brightness levels,  and applying effects. The target area can be specified as "links" (left),
		/// "rechts" (right), or left unspecified to apply to all areas.</remarks>
		/// <param name="param">The parameters that define the lighting configuration, including on/off state, brightness percentage, and target
		/// area.</param>
		/// <param name="returnmsg">An output parameter that contains a message describing the result of the operation.</param>
		/// <returns>An <see cref="AlexaReturnType"/> value indicating the result of the operation.  Returns <see
		/// cref="AlexaReturnType.String"/> if the operation is successful, or <see cref="AlexaReturnType.Error"/> if an error
		/// occurs.</returns>
		private AlexaReturnType Set(EventbeleuchtungParams param, out string returnmsg) {
			AlexaReturnType returns = AlexaReturnType.Error;
			returnmsg = "Da ist was schief gelaufen";

			string target = param.linksrechts?? "";
			if(param.einaus != null) {
				switch(param.einaus) {
					case "ein":
					case "an":
						if(target == "") _ = HitUrl("setCwWw?ww=50&cw=50");
						if(target == "links") _ = HitUrl("setCwWw?ww=50");
						if(target == "rechts") _ = HitUrl("setCwWw?cw=50");
						returnmsg = $"Joo, {_name} {target} is an gemacht";
						returns = AlexaReturnType.String;
						break;
					case "aus":
						if(target == "") _ = HitUrl("setCwWw?ww=0&cw=0");
						if(target == "links") _ = HitUrl("setCwWw?ww=0");
						if(target == "rechts") _ = HitUrl("setCwWw?cw=0");
						returnmsg = $"Joo, {_name} {target} is aus gemacht";
						returns = AlexaReturnType.String;
						break;
				}
			}
			if(param.prozent != null) {
				int p;
				if(Int32.TryParse(param.prozent, out p)) {
					if(target == "")
						_ = HitUrl($"setCwWw?ww={p}&cw={p}");
					if(target == "links")
						_ = HitUrl($"setCwWw?ww={p}");
					if(target == "rechts")
						_ = HitUrl($"setCwWw?cw={p}");
					returnmsg = $"Joo, {_name} {target} {p} prozent is gemacht";
					returns = AlexaReturnType.String;
				}
			}
			if(param.einaus == null && param.prozent == null) {
				if(target == "")
					_ = HitUrl($"setCwWwEffect?effect=4");
				if(target == "links")
					_ = HitUrl($"setCwWwEffect?effect=5");
				if(target == "rechts")
					_ = HitUrl($"setCwWwEffect?effect=6");
				returnmsg = $"Joo, {_name} {target} effect is gemacht";
				returns = AlexaReturnType.String;
			}
			return returns;
		}

		/// <summary>
		/// Sets the specified parameters for the current instance and returns a result indicating the outcome.
		/// </summary>
		/// <param name="param">The parameters to be set. Must be an instance of a supported parameter type.</param>
		/// <param name="returnmsg">An output parameter that contains a message describing the result of the operation.  If the operation is
		/// successful, this will contain a success message; otherwise, it will contain an error message.</param>
		/// <returns>An <see cref="AlexaReturnType"/> value indicating the result of the operation.  Returns <see
		/// cref="AlexaReturnType.String"/> if the parameter type is invalid.</returns>
		public AlexaReturnType Set(IParams param, out string returnmsg) {
			if(param.GetType() == typeof(EventbeleuchtungParams)) {
				return Set((EventbeleuchtungParams)param, out returnmsg);
			}
			returnmsg = $"{_name} hat einen falschen Parameter";
			return AlexaReturnType.String;
		}

		/// <summary>
		/// Sends an HTTP GET request to a specified command endpoint on the configured IP address.
		/// </summary>
		/// <remarks>The method constructs a URL using the configured IP address and the provided command,  sends an
		/// asynchronous GET request to the constructed URL, and logs the response.</remarks>
		/// <param name="cmd">The command to append to the base URL for the request. Must not be null or empty.</param>
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
	/// Represents a collection of event lighting devices, providing methods to initialize and retrieve devices by name.
	/// </summary>
	/// <remarks>This class manages a list of <see cref="Eventbeleuchtung"/> objects, which represent individual
	/// lighting devices. It provides functionality to initialize the collection with predefined devices and to retrieve a
	/// device by its name.</remarks>
	public class Eventbeleuchtungen: IList {
		private List<Eventbeleuchtung> eventbeleuchtungen;

		/// <summary>
		/// Initializes a new instance of the <see cref="Eventbeleuchtungen"/> class.
		/// </summary>
		/// <remarks>This constructor initializes the <see cref="Eventbeleuchtungen"/> instance with an empty list of
		/// <see cref="Eventbeleuchtung"/> objects.</remarks>
		public Eventbeleuchtungen() {
			eventbeleuchtungen = new List<Eventbeleuchtung>();
		}

		/// <summary>
		/// Initializes the collection of event lighting configurations with predefined settings.
		/// </summary>
		/// <remarks>This method populates the internal collection with a set of predefined lighting configurations, 
		/// each associated with a specific location and IP address. It is intended to be called during the  setup phase to
		/// ensure the collection is ready for use.</remarks>
		public void Init() {
			eventbeleuchtungen.Add(new Eventbeleuchtung("wohnzimmer", "172.17.80.97"));
			eventbeleuchtungen.Add(new Eventbeleuchtung("küche", "172.17.80.142"));
			eventbeleuchtungen.Add(new Eventbeleuchtung("kinderzimmer", "172.17.80.164"));
			eventbeleuchtungen.Add(new Eventbeleuchtung("pia", "172.17.80.164"));
		}

		/// <summary>
		/// Retrieves an <see cref="IData"/> instance that matches the specified name.
		/// </summary>
		/// <param name="name">The name of the device to retrieve. The comparison is case-insensitive. Can be <see langword="null"/>.</param>
		/// <returns>An <see cref="IData"/> instance that matches the specified name, or a default instance with the name "noDevice" if
		/// no match is found.</returns>
		public IData Get(string? name) {
			return eventbeleuchtungen.Find(eb => eb.name == name?.ToLower()) ?? new Eventbeleuchtung("noDevice", "");
		}
	}

	/// <summary>
	/// Represents the parameters for configuring event lighting settings.
	/// </summary>
	/// <remarks>This class encapsulates the configuration options for event lighting, including the on/off state,
	/// brightness level, and directional settings. It is used to pass these parameters to methods or systems that control
	/// event lighting.</remarks>
	public class EventbeleuchtungParams : IParams {
		private string? _einaus;
		public string? einaus {
			get { return _einaus; }
			set { _einaus = value;  }
		}
		private string? _prozent;
		public string? prozent {
			get { return _prozent; }
			set { _prozent = value; }
		}
		private string? _linksrechts;
		public string? linksrechts {
			get {  return _linksrechts; }
			set { _linksrechts = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventbeleuchtungParams"/> class with the specified lighting
		/// parameters.
		/// </summary>
		/// <param name="einaus">Specifies whether the lighting is on or off. Can be <see langword="null"/> if not specified.</param>
		/// <param name="prozent">The brightness level of the lighting as a percentage. Can be <see langword="null"/> if not specified.</param>
		/// <param name="linksrechts">Specifies the direction of the lighting (e.g., left or right). Can be <see langword="null"/> if not specified.</param>
		public EventbeleuchtungParams(string? einaus, string? prozent, string? linksrechts) {
			_einaus = einaus;
			_prozent = prozent;
			_linksrechts = linksrechts;
		}

		/// <summary>
		/// Returns a string representation of the current object, including its key properties.
		/// </summary>
		/// <returns>A string that represents the current object, including the values of <c>_einaus</c>, <c>_prozent</c>, and
		/// <c>_linksrechts</c>.</returns>
		public override string ToString() {
			return $"einaus: {_einaus}, prozent: {_prozent}, linksrechts: {_linksrechts}";
		}
	}
}