using FluentValidation;
using MyShop.CORE.Dtos.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.FluentValidation.Order
{
    public class AddOrderDtoValidator:AbstractValidator<AddOrderDto>
    {
        public AddOrderDtoValidator()
        {
            
            RuleFor(o => o.CustomerId)
                .NotEmpty().WithMessage("CustomerId shouldn't be empty");
            RuleFor(o => o.PhoneNumber)
                .NotEmpty().WithMessage("Phone number shouldn't be empty");

        }
    }
}
