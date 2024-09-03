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
using System.Runtime;
using System.Text;
using System.Security.Claims;

namespace MoviesWeb.Controllers
{
    public class TicketController : BaseController<TicketController>
    {
        private readonly ILogger<TicketController> _logger;
        private readonly HttpClient _httpClient;




        public TicketController(ILogger<TicketController> logger, IOptions<CxSettings> cxSettings)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Ticket()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));
                HttpResponseMessage responseMessage = await client.GetAsync(CxSettings.MoviesAPI + "/Ticket");

                if (responseMessage.IsSuccessStatusCode)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    Console.WriteLine("API Response: " + data); // Debug output

                    // Deserialize the response into a dynamic object or a strong type representing the API response
                    var ticketData = JsonConvert.DeserializeObject<TicketApiResponse>(data);

                    if (ticketData != null)
                    {
                        // Combine the movie and TV show tickets into a single list of TicketResponse
                        var tickets = new List<TicketResponse>();

                        if (ticketData.Movies != null)
                        {
                            tickets.AddRange(ticketData.Movies.Select(m => new TicketResponse { Movie = m }));
                        }

                        if (ticketData.TVShows != null)
                        {
                            tickets.AddRange(ticketData.TVShows.Select(t => new TicketResponse { Tvshow = t }));
                        }

                        Console.WriteLine("Deserialized Tickets Count: " + tickets.Count); // Debug output
                        return View(tickets);
                    }
                }

                Console.WriteLine("API call failed with status: " + responseMessage.StatusCode); // Debug output
                return View(new List<TicketResponse>());
            }
        }





        [HttpGet]
        public async Task<JsonResult> CheckIfMovieExistsInTicket(long movieId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));
                var requestUri = $"{CxSettings.MoviesAPI}/Ticket/CheckIfMovieExist?movieId={movieId}";

                HttpResponseMessage responseMessage = await client.GetAsync(requestUri);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = await responseMessage.Content.ReadAsStringAsync();
                    return Json(bool.Parse(result));
                }
                else
                {
                    return Json(false);
                }
            }
        }




        [HttpPost]
        public async Task<IActionResult> DeleteTicket(long id)
        {
            using (var client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("MovieApiToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Fetch the movie details by ID
                HttpResponseMessage responseMessage = await client.DeleteAsync(CxSettings.MoviesAPI + "/Ticket?id=" + id);

                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Ticket", "Ticket");

                }

                return View("Error");  // Optionally return an error view or a not found view
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateTicket(int movieId, Create_Ticket_Model? createTicket)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Login", "Account");
            }

            createTicket.Movie_id = movieId;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));
                var curUserResponse = await httpClient.GetAsync(CxSettings.MoviesAPI + "/User/CurrentUser");

                if (!curUserResponse.IsSuccessStatusCode)
                {
                    // Handle the case where the user is not found or another error occurred
                    ModelState.AddModelError("", "Failed to retrieve the current user.");
                    return RedirectToAction("Index", "Home");
                }

                // Deserialize the user object from the response
                var curUserContent = await curUserResponse.Content.ReadAsStringAsync();
                var curUser = JsonConvert.DeserializeObject<User_Model>(curUserContent);

                // Now set the User_id in the createTicket model
                createTicket.User_id = (int)curUser.Id;

                // Serialize the createTicket model to JSON
                HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(createTicket), Encoding.UTF8, "application/json");

                // Make the POST request to the API's Ticket creation endpoint
                var response = await httpClient.PostAsync(CxSettings.MoviesAPI + "/Ticket", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    // If the registration is successful, redirect to the login page
                    return RedirectToAction("Ticket", "Ticket");
                }
                else
                {
                    // If the registration fails, add an error message and return to the form
                    ModelState.AddModelError("", "Failed to buy this movie. Please try again.");
                    return RedirectToAction("Index", "Home");
                }
            }
        }






        /// <summary>
        ///// ///////////////////////////////////////////////////////
        ///




        [HttpPost]
        public async Task<IActionResult> CreateTicketTvshow(int tvshowId, Create_Ticket_Model? createTicket)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Login", "Account");
            }

            createTicket.Tvshow_id = tvshowId;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));
                var curUserResponse = await httpClient.GetAsync(CxSettings.MoviesAPI + "/User/CurrentUser");

                if (!curUserResponse.IsSuccessStatusCode)
                {
                    // Handle the case where the user is not found or another error occurred
                    ModelState.AddModelError("", "Failed to retrieve the current user.");
                    return RedirectToAction("Index", "Home");
                }

                // Deserialize the user object from the response
                var curUserContent = await curUserResponse.Content.ReadAsStringAsync();
                var curUser = JsonConvert.DeserializeObject<User_Model>(curUserContent);

                // Now set the User_id in the createTicket model
                createTicket.User_id = (int)curUser.Id;

                // Serialize the createTicket model to JSON
                HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(createTicket), Encoding.UTF8, "application/json");

                // Make the POST request to the API's Ticket creation endpoint
                var response = await httpClient.PostAsync(CxSettings.MoviesAPI + "/Ticket", requestContent);

                if (response.IsSuccessStatusCode)
                {
                    // If the registration is successful, redirect to the login page
                    return RedirectToAction("Ticket", "Ticket");
                }
                else
                {
                    // If the registration fails, add an error message and return to the form
                    ModelState.AddModelError("", "Failed to buy this movie. Please try again.");
                    return RedirectToAction("TvshowsList", "Tvshows");
                }
            }
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
