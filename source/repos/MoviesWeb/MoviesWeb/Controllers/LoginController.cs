using Microsoft.AspNetCore.Mvc;

namespace MoviesWeb.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
      
    }
}
