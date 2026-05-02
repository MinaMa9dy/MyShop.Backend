using MyShop.CORE.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Dtos.Wish
{
    public class WishDto
    {
        public Guid ProductId { get; set; }
        public ProductDto? Product { get; set; }
    }
}
