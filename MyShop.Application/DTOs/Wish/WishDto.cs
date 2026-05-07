using MyShop.Application.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.DTOs.Wish
{
    public class WishDto
    {
        public Guid ProductId { get; set; }
        public ProductDto? Product { get; set; }
    }
}
