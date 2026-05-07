using MyShop.Domain.Entities.OrderEntities;
using MyShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.DTOs.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SellerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerPhone { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public Guid? AppliedCouponCode { get; set; }
        public string Status { get; set; } = string.Empty;
        public CitiesOptions City { get; set; }
        public string Street { get; set; } = string.Empty;

        public string? Comment { get; set; }
        public List<OrderItem> orderItems { get; set; } = new List<OrderItem>();

    }
}
