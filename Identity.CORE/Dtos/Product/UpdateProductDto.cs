using Microsoft.AspNetCore.Http;
using MyShop.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Dtos.Product
{
    public class UpdateProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool HaveSale { get; set; } = false;
        public bool IsFasting { get; set; } = false;
        public int Popularity { get; set; } = 0;
        //public int NewPrice { get; set; }
        //public int StockQuantity { get; set; }
        //public int ShownQuantity { get; set; }
        public Guid? SupplierId { get; set; }
        public Guid? CategoryId { get; set; }
        public List<IFormFile> Photos { get; set; } = new List<IFormFile>();
        public List<Guid>? PhotoIdsToDelete { get; set; }
    }
}
