using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoviesAPI.Models;
using MoviesWeb.Models;
using MoviesWeb.Models.System;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MoviesWeb.Controllers
{
    public class AddTvshowController : Controller
    {

        private readonly CxSettings _cxSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddTvshowController(IOptions<CxSettings> cxSettings, IHttpContextAccessor httpContextAccessor)
        {
            _cxSettings = cxSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet]
        public IActionResult AddTvshow()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTvshow(Create_Tvshows_Model? createTvshow)
        {
            if (!ModelState.IsValid)
            {
                return View("AddTvshow", createTvshow);
            }

            // Handle Release Date
            if (DateTime.TryParse(Request.Form["ReleaseDate"], out DateTime releaseDate))
            {
                createTvshow.Release_date = releaseDate;
            }
            else
            {
                ModelState.AddModelError("", "Invalid date format.");
                return View("AddTvshow", createTvshow);
            }

            // Handle Image Upload
            if (Request.Form.Files.Count > 0)
            {
                var imageFile = Request.Form.Files[0];
                if (imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        createTvshow.Image = memoryStream.ToArray(); // Convert image to byte array
                    }
                }
            }
            else
            {
                // Optionally handle the case where no image is provided
                createTvshow.Image = null;
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));

                // Serialize the user model to JSON
                HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(createTvshow), Encoding.UTF8, "application/json");

                // Make the POST request to the API's Register endpoint
                var response = await httpClient.PostAsync(_cxSettings.MoviesAPI + "/Tvshows", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    // If the registration is successful, redirect to the home page
                    return RedirectToAction("TvShowsList", "TvShows");
                }
                else
                {
                    // If the registration fails, add an error message and return to the form
                    ModelState.AddModelError("", "Registration failed. Please try again.");
                    return View("AddTvshow", createTvshow);
                }
            }
        }


    }
}
