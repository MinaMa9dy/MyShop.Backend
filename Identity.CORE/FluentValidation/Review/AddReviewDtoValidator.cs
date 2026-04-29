using FluentValidation;
using MyShop.CORE.Dtos.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.FluentValidation.Review
{
    public class AddReviewDtoValidator:AbstractValidator<AddReviewDto>
    {
        public AddReviewDtoValidator()
        {
            RuleFor(r => r.CustomerId)
                .NotEmpty().WithMessage("CustomerId shouldn't be empty");
            RuleFor(r=>r.ProductId)
                .NotEmpty().WithMessage("ProductId shouldn't be empty");
            RuleFor(r => r.stars)
                .InclusiveBetween(1, 5).WithMessage("Stars shouldn't be less than 1 or greater than 5");
            RuleFor(r => r.Content)
                .NotEmpty().WithMessage("Comment shouldn't be empty")
                .Must(c => c.Length > 0 && c.Length < 500).WithMessage("Comment should be less than 500 characters");


        }
    }
}
