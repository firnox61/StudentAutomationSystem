namespace StudentAutomationMVC.Web.Models.Users
{
    public class UserListDto 
    {
        public int Id { get; set; }
        public string FullName { get; set; } // FirstName + LastName birleştirilmiş
        public string Email { get; set; }
        public bool Status { get; set; }
    }
}
