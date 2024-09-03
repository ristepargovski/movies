using MoviesAPI.Models;
using System.ComponentModel;

namespace MoviesAPI.Models
{
	public class User_Model
	{
		public long Id { get; set; }

		public Boolean? Active { get; set; }
		
		public string? Name { get; set; }
		public string? Password { get; set; }
		public string? Phone { get; set; }
		public string? Username { get; set; }
		public string? Email { get; set; }
		public string? ConfirmPassword { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public int ZipCode { get; set; }
        public string Role { get; set; }

    }

    public class Create_User_Model
	{
        public Boolean Active { get; set; }
		public string Name { get; set; }
		public string Password { get; set; }
		public string Phone { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }
		public string? ConfirmPassword { get; set; }
        public string? Token { get; set; } // Token for email verification

        //public string Street { get; set; }
        //	public string City { get; set; }
        //	public string State { get; set; }
        //	public int ZipCode { get; set; }
    }

    public class Update_User_Model
	{
		public Boolean? Active { get; set; }
		public string? Name { get; set; }
		public string? Password { get; set; }
		public string? Phone { get; set; }
		public string? Username { get; set; }
		public string? Email { get; set; }
		public string? ConfirmPassword { get; set; }
		public string? Street { get; set; }
		public string? City { get; set; }
		public string? State { get; set; }
		public int? ZipCode { get; set; }
	}

    public class Update_Current_User_Model
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public int? ZipCode { get; set; }
    }


}

public class ChangePasswordModel
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string? ConfirmNewPassword { get; set; }
}

public class NewPasswordForgotPassword
{
    public string Email { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}


public class UserProfileViewModel
{
    public Update_Current_User_Model CurrentUser { get; set; }
    public List<User_Model> Users { get; set; }
}