using MoviesAPI.Models;
using Newtonsoft.Json;

namespace MoviesWeb.Models.System
{
	public class LoginResponse
	{
		[JsonProperty("accessToken")]
		public string Token { get; set; }

		[JsonProperty("korisnik")]
		public User_Model user { get; set; }

		//[JsonProperty("prava")]
		// KorisnikPrava KorisnikPrava { get; set; }

		//[JsonProperty("setiranja")]
		//public IEnumerable<KorisnikSetupDto> KorisnikSetiranja { get; set; }

		[JsonProperty("error")]
		public string Error { get; set; }
	}
}
