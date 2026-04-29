using MyShop.CORE.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ReviewCount => Reviews.Count;
        public int Popularity { get; set; } = 0;
        public bool HaveSale { get; set; }
        public bool IsFasting { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid SupplierId { get; set; } = Guid.Empty;
        [ForeignKey(nameof(SupplierId))]
        public Seller Supplier { get; set; }
        public Guid CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        public List<ProductPhoto> productPhotos { get; set; } = new List<ProductPhoto>();
        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<WishList> Wishes { get; set; } = new List<WishList>();
        public List<ProductVariant> productVariants { get; set; } = new List<ProductVariant>();

    }
}
