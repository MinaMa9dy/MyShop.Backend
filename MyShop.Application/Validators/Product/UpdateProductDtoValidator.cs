using FluentValidation;
using MyShop.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Validators.Product
{
    public class UpdateProductDtoValidator:AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(50).WithMessage("Product name must not exceed 50 characters");
            RuleFor(p => p.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
            RuleFor(p => p.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required");
            RuleFor(p => p.SupplierId)
                .NotEmpty().WithMessage("SupplierId is required");
            RuleFor(p => p.Popularity)
                .NotEmpty().WithMessage("Price shouldn't be empty")
                .GreaterThanOrEqualTo(0).WithMessage("Popularity must be non negative number");
        }
    }
}
