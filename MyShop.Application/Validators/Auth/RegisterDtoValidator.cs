using FluentValidation;
using MyShop.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Validators.Auth
{
    public class RegisterDtoValidator:AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email shouldn't be Empty")
                .EmailAddress().WithMessage("Should be valid email");
            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password should't be empty")
                .MinimumLength(6).WithMessage("Password shouldn't be less than 6 characters!");
            RuleFor(r => r.FirstName)
                .NotEmpty().WithMessage("First Name should't be empty")
                .MinimumLength(2).WithMessage("First Name can't be less than 2 characters");
            RuleFor(r => r.LastName)
                .NotEmpty().WithMessage("Last Name should't be empty")
                .MinimumLength(2).WithMessage("Last Name can't be less than 2 characters");
            RuleFor(r => r.PhoneNumber)
                .NotEmpty().WithMessage("Phone number can't be empty");
            RuleFor(r => r.ConfirmPassword)
                .NotEmpty().WithMessage("ConfirmPassword can't be empty")
                .Equal(r => r.Password).WithMessage("Password and Confirm Password not the same");


        }
    }
}
