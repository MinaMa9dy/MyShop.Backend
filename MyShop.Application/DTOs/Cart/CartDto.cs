using MyShop.Application.DTOs.CartItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.DTOs.Cart
{
    public class CartDto
    {
        public string UserId { get; set; } = default!;
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}
