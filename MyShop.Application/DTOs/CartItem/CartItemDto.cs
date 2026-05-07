using System;

namespace MyShop.Application.DTOs.CartItem
{
    public class CartItemDto
    {
        public Guid ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int Price { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
