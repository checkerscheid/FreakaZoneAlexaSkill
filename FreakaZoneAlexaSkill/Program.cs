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
//# Revision     : $Rev:: 249                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: Program.cs 249 2025-07-30 22:16:11Z                      $ #
//#                                                                                 #
//###################################################################################
using FreakaZone.Libraries.wpEventLog;
using System.Reflection;

namespace FreakaZoneAlexaSkill {
	internal static class Program {
		public const string subversion = "248";
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Debug debug = new Debug("FreakaZoneAlexaSkill");
			string[] pVersion = Application.ProductVersion.Split('.');
			Debug.Write(MethodInfo.GetCurrentMethod(), $"START {Application.CompanyName} - {Application.ProductName} V {pVersion[0]}.{pVersion[1]}.{subversion}" +
				"\r\n####################################################################\r\n");
			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			Application.Run(new AlexaSkill());
			Debug.Write(MethodInfo.GetCurrentMethod(), "Programm finished\r\n\r\n");
			if(System.Diagnostics.Trace.Listeners != null) {
				System.Diagnostics.Trace.Listeners.Clear();
			}
		}
	}
}