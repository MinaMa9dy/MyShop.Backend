using MyShop.CORE.Enums;
using MyShop.CORE.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyShop.CORE.Entities.OrderEntities
{
    public class Order
    {
        public Guid Id { get; set; } 
        public Guid CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }
        public Guid SellerId { get; set; }
        [ForeignKey(nameof(SellerId))]
        public Seller Seller { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerPhone { get; set; }
        public CitiesOptions City { get; set; }
        public string Street { get; set; }
        public DeliveryStatusOptions Status { get; set; }
        public string? Comment { get; set; }
        
        public Guid? AppliedCouponCode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal DiscountAmount { get; set; }  
        public decimal TotalAmount { get; set; }   

        [JsonIgnore]
        public List<OrderItem> orderItems { get; set; } = new List<OrderItem>();


    }
}
