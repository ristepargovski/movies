using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Models;

using Newtonsoft.Json;
using MoviesWeb.Models.System;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Authorization;
using System.Xml.Linq;
using MoviesWeb.Models;
public class MoviesController : Controller
{
    
    private readonly HttpClient _httpClient;
    private readonly CxSettings _cxSettings;

        public MoviesController(HttpClient httpClient, IOptions<CxSettings> cxSettings)
    {
        _httpClient = httpClient;
        _cxSettings = cxSettings.Value;
    }

    // Action to display the search page
    [HttpGet]
    public async Task<IActionResult> SearchResults()
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));

            List<Movies_Model> movies = new List<Movies_Model>();

            // Fetch all movies
            string url = $"{_cxSettings.MoviesAPI}/Movies";
            HttpResponseMessage responseMessage = await client.GetAsync(url);

            if (responseMessage.IsSuccessStatusCode)
            {
                string dataMovie = await responseMessage.Content.ReadAsStringAsync();
                movies = JsonConvert.DeserializeObject<List<Movies_Model>>(dataMovie);
            }

            // Fetch average ratings
            var ratingsResponse = await client.GetAsync($"{_cxSettings.MoviesAPI}/RatingMovie/AvgRatingMovies");
            if (!ratingsResponse.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch ratings.");
            }

            var avgRatingsJson = await ratingsResponse.Content.ReadAsStringAsync();
            var avgRatings = JsonConvert.DeserializeObject<List<AllMoviesRatingAvg>>(avgRatingsJson);

            // Map average ratings to a dictionary
            var ratingDictionary = avgRatings.ToDictionary(r => r.Movie_id, r => r.average_rating);

            // Update movie ratings
            foreach (var movie in movies)
            {
                if (ratingDictionary.TryGetValue(movie.Id, out var avgRating))
                {
                    movie.Rating = 0;
                    movie.Rating = avgRating *2;
                }
            }

            return PartialView("SearchResults", movies);
        }
    }






    [HttpGet]
    public async Task<IActionResult> SearchResultsNavbar(string searchQuery)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));

            List<Movies_Model> movies = new List<Movies_Model>();
            List<Tvshows_Model> tvshows = new List<Tvshows_Model>();

            MovieAndTvshow_Model viewModel = new MovieAndTvshow_Model();

            string urlMovies = string.IsNullOrEmpty(searchQuery) ? $"{_cxSettings.MoviesAPI}/Movies" : $"{_cxSettings.MoviesAPI}/Movies/Search?name={searchQuery}";
            string urlTvshows = string.IsNullOrEmpty(searchQuery) ? $"{_cxSettings.MoviesAPI}/Tvshows" : $"{_cxSettings.MoviesAPI}/Tvshows/Search?name={searchQuery}";


            try
            {
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
                    string dataTvshows = await responseMessageTvshows.Content.ReadAsStringAsync();
                    tvshows = JsonConvert.DeserializeObject<List<Tvshows_Model>>(dataTvshows);
                }
                else
                {
                    // Log or handle unsuccessful response
                    Console.WriteLine($"Error fetching TV shows: {responseMessageTvshows.StatusCode}");
                }

                viewModel.Movie = movies;
                viewModel.Tvshow = tvshows;

                // Log the view model for debugging
                Console.WriteLine($"ViewModel: {JsonConvert.SerializeObject(viewModel)}");

                return View("searchResultsNarbar", viewModel);
            }
            catch (Exception ex)
            {
                // Log exceptions
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500); // Internal server error
            }
        }
    }






    //[HttpGet]
    //public async Task<IActionResult> DetailsMovie(int id)
    //{
    //    using (var client = new HttpClient())
    //    {
    //        var token = HttpContext.Session.GetString("MovieApiToken");
    //        if (string.IsNullOrEmpty(token))
    //        {
    //            return RedirectToAction("Login", "Account");
    //        }

    //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    //        // Fetch the movie details by ID
    //        HttpResponseMessage responseMessage = await client.GetAsync(_cxSettings.MoviesAPI + "/Movies/" + id);

    //        if (responseMessage.IsSuccessStatusCode)
    //        {
    //            string data = await responseMessage.Content.ReadAsStringAsync();
    //            var movie = JsonConvert.DeserializeObject<Movies_Model>(data); // Deserialize to a single Movies_Model object

    //            if (movie != null)
    //            {
    //                return View(movie); // Pass the movie model to the view
    //            }
    //        }

    //        return View("Error");  // Optionally return an error view or a not found view
    //    }
    //}



    [HttpGet]
    public async Task<IActionResult> DetailsMovie(int id)
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
            HttpResponseMessage responseMessage = await client.GetAsync(_cxSettings.MoviesAPI + "/Movies/" + id);

            if (!responseMessage.IsSuccessStatusCode)
            {
                return View("Error");  // Optionally return an error view or a not found view
            }

            string data = await responseMessage.Content.ReadAsStringAsync();
            var movie = JsonConvert.DeserializeObject<Movies_Model>(data);

            if (movie == null)
            {
                return View("Error");  // Optionally return an error view or a not found view
            }

            // Fetch Movie Comments
            HttpResponseMessage commentResponseMessage = await client.GetAsync(_cxSettings.MoviesAPI + "/Comments/GetMovieComments?id=" + id);
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

            // Check if the movie exists in a ticket
            var requestUri = $"{_cxSettings.MoviesAPI}/Ticket/CheckIfMovieExist?movieId={id}";
            HttpResponseMessage ticketResponseMessage = await client.GetAsync(requestUri);

            bool isPurchased = false;

            if (ticketResponseMessage.IsSuccessStatusCode)
            {
                var result = await ticketResponseMessage.Content.ReadAsStringAsync();
                isPurchased = bool.Parse(result);
            }

            // Get Current User Id
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

            // Get Current User Review for movie with id
            HttpResponseMessage RatingMovieResponse = await client.GetAsync(_cxSettings.MoviesAPI + "/RatingMovie?movie_id="+ id);
            int ratingMovie = 0;

            if (RatingMovieResponse.IsSuccessStatusCode)
            {
                string responseContent = await RatingMovieResponse.Content.ReadAsStringAsync();
                dynamic ratingData = JsonConvert.DeserializeObject<dynamic>(responseContent);

                // Extract the rating value
                int.TryParse((string)ratingData.rating, out ratingMovie);
            }


            // GET AVERAGE REVIEWS FOR MOVIE AND HOW MANY USERS REVIEWED

            HttpResponseMessage ratingTvshowResponse = await client.GetAsync(_cxSettings.MoviesAPI + "/RatingMovie/summary?movie_id=" + id);

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
            var viewModel = new MovieViewModel
            {
                Movie = movie,
                IsPurchased = isPurchased,
                Comments = comments,
                UserId = userId,
                RatingMovie = ratingMovie,
                TotalUsersRated = TotalUsersRated,
                AverageRating = averageRating,
                averageRatingMultiply2 = averageRatingMultiply2
            };

            return View(viewModel); // Pass the view model to the view
        }
    }



    [HttpPost]
    public async Task<IActionResult> AddRatingMovie(int movie_id, Create_Rating_Movie_Model CreateRatingMovie)
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));
            CreateRatingMovie.Movie_id = movie_id;
            // Serialize the user model to JSON
            HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(CreateRatingMovie), Encoding.UTF8, "application/json");

            // Make the POST request to the API's Register endpoint
            var response = await httpClient.PostAsync(_cxSettings.MoviesAPI + "/RatingMovie", requestContent);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to add comment. Please try again.");
                return View();
            }
            var referrerUrl = HttpContext.Request.Headers["Referer"].ToString();
            return Redirect(referrerUrl);
        }
    }





    [HttpGet]
    public async Task<IActionResult> DeleteConfirmation(int id)
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
            HttpResponseMessage responseMessage = await client.GetAsync(_cxSettings.MoviesAPI + "/Movies/" + id);

            if (responseMessage.IsSuccessStatusCode)
            {
                string data = await responseMessage.Content.ReadAsStringAsync();
                var movie = JsonConvert.DeserializeObject<Movies_Model>(data); // Deserialize to a single Movies_Model object

                if (movie != null)
                {
                    return View(movie); // Pass the movie model to the view
                }
            }

            return View("Error");  // Optionally return an error view or a not found view
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
            HttpResponseMessage responseMessage = await client.DeleteAsync(_cxSettings.MoviesAPI + "/Movies/?id=" + id);

            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index","Home");

            }

            return View("Error");  // Optionally return an error view or a not found view
        }
    }


    [HttpGet]
    public async Task<IActionResult>EditMovie(int id)
    {
        using(var client = new HttpClient())
        {
            var token = HttpContext.Session.GetString("MovieApiToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage responseMessage = await client.GetAsync(_cxSettings.MoviesAPI + "/Movies/" + id);
       
        if(responseMessage.IsSuccessStatusCode)
            {
                string data = await responseMessage.Content.ReadAsStringAsync();
                Update_Movie_Model movie = JsonConvert.DeserializeObject<Update_Movie_Model>(data);
         
        if (movie != null)
            {
                return View(movie);
            }
            }
            return View();
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditMovie(Movies_Model? movie)
    {
        if (movie == null)
        {
            ModelState.AddModelError("", "Movie model is null.");
            return View("EditMovie", movie);
        }

        try
        {
            // Extract ReleaseDate from the form and parse it
            if (DateTime.TryParse(Request.Form["ReleaseDate"], out DateTime releaseDate))
            {
                movie.Release_date = releaseDate;
            }
            else
            {
                ModelState.AddModelError("ReleaseDate", "Invalid date format.");
                return View("EditMovie", movie);
            }
           
                // Serialize and send the model to the API for update
                string data = JsonConvert.SerializeObject(movie);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));
            HttpResponseMessage response = await _httpClient.PutAsync(_cxSettings.MoviesAPI + "/Movies/?id=" + movie.Id, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Handle unsuccessful response, perhaps logging or adding a model error
                ModelState.AddModelError("", "Failed to update movie.");
                return View("EditMovie", movie);
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            // You might want to log the exception or show an error message to the user
            ModelState.AddModelError("", "An error occurred while updating the movie.");
            return View("EditMovie", movie);
        }
    }




}
