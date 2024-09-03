using MoviesWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoviesAPI.Models;
using MoviesWeb.Models;
using MoviesWeb.Models.System;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using System.Runtime;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Reflection.Emit;

namespace MoviesWeb.Controllers
{
    [Authorize]

    public class ProfileController : BaseController<ProfileController>
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly HttpClient _httpClient;




        public ProfileController(ILogger<ProfileController> logger, IOptions<CxSettings> cxSettings, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        //public async Task<IActionResult> Index()
        //{
        //    var movies = await GetAllMovies();
        //    return View(movies);
        //}

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var viewModel = new UserProfileViewModel();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));

                // Get current user profile
                HttpResponseMessage currentUserResponse = await client.GetAsync(CxSettings.MoviesAPI + "/User/CurrentUser");

                if (currentUserResponse.IsSuccessStatusCode)
                {
                    string currentUserData = await currentUserResponse.Content.ReadAsStringAsync();
                    var currentUser = JsonConvert.DeserializeObject<User_Model>(currentUserData);

                    if (currentUser != null)
                    {
                        viewModel.CurrentUser = new Update_Current_User_Model
                        {
                            Name = currentUser.Name,
                            Phone = currentUser.Phone,
                            Street = currentUser.Street,
                            City = currentUser.City,
                            State = currentUser.State,
                            ZipCode = currentUser.ZipCode
                        };
                    }
                }

                // Get all users
                HttpResponseMessage usersResponse = await client.GetAsync(CxSettings.MoviesAPI + "/User");

                if (usersResponse.IsSuccessStatusCode)
                {
                    string usersData = await usersResponse.Content.ReadAsStringAsync();
                    viewModel.Users = JsonConvert.DeserializeObject<List<User_Model>>(usersData);
                }
            }

            return View(viewModel);
        }



        [HttpGet]
        public async Task<IActionResult> EditProfileCurrentUser()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));

                // Make a request to get the current user's profile based on the authenticated user's token
                HttpResponseMessage responseMessage = await client.GetAsync(CxSettings.MoviesAPI + "/User/CurrentUser");

                if (responseMessage.IsSuccessStatusCode)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    var currentUser = JsonConvert.DeserializeObject<User_Model>(data);

                    if (currentUser != null)
                    {
                        var updateUserModel = new Update_Current_User_Model
                        {
                            Name = currentUser.Name,
                            Phone = currentUser.Phone,
                            Street = currentUser.Street,
                            City = currentUser.City,
                            State = currentUser.State,
                            ZipCode = currentUser.ZipCode
                        };

                        return View(updateUserModel);
                    }
                }

                // Handle error case if needed
                return View(new Update_Current_User_Model()); // or some error view
            }
        }




        [HttpGet]
            public IActionResult ChangePasswordCurrentUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditProfileCurrentUser(Update_Current_User_Model? updateCurrentUser)
        {
            using (var client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("MovieApiToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(updateCurrentUser), Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = await client.PutAsync(CxSettings.MoviesAPI + "/User/CurrentUser", requestContent);

                if (responseMessage.IsSuccessStatusCode)
                {
                    // Immediately fetch the updated data
                    HttpResponseMessage fetchResponse = await client.GetAsync(CxSettings.MoviesAPI + "/User/CurrentUser");
                    if (fetchResponse.IsSuccessStatusCode)
                    {
                        string data = await fetchResponse.Content.ReadAsStringAsync();
                        var updatedUser = JsonConvert.DeserializeObject<Update_Current_User_Model>(data);

                        // Log or debug here to confirm the data received
                        // e.g., Debug.WriteLine(JsonConvert.SerializeObject(updatedUser));

                        return RedirectToAction("Profile", "Profile");
                    }
                }

                return View();
            }
        }




        [HttpPost]
        public async Task<IActionResult> ChangePasswordCurrentUser(ChangePasswordModel changePassword)
        {
            if(changePassword.NewPassword != changePassword.ConfirmNewPassword)
            {
                return View("Profile", "Profile");

            }
            changePassword.ConfirmNewPassword = null;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));
                var content = new StringContent(JsonConvert.SerializeObject(changePassword), Encoding.UTF8, "application/json");

                var queryParams = $"?oldPassword={Uri.EscapeDataString(changePassword.OldPassword)}&newPassword={Uri.EscapeDataString(changePassword.NewPassword)}&confirmPassword={Uri.EscapeDataString(changePassword.ConfirmNewPassword ?? "null")}";
                var uri = CxSettings.MoviesAPI + "/User/NewPassword" + queryParams;

                var response = await client.PutAsync(uri, null); // No content as query parameters are used


                if (response.IsSuccessStatusCode)
                {
                    // If the registration is successful, redirect to the login page
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // If the registration fails, add an error message and return to the form
                    ModelState.AddModelError("", "Password change failed. Please try again.");
                    return View("Profile", "Profile");
                }

            }
            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
             IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                HttpResponseMessage responseMessage = await client.GetAsync(CxSettings.MoviesAPI + "/User/" + id);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User_Model>(data); // Deserialize to a single Movies_Model object

                    if (user != null)
                    {
                        return View(user); // Pass the movie model to the view
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
                HttpResponseMessage responseMessage = await client.DeleteAsync(CxSettings.MoviesAPI + "/User/?id=" + id);

                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Profile", "Profile");

                }

                return View("Error");  // Optionally return an error view or a not found view
            }
        }











    }
}
