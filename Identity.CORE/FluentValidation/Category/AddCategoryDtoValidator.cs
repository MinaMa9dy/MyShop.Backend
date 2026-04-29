using FluentValidation;
using MyShop.CORE.Dtos.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.FluentValidation.Category
{
    public class AddCategoryDtoValidator:AbstractValidator<AddCategoryDto>
    {
        public AddCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required")
                .MaximumLength(50).WithMessage("Category name must not exceed 50 characters");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
            

        }
    }
}
