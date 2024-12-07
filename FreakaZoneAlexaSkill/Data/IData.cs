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
