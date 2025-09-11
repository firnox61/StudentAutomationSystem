using System.ComponentModel.DataAnnotations;
namespace StudentAutomationMVC.Web.Models.Users
{
    public class UserForRegisterDto 
    {
        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        [StringLength(100, ErrorMessage = "E-posta en fazla 100 karakter olabilir.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6, en fazla 100 karakter olmalıdır.")]
        // İsterseniz temel karmaşıklık için Regex ekleyebilirsiniz:
        // [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{6,}$",
        //  ErrorMessage = "Şifre en az bir büyük, bir küçük harf ve bir rakam içermelidir.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Şifre tekrar zorunludur.")]
        [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
    }
}
