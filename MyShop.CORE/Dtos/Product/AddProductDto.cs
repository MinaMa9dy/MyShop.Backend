using Microsoft.AspNetCore.Http;
using MyShop.CORE.Entities;
using MyShop.CORE.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Dtos.Product
{
    public class AddProductDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        //public decimal Price { get; set; } = 0;
        public bool IsFasting { get; set; } = false;
        public bool HaveSale { get; set; } = false;
        public int Popularity { get; set; } = 0;
        //public int Stock { get; set; }
        public Guid CategoryId { get; set; } = Guid.Empty;
        public Guid SupplierId { get; set; } = Guid.Empty;
        public List<IFormFile>? Photos { get; set; }
    }
}
