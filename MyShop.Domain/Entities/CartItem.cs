using MyShop.Domain.Entities.ProductEntities;
using MyShop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Domain.Entities
{
    public class CartItem
    {

        [Key]
        public Guid ProductVariantId { get; set; }
        [ForeignKey(nameof(ProductVariantId))]
        public ProductVariant ProductVariant { get; set; }
        [Key]
        public Guid CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }
        public int Quantity { get; set; }

    }
}
