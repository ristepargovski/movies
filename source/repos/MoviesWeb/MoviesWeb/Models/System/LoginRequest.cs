using System.ComponentModel.DataAnnotations;

namespace MoviesWeb.Models.System
{
	public class LoginRequest
	{
		/// Внесено корисничко име
		[Required(ErrorMessage = "Полето {0} е задолжително")]
		[Display(Name = "Email")]
		public string Email { get; set; }

		/// Внесена лозинка
		[Required(ErrorMessage = "Полето {0} е задолжително")]
		[Display(Name = "Password")]
		public string Password { get; set; }
	}
}
