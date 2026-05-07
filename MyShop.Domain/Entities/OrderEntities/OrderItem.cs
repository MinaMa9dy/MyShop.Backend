using MyShop.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyShop.Domain.Entities.OrderEntities
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }//
        [ForeignKey(nameof(OrderId))]
        [JsonIgnore]
        public Order Order { get; set; }
        public Guid? ProductVariantId { get; set; }//
        [ForeignKey(nameof(ProductVariantId))]
        [JsonIgnore]
        public ProductVariant? ProductVariant { get; set; }
        public int Quantity { get; set; }//
        public decimal UnitPrice { get; set; }//
        public string ProductName { get; set; }//
        public string? ProductPhotoPath { get; set; }//
    }
}
