using FreakaZoneAlexaSkill.Data;
using FreakaZoneAlexaSkill.Src;
using System.Reflection;

namespace TestClient {
	public partial class Form1: Form {
		private Tvs tvs;
		private Tv tv;
		public Form1() {
			InitializeComponent();
			tvs = new Tvs();
			tvs.init();
		}

		private void button1_Click(object sender, EventArgs e) {
			string returnmsg = "";
			string roomname = "wohnzimmer";
			tv = (Tv)tvs.Get(roomname);
			if(tv.name == "noDevice") {
				Logger.Write(MethodBase.GetCurrentMethod(), $"TV: '{roomname}' nicht gefunden");
			} else {
				TVParams tvp = new TVParams(
					einaus: null,
					tvbutton: "enter",
					dienst: null,
					richtung: null);
				tv.Set(tvp, out returnmsg);
				Logger.Write(MethodBase.GetCurrentMethod(), $"{returnmsg} - {tvp.ToString()}");
			}
		}
	}
}
