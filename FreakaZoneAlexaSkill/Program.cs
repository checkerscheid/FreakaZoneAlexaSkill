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
//# Revision     : $Rev:: 244                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: Program.cs 244 2025-06-28 15:02:16Z                      $ #
//#                                                                                 #
//###################################################################################
using FreakaZone.Libraries.wpEventLog;
using System.Reflection;

namespace FreakaZoneAlexaSkill {
	internal static class Program {
		public const string subversion = "242";
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Debug debug = new Debug("FreakaZoneAlexaSkill");
			Debug.Write(MethodInfo.GetCurrentMethod(), $"START v 1.0.{subversion}" +
				"\r\n####################################################################\r\n\r\n");
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			Application.Run(new AlexaSkill());
		}
	}
}