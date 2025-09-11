namespace StudentAutomationMVC.Web.Models.Users
{
    public class UserCreateDto 
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // düz şifre
        public bool Status { get; set; }
    }
}
