using FluentValidation;
using MyShop.Core.Dtos;


namespace MyShop.CORE.FluentValidation.Auth
{
    public class LoginDtoValidator:AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(l => l.Email)
                .NotEmpty().WithMessage("Email shouldn't be Empty")
                .EmailAddress().WithMessage("Should be valid email");
            RuleFor(l => l.Password)
                .NotEmpty().WithMessage("Password should't be empty")
                .MinimumLength(4).WithMessage("Password shouldn't be less than 4 characters!");
        }
    }
}
