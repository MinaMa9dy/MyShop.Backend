using MyShop.CORE.Dtos.CartItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Dtos.Cart
{
    public class CartDto
    {
        public string UserId { get; set; } = default!;
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}
