using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Dtos.CartItem
{
    public class CartItemCreateDto
    {
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
