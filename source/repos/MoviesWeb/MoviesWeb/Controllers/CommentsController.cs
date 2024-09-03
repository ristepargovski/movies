using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoviesAPI.Models;
using MoviesWeb.Models.System;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace MoviesWeb.Controllers
{
    public class CommentsController : Controller
    {
        private readonly CxSettings _cxSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentsController(IOptions<CxSettings> cxSettings, IHttpContextAccessor httpContextAccessor)
        {
            _cxSettings = cxSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }


        



       [HttpPost]
        public async Task<IActionResult> AddComment(Create_Comments_Model AddComment)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("MovieApiToken"));

                // Serialize the user model to JSON
                HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(AddComment), Encoding.UTF8, "application/json");

                // Make the POST request to the API's Register endpoint
                var response = await httpClient.PostAsync(_cxSettings.MoviesAPI + "/Comments", requestContent);

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
                HttpResponseMessage responseMessage = await client.DeleteAsync(_cxSettings.MoviesAPI + "/Comments/DeleteMovie?id=" + id);

                // Get the referrer URL
                var referrerUrl = HttpContext.Request.Headers["Referer"].ToString();
                return Redirect(referrerUrl);
            }
        }

    }
}
