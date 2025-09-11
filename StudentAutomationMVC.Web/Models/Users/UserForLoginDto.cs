using System.ComponentModel.DataAnnotations;
namespace StudentAutomationMVC.Web.Models.Users
{
    public class UserForLoginDto 
    {
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Password { get; set; } = string.Empty;
    }
}
