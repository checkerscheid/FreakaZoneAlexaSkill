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
//# File-ID      : $Id:: IData.cs 187 2025-02-17 00:57:15Z                        $ #
//#                                                                                 #
//###################################################################################
using FreakaZone.Libraries.wpCommon;

namespace FreakaZoneAlexaSkill.Data {
	public interface IData {
		public string name { get; set; }
		public string ip { get; set; }
		public AlexaReturnType Set(IParams param, out string msg);
	}
	public interface IList {
		public void init();
		public IData Get(string? name);
	}
	public interface IParams {
		public string ToString();
	}
}
