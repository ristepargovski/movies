using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoviesWeb.Models.System;
using MoviesWeb.Services;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using MoviesAPI.Models;

namespace MoviesWeb.Controllers
{

	[Authorize]
	public class AccountController : Controller
	{
		private readonly IUserService _userService;
		private readonly CxSettings _cxSettings;

		public AccountController(IUserService userService, IOptions<CxSettings> cxSettings)
		{
			_userService = userService;
			_cxSettings = cxSettings.Value;
		}

		[AllowAnonymous]
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Profile()
		{
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));

				HttpResponseMessage responseMessage = await client.GetAsync(_cxSettings.MoviesAPI + "/Profile");

				string jsonMessage;
				using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync())
				{
					jsonMessage = new StreamReader(responseStream).ReadToEnd();
				}
				User_Model user = JsonConvert.DeserializeObject<User_Model>(jsonMessage);

				return View("Profile", user);
			}
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> Login(LoginRequest loginRequest)
		{
			if (!ModelState.IsValid)
			{
				return View(loginRequest);
			}

			LoginResponse loginResponse = await _userService.ProveriKorisnikAkreditivi(loginRequest);

			if (loginResponse != null)
			{

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, loginResponse.user.Username),
            new Claim(ClaimTypes.GivenName, loginResponse.user.Name),
			new Claim(ClaimTypes.Email, loginResponse.user.Email),
                    new Claim("ADMIN", loginResponse.user.Role.ToString()),

        //   new Claim(ClaimTypes.Email, loginResponse.user.Email),

        };
                //Session["LoginResponse"] = loginResponse;

                //var claims = new List<Claim>
                //{
                //	//new Claim(ClaimTypes.NameIdentifier, loginResponse.user.Username),
                ////	new Claim(ClaimTypes.GivenName, loginResponse.user.Name),
                //	//new Claim(ClaimTypes.Email, loginResponse.user.Email)
                //};

                //claims.Add(new Claim("ADMINISTRATOR", loginResponse.Korisnik.Administrator.ToString()));

                //claims.Add(new Claim("TP", loginResponse.Korisnik.Tp.ToString()));
                //claims.Add(new Claim("P", loginResponse.Korisnik.P.ToString()));
                //claims.Add(new Claim("RE", loginResponse.Korisnik.Re.ToString()));
                //claims.Add(new Claim("IME_PREZIME", loginResponse.Korisnik.Ime.ToString() + " " + loginResponse.Korisnik.Prezime.ToString()));
                //claims.Add(new Claim("RE", loginResponse.Korisnik.Re.ToString()));
                // стави claims за доделени права по модули
                //if (loginResponse.KorisnikPrava != null)
                //{
                //	claims.Add(new Claim("BACKUP", loginResponse.KorisnikPrava.Pravo_backup.ToString()));
                //	claims.Add(new Claim("PROJECT", loginResponse.KorisnikPrava.Pravo_project.ToString()));
                //	claims.Add(new Claim("REGISTAR", loginResponse.KorisnikPrava.Pravo_registar.ToString()));
                //	claims.Add(new Claim("TIKET_FAKTURA", loginResponse.KorisnikPrava.Pravo_tiket_faktura.ToString()));
                //}
                //else
                //{
                //	claims.Add(new Claim("BACKUP", "0"));
                //	claims.Add(new Claim("PROJECT", "0"));
                //	claims.Add(new Claim("REGISTAR", "0"));
                //	claims.Add(new Claim("TIKET_FAKTURA", "0"));
                //}


                //foreach (var setiranje in loginResponse.KorisnikSetiranja)
                //{
                //	if (setiranje.Setup_v1 != null)
                //		claims.Add(new Claim("SETUP" + setiranje.Setup_id.ToString() + "_V1", setiranje.V1 ?? setiranje.Setup_v1));

                //	if (setiranje.Setup_v2 != null)
                //		claims.Add(new Claim("SETUP" + setiranje.Setup_id.ToString() + "_V2", setiranje.V2 ?? setiranje.Setup_v2));
                //}

                //claims.Add(new Claim("TokenIntraAPI", loginResponse.Token));

                var claimsIdentity = new ClaimsIdentity(
					claims,
					CookieAuthenticationDefaults.AuthenticationScheme);

				var authProperties = new AuthenticationProperties
				{
					AllowRefresh = true,
					IsPersistent = false,
					RedirectUri = "/Login/Login"
				};

				await HttpContext.SignInAsync(
					CookieAuthenticationDefaults.AuthenticationScheme,
					new ClaimsPrincipal(claimsIdentity),
					authProperties);

                HttpContext.Session.SetString("MovieApiToken", loginResponse.Token);

                return RedirectToAction("Index", "Home");
			}

			ModelState.AddModelError("", "Погрешно корисничко име или лозинка!");
			return RedirectToAction("Index", "Home");
        }

		[HttpPost]
		public async Task<IActionResult> Profile(Update_User_Model updateRequest)
		{
			if (!ModelState.IsValid)
			{
				return View(updateRequest);
			}

			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));

				HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(updateRequest), Encoding.UTF8, "application/json");

				using (var response = await httpClient.PutAsync(_cxSettings.MoviesAPI + "/Profile", requestContent))
				{
					if (response.IsSuccessStatusCode)
					{
						return RedirectToAction("Profile", "Profile");
					}
					else
					{
						ModelState.AddModelError("", "Настана грешка при ажурирањето!");
						return View(updateRequest);
					}
				}
			}
		}

		[HttpPost]
		
		public async Task<IActionResult>Logout()
		{
			await HttpContext.SignOutAsync(
				CookieAuthenticationDefaults.AuthenticationScheme);

			return RedirectToAction("Login", "Login");
		}


    }
}
