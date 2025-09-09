using FluentValidation;
using StudentAutomation.Application.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAutomation.Application.Validators.Users
{
    public class OperationClaimCreateDtoValidator : AbstractValidator<OperationClaimCreateDto>
    {
        public OperationClaimCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Rol adı boş olamaz.")
                .MaximumLength(50).WithMessage("Rol adı en fazla 50 karakter olabilir.");
        }
    }
}
