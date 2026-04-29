using MyShop.CORE.Enums;
using System;

namespace MyShop.CORE.DTOs.Coupon
{
    public class CouponDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public string CouponDescription { get; set; } = string.Empty;
        public decimal MinAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsActive { get; set; }
        public int UsedCount { get; set; }
        public int? UsageLimit { get; set; }
    }
}
