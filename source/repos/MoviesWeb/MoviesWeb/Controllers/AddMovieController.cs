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
    [Authorize]

    public class AddMovieController : Controller
    {

        private readonly CxSettings _cxSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddMovieController(IOptions<CxSettings> cxSettings, IHttpContextAccessor httpContextAccessor)
        {
            _cxSettings = cxSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet]
        public IActionResult AddMovie()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddMovie(Create_Movies_Model createMovie)
        {
            if (!ModelState.IsValid)
            {
                return View("AddMovie", createMovie);
            }

            // Handle Release Date
            if (DateTime.TryParse(Request.Form["ReleaseDate"], out DateTime releaseDate))
            {
                createMovie.Release_date = releaseDate;
            }
            else
            {
                ModelState.AddModelError("", "Invalid date format.");
                return View("AddMovie", createMovie);
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
                        createMovie.Image = memoryStream.ToArray(); // Convert image to byte array
                    }
                }
            }
            else
            {
                // Optionally handle the case where no image is provided
                createMovie.Image = null;
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));

                // Serialize the user model to JSON
                HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(createMovie), Encoding.UTF8, "application/json");

                // Make the POST request to the API's Register endpoint
                var response = await httpClient.PostAsync(_cxSettings.MoviesAPI + "/Movies", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    // If the registration is successful, redirect to the home page
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // If the registration fails, add an error message and return to the form
                    ModelState.AddModelError("", "Registration failed. Please try again.");
                    return View("AddMovie", createMovie);
                }
            }
        }


    }
}
