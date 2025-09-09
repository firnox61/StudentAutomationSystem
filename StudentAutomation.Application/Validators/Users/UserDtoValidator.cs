using FluentValidation;
using StudentAutomation.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Validators.Users
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Geçerli kullanıcı ID girilmelidir");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad boş olamaz");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad boş olamaz");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Geçerli bir e-posta girilmelidir");
        }
    }
}
