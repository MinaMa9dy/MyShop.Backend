using FluentValidation;
using MyShop.CORE.Dtos.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.FluentValidation.Profile
{
    public class ChangePasswordDtoValidator: AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(p => p.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required");
            RuleFor(p => p.NewPassword)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");
            RuleFor(p => p.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required")
                .Equal(p => p.NewPassword).WithMessage("Passwords do not match");
        }
    }
}
