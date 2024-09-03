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
    public class TvshowsController : Controller
    {


        private readonly HttpClient _httpClient;
        private readonly CxSettings _cxSettings;

        public TvshowsController(HttpClient httpClient, IOptions<CxSettings> cxSettings)
        {
            _httpClient = httpClient;
            _cxSettings = cxSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> TvshowsList()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));

                List<Tvshows_Model> tvshows = new List<Tvshows_Model>();
                List<Movies_Model> movies = new List<Movies_Model>();

                MovieAndTvshow_Model viewModel = new MovieAndTvshow_Model();


                string urlMovies = $"{_cxSettings.MoviesAPI}/Movies";
                string urlTvshows = $"{_cxSettings.MoviesAPI}/Tvshows";

                HttpResponseMessage responseMessageMovies = await client.GetAsync(urlMovies);
                HttpResponseMessage responseMessageTvshows = await client.GetAsync(urlTvshows);

                if (responseMessageMovies.IsSuccessStatusCode)
                {
                    string dataMovies = await responseMessageMovies.Content.ReadAsStringAsync();
                    movies = JsonConvert.DeserializeObject<List<Movies_Model>>(dataMovies);
                }
                else
                {
                    // Log or handle unsuccessful response
                    Console.WriteLine($"Error fetching movies: {responseMessageMovies.StatusCode}");
                }

                if (responseMessageTvshows.IsSuccessStatusCode)
                {
                    string dataMovies = await responseMessageTvshows.Content.ReadAsStringAsync();
                    tvshows = JsonConvert.DeserializeObject<List<Tvshows_Model>>(dataMovies);
                }
                else
                {
                    // Log or handle unsuccessful response
                    Console.WriteLine($"Error fetching movies: {responseMessageTvshows.StatusCode}");
                }

                viewModel.Movie = movies;
                viewModel.Tvshow = tvshows;


                return View("TvshowsList", viewModel);
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditTvshows(int id)
        {
            using (var client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("MovieApiToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage responseMessage = await client.GetAsync(_cxSettings.MoviesAPI + "/Tvshows/" + id);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    Update_Tvshows_Model tvshows = JsonConvert.DeserializeObject<Update_Tvshows_Model>(data);

                    if (tvshows != null)
                    {
                        return View(tvshows);
                    }
                }
                return View();
            }
        }



        [HttpGet]
        public async Task<IActionResult> DetailsTvshows(long id)
        {
            using (var client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("MovieApiToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Login");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Fetch the movie details by ID
                HttpResponseMessage responseMessage = await client.GetAsync(_cxSettings.MoviesAPI + "/Tvshows/" + id);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    return View("Error");  // Optionally return an error view or a not found view
                }

                string data = await responseMessage.Content.ReadAsStringAsync();
                var tvshows = JsonConvert.DeserializeObject<Tvshows_Model>(data);

                if (tvshows == null)
                {
                    return View("Error");  // Optionally return an error view or a not found view
                }

                // Check if the movie exists in a ticket
                var requestUri = $"{_cxSettings.MoviesAPI}/Ticket/CheckIfTvShowsExist?tvshowId={id}";
                HttpResponseMessage ticketResponseMessage = await client.GetAsync(requestUri);

                bool isPurchased = false;

                if (ticketResponseMessage.IsSuccessStatusCode)
                {
                    var result = await ticketResponseMessage.Content.ReadAsStringAsync();
                    isPurchased = bool.Parse(result);
                }
                // REVIEW REQUEST
                HttpResponseMessage RatingTvshowResponse = await client.GetAsync(_cxSettings.MoviesAPI + "/RatingTvshow?tvshow_id=" + id);
                int ratingTvshow = 0;

                if (RatingTvshowResponse.IsSuccessStatusCode)
                {
                    string responseContent = await RatingTvshowResponse.Content.ReadAsStringAsync();
                    dynamic ratingData = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    // Extract the rating value
                    int.TryParse((string)ratingData.rating, out ratingTvshow);
                }

                //COMMENT REQUEST
                HttpResponseMessage commentResponseMessage = await client.GetAsync(_cxSettings.MoviesAPI + "/Comments/GetTvshowComments?id=" + id);
                if (!commentResponseMessage.IsSuccessStatusCode)
                {
                    return View("Error");  // Optionally return an error view or a not found view
                }

                string dataComments = await commentResponseMessage.Content.ReadAsStringAsync();
                var comments = JsonConvert.DeserializeObject<List<Comments_Model>>(dataComments);

                if (comments == null)
                {
                    return View("Error");  // Optionally return an error view or a not found view
                }

                //GET CURRENT USER ID
                HttpResponseMessage UserIdResponse = await client.GetAsync(_cxSettings.MoviesAPI + "/User/IdCurrentUser/");

                int userId = 0;
                if (UserIdResponse.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    string responseContent = await UserIdResponse.Content.ReadAsStringAsync();

                    // Deserialize the response content to an integer
                    if (int.TryParse(responseContent, out int result))
                    {
                        userId = result;
                    }
                }
                // GET AVERAGE REVIEWS FOR TVSHOW AND HOW MANY USERS REVIEWED
                HttpResponseMessage ratingTvshowResponse = await client.GetAsync(_cxSettings.MoviesAPI + "/RatingTvshow/summary?tvshow_id=" + id);

                int TotalUsersRated = 0;
                double averageRating = 0.0;

                if (ratingTvshowResponse.IsSuccessStatusCode)
                {
                    string responseContent = await ratingTvshowResponse.Content.ReadAsStringAsync();

                    // Deserialize the response content
                    dynamic ratingData = JsonConvert.DeserializeObject<dynamic>(responseContent);

                    // Extract the user count and average rating with correct case
                    TotalUsersRated = Convert.ToInt32(ratingData.totalUsersRated);  // Notice the lowercase "t"
                    averageRating = Convert.ToDouble(ratingData.averageRating);  // This line is fine as it is
                }


                double averageRatingMultiply2 = averageRating * 2;





                // Create MovieViewModel and set its properties
                var viewModel = new TvshowsViewModel
                {
                    Tvshows = tvshows,
                    IsPurchased = isPurchased,
                    RatingTvshow = ratingTvshow,
                    Comments = comments,
                    UserId = userId,
                    TotalUsersRated = TotalUsersRated,
                    AverageRating = averageRating,
                    averageRatingMultiply2 = averageRatingMultiply2

                };

                return View(viewModel); // Pass the view model to the view
            }
        }




        [HttpPost]
        public async Task<IActionResult> AddRatingTvshow(int tvshow_id, Create_Rating_Tvshow_Model CreateRatingTvshow)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));
                CreateRatingTvshow.Tvshow_id = tvshow_id;
                // Serialize the user model to JSON
                HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(CreateRatingTvshow), Encoding.UTF8, "application/json");

                // Make the POST request to the API's Register endpoint
                var response = await httpClient.PostAsync(_cxSettings.MoviesAPI + "/RatingTvshow", requestContent);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to add comment. Please try again.");
                    return View();
                }
                var referrerUrl = HttpContext.Request.Headers["Referer"].ToString();
                return Redirect(referrerUrl);
            }
        }









        [HttpPost]
        public async Task<IActionResult> EditTvshows(Tvshows_Model? tvshowsmodel)
        {
            if (tvshowsmodel == null)
            {
                ModelState.AddModelError("", "Movie model is null.");
                return View("EditTvshows", tvshowsmodel);
            }

            try
            {
                // Extract ReleaseDate from the form and parse it
                if (DateTime.TryParse(Request.Form["ReleaseDate"], out DateTime releaseDate))
                {
                    tvshowsmodel.Release_date = releaseDate;
                }
                else
                {
                    ModelState.AddModelError("ReleaseDate", "Invalid date format.");
                    return View("EditTvshows", tvshowsmodel);
                }


                if (Request.Form.Files.Count > 0)
                {
                    var imageFile = Request.Form.Files[0];
                    if (imageFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await imageFile.CopyToAsync(memoryStream);
                            tvshowsmodel.Image = memoryStream.ToArray(); // Convert image to byte array
                        }
                    }
                }
                else
                {
                    // Optionally handle the case where no image is provided
                    tvshowsmodel.Image = null;
                }
                // Serialize and send the model to the API for update
                string data = JsonConvert.SerializeObject(tvshowsmodel);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));
                HttpResponseMessage response = await _httpClient.PutAsync(_cxSettings.MoviesAPI + "/Tvshows/?id=" + tvshowsmodel.Id, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("TvshowsList", "Tvshows");
                }
                else
                {
                    // Handle unsuccessful response, perhaps logging or adding a model error
                    ModelState.AddModelError("", "Failed to update movie.");
                    return View("EditTvshows", tvshowsmodel);
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                // You might want to log the exception or show an error message to the user
                ModelState.AddModelError("", "An error occurred while updating the movie.");
                return View("EditTvshows", tvshowsmodel);
            }
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int id)
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
                HttpResponseMessage responseMessage = await client.DeleteAsync(_cxSettings.MoviesAPI + "/Tvshows/?id=" + id);

                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("TvshowsList", "Tvshows");

                }

                return View("Error");  // Optionally return an error view or a not found view
            }
        }






    }
}
