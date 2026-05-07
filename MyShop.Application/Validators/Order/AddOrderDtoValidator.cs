using FluentValidation;
using MyShop.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Validators.Order
{
    public class AddOrderDtoValidator:AbstractValidator<AddOrderDto>
    {
        public AddOrderDtoValidator()
        {
            
            RuleFor(o => o.PhoneNumber)
                .NotEmpty().WithMessage("Phone number shouldn't be empty");

        }
    }
}
