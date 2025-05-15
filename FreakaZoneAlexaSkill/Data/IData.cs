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
//# File-ID      : $Id:: IData.cs 214 2025-05-15 14:51:30Z                        $ #
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
		public void Init();
		public IData Get(string? name);
	}
	public interface IParams {
		public string ToString();
	}
}
