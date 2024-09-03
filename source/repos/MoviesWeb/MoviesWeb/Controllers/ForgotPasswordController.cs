using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoviesAPI.Models;
using MoviesWeb.Models.System;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text;

namespace MoviesWeb.Controllers
{
    [AllowAnonymous]

    public class ForgotPasswordController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly CxSettings _cxSettings;


        public ForgotPasswordController(HttpClient httpClient, IOptions<CxSettings> cxSettings)
        {
            _httpClient = httpClient;
            _cxSettings = cxSettings.Value;
        }



        [HttpPost]
        public async Task<IActionResult> SendCode(string email)
        {
            using (var client = new HttpClient())
            {
                // Create the content for the POST request as JSON
                var content = new StringContent(JsonConvert.SerializeObject(new { Email = email }), Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage responseMessage = await client.PostAsync(_cxSettings.MoviesAPI + "/Verification/send-code", content);

                if (responseMessage.IsSuccessStatusCode)
                {
                    // Optionally, get response data if needed
                    string data = await responseMessage.Content.ReadAsStringAsync();

                    // Set ViewBag flag to show the code input field
                    ViewBag.CodeSent = true;

                    // Return the view with success message
                    return NoContent();

                }
                else
                {
                    // Handle non-success status codes
                    string errorMessage = await responseMessage.Content.ReadAsStringAsync();
                    ViewBag.CodeSent = false;
                    ViewBag.Message = errorMessage;

                    // Return the view with error message
                    return NoContent();
                }
            }
        }




        [HttpPost]
        public async Task<IActionResult> VerifyCode(string email,string code)
        {
            using (var client = new HttpClient())
            {
                // Create the content for the POST request as JSON
                var content = new StringContent(JsonConvert.SerializeObject(new { Email = email,Code = code }), Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage responseMessage = await client.PostAsync(_cxSettings.MoviesAPI + "/Verification/verify-code", content);

                if (responseMessage.IsSuccessStatusCode)
                {
                    // Optionally, get response data if needed
                    string data = await responseMessage.Content.ReadAsStringAsync();

                    // Set ViewBag flag to show the code input field
                    ViewBag.CodeSent = true;

                    // Return the view with success message
                    ViewData["Email"] = email;
                    return NoContent();


                }
                else
                {
                    return NoContent();
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult>NewPassword([FromBody]NewPasswordForgotPassword newPassword)
        {
            using (var client = new HttpClient())
            {
                if (newPassword.NewPassword != newPassword.ConfirmNewPassword)
                {
                    return BadRequest("The new password and confirm password do not match.");
                }

                var content = new StringContent(JsonConvert.SerializeObject(newPassword), Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = await client.PutAsync(_cxSettings.MoviesAPI + "/Verification/new-password", content);

          



                if (responseMessage.IsSuccessStatusCode)
                {
                    // If the registration is successful, redirect to the login page
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // If the registration fails, add an error message and return to the form
                    ModelState.AddModelError("", "Password change failed. Please try again.");
                    return NoContent();        
                }

            }
        }




    }
}
