using FluentValidation;
using MyShop.CORE.Dtos.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.FluentValidation.Profile
{
    public class UpdateProfileDtoValidator:AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator()
        {
            RuleFor(p => p.FirstName)
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters");
            RuleFor(p => p.LastName)
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");
            RuleFor(p => p.Address)
                .MaximumLength(500).WithMessage("Address must not exceed 500 characters");
            
                
        }

    }
}
