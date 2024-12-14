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

namespace FreakaZoneAlexaSkill.Data {
	public interface IData {
		public string name { get; set; }
		public string ip { get; set; }
		public bool Set(IParams param, out IOutputSpeech returnmsg);
	}
	public interface IList {
		public void init();
		public IData Get(string? name);
	}
	public interface IParams {
		public string ToString();
	}
}
