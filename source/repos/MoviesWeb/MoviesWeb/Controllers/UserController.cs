using MoviesWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoviesAPI.Models;
using MoviesWeb.Models;
using MoviesWeb.Models.System;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace MoviesWeb.Controllers
{
    public class UserController : BaseController<UserController>
    {
        private readonly ILogger<UserController> _logger;
        private readonly HttpClient _httpClient;




        public UserController(ILogger<UserController> logger, IOptions<CxSettings> cxSettings)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

      
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()

        {


            List<User_Model> users = new List<User_Model>();
            HttpResponseMessage responseMessage = await _httpClient.GetAsync(CxSettings.MoviesAPI + "/User");

            if (responseMessage.IsSuccessStatusCode)
            {
                string data = await responseMessage.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<List<User_Model>>(data);

            }
            return View(users);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
