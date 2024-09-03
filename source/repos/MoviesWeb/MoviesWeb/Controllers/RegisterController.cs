using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoviesAPI.Models;
using MoviesWeb.Models.System;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MoviesWeb.Controllers
{
    [AllowAnonymous]
    [Authorize]

    public class RegisterController : Controller
    {
        private readonly CxSettings _cxSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RegisterController(IOptions<CxSettings> cxSettings, IHttpContextAccessor httpContextAccessor)
        {
            _cxSettings = cxSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Display the registration form
        [HttpGet]
        public IActionResult Register(string EmailAddress)
        {
            // Use the EmailAddress value here
            var model = new Create_User_Model { Email = EmailAddress };
            return View(model);
        }


        // POST: Handle the form submission and create a new user
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(Create_User_Model createUser)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", createUser);
            }
            if(createUser.Password != createUser.ConfirmPassword)
            {
                return View("Register", createUser);
            }
            createUser.ConfirmPassword = null;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));

                // Serialize the user model to JSON
                HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(createUser), Encoding.UTF8, "application/json");

                // Make the POST request to the API's Register endpoint
                var response = await httpClient.PostAsync(_cxSettings.MoviesAPI + "/User/Register", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    // If the registration is successful, redirect to the login page
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    // If the registration fails, add an error message and return to the form
                    ModelState.AddModelError("", "Registration failed. Please try again.");
                    return View("Register", createUser);
                }
            }
        }
    }
}
