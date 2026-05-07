using MyShop.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public Guid? SuperCategoryId { get; set; }
        public Category? SuperCategory { get; set; }

    }
}
