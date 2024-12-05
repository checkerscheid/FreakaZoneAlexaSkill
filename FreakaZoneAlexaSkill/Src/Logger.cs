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
//# Revision     : $Rev:: 145                                                     $ #
//# Author       : $Author::                                                      $ #
//# File-ID      : $Id:: Logger.cs 145 2024-12-05 19:12:44Z                       $ #
//#                                                                                 #
//###################################################################################
using System.Diagnostics;
using System.Reflection;

namespace FreakaZoneAlexaSkill.Src {
	public static class Logger {
		public const string ErrorString = "Message: {0}\r\nTrace:\r\n{1}";
		public static void Write(MethodBase? mb, string msg) {
			int l = 20;
			String n = mb?.Name ?? "";
			if(n.Length > l) {
				n = n.Substring(n.Length - l, l);
			}
			string dmsg = String.Format("{0:dd.MM.yy HH:mm:ss.fff} [{1}] - {2}", DateTime.Now, n.PadRight(l), msg);
			Debug.WriteLine(dmsg);
			Console.WriteLine(dmsg);
		}
		public static void Write(MethodBase? mb, string msg, params string[] args) {
			Write(mb, String.Format(msg, args));
		}
		public static void WriteError(MethodBase? mb, Exception ex, params string[] obj) {
			string additional = "";
			for(int i = 0; i < obj.Length; i++) {
				additional += "'" + obj[i] + "'";
				if(i < obj.Length - 1)
					additional += ", ";
			}
			string newErrorString = Logger.ErrorString + "\r\n\r\n{2}";
			Write(mb, newErrorString, ex.Message, ex.StackTrace ?? "StackTrace.Empty", additional);
		}
	}
}
