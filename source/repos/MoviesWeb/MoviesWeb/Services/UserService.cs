using Microsoft.Extensions.Options;
using MoviesWeb.Models.System;
using Newtonsoft.Json;
using System.Text;
using LoginRequest = MoviesWeb.Models.System.LoginRequest;

namespace MoviesWeb.Services
{
	public class UserService:IUserService 
	{

		private readonly CxSettings _cxSettings;
		public const string MovieApiToken = "MovieApiToken";
	


        public UserService(IOptions<CxSettings> cxSettings)
		{
			_cxSettings = cxSettings.Value;

		}

		public async Task<LoginResponse> ProveriKorisnikAkreditivi(LoginRequest loginRequest)
		{
			LoginResponse loginResponse = new LoginResponse();

            using (var client = new HttpClient())
            {
         

                HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

				using (var response = await client.PostAsync(_cxSettings.MoviesAPI + "/Account/Login", requestContent))
				{
					if (response.IsSuccessStatusCode)
					{
						string apiResponse = await response.Content.ReadAsStringAsync();
						loginResponse = JsonConvert.DeserializeObject<LoginResponse>(apiResponse);
					}
					else
					{
						return null;
					}
				}
			}

			return loginResponse;
		}

		

		//public async Task<bool> AzurirajKorisnik(UpdateProfilRequest updateRequest)
		//{           


		//}
	}
}

	
