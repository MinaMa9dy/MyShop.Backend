using System;
using System.Collections.Generic;

namespace MyShop.CORE.DTOs.Coupon
{
    public class CouponResponseDto
    {
        public CouponDto Coupon { get; set; } = null!;
        public decimal TotalDiscount { get; set; }
        public decimal FinalSubtotal { get; set; }
        public Dictionary<Guid, decimal>? ItemPrices { get; set; }
    }
}
