using System.ComponentModel.DataAnnotations;

namespace StudentAutomation.Blazor.Models.Auth
{
    public class RegisterRequest
    {
        [Required, StringLength(64)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(64)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(128)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(4)]
        public string Password { get; set; } = string.Empty;

        [Required, Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor.")]
        public string PasswordConfirm { get; set; } = string.Empty;
    }
}
