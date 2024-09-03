using MoviesWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoviesAPI.Models;
using MoviesWeb.Models;
using MoviesWeb.Models.System;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;

namespace MoviesWeb.Controllers
{
    [Authorize]


    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;




        public HomeController(ILogger<HomeController> logger, IOptions<CxSettings> cxSettings)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        //public async Task<IActionResult> Index()
        //{
        //    var movies = await GetAllMovies();
        //    return View(movies);
        //}

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUser = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name).Value;
            }

            return View();
        }


       
    }
}




//var apiToken = HttpContext.Session.GetString("MovieApiToken");

//if (string.IsNullOrEmpty(apiToken))
//{
//    // User is not logged in or token is not available, return the home view
//    return View("Home"); // Make sure you have a Home view in the appropriate location
//}

//// User is logged in, proceed with fetching the movie data
//using (var client = new HttpClient())
//{
//    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

//    HttpResponseMessage responseMessage = await client.GetAsync(CxSettings.MoviesAPI + "/Movies");
//    List<Movies_Model> movies = new List<Movies_Model>();

//    if (responseMessage.IsSuccessStatusCode)
//    {
//        string data = await responseMessage.Content.ReadAsStringAsync();
//        movies = JsonConvert.DeserializeObject<List<Movies_Model>>(data);
//    }

//    return View(movies);
//}