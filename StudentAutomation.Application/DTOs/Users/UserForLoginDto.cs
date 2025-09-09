using StudentAutomation.Core.Abstractions;
namespace StudentAutomation.Application.DTOs.Users
{
    public class UserForLoginDto : IDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
