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
using System.Text;
using System.Collections.Generic;
using System.Net.Http;

namespace MoviesWeb.Controllers
{
    [Authorize]
    public class EditProfileController : BaseController<EditProfileController>
    {
        private readonly ILogger<EditProfileController> _logger;
        private readonly HttpClient _httpClient;
        private readonly CxSettings _cxSettings;


        public EditProfileController(ILogger<EditProfileController> logger, IOptions<CxSettings> cxSettings, HttpClient httpClient)
        {
            _logger = logger;
            _cxSettings = cxSettings.Value;
            _httpClient = new HttpClient();
        }
        [HttpGet]

        public async Task<IActionResult> EditProfile(int id)
        {
            using (var client = new HttpClient())
            {
                var token = HttpContext.Session.GetString("MovieApiToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Account");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage responseMessage = await client.GetAsync(_cxSettings.MoviesAPI + "/User/" + id);  // Adjust the endpoint as needed

                if (responseMessage.IsSuccessStatusCode)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    Update_User_Model user = JsonConvert.DeserializeObject<Update_User_Model>(data);

                    if (user != null)
                    {
                        return View(user);
                    }
                }

                return View();  // Optionally return an error view or a not found view
            }
        }

      


        [HttpPost]
        public async Task<IActionResult> EditProfile(User_Model model)
        {
            try
            {
                if(model.Password != model.ConfirmPassword)
                {
                  return View("Profile", "Profile");
                }
                model.ConfirmPassword = null;

                // Serialize and send the model to the API for update
                string data = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));
                HttpResponseMessage response = await _httpClient.PutAsync(_cxSettings.MoviesAPI + "/User/" + model.Id, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Profile", "Profile");
                }
                else
                {
                  
                    return RedirectToAction(); 
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                // You might want to log the exception or show an error message to the user
                return RedirectToAction("Index"); // Redirect to a default action or another appropriate action
            }
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
