using FluentValidation;
using MyShop.CORE.Dtos.CartItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.FluentValidation.Cart
{
    public class CartItemDtoValidator:AbstractValidator<CartItemDto>
    {
        public CartItemDtoValidator()
        {
            


            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        }
    }
}
