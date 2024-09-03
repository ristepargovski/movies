using MoviesWeb.Models.System;

namespace MoviesWeb.Services
{
	public interface IUserService
	{

		Task<LoginResponse> ProveriKorisnikAkreditivi(LoginRequest loginRequest);
		//Task<bool> AzurirajKorisnik(UpdateProfilRequest updateRequest);
	}
}
