using FluentValidation;
using MyShop.CORE.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.FluentValidation.Product
{
    public class AddProductDtoValidator:AbstractValidator<AddProductDto>
    {
        public AddProductDtoValidator()
        {
            RuleFor(p=> p.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(50).WithMessage("Product name must not exceed 50 characters");
            RuleFor(p => p.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
            RuleFor(p => p.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required");
            RuleFor(p => p.SupplierId)
                .NotEmpty().WithMessage("SupplierId is required");
            RuleFor(p => p.Popularity)
                .GreaterThanOrEqualTo(0).WithMessage("Popularity must be non negative number");

        }
    }
}
