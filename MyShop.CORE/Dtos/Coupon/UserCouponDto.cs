using System;

namespace MyShop.CORE.DTOs.Coupon
{
    public class UserCouponDto
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public CouponDto Coupon { get; set; } = null!;
        public bool CanUse { get; set; }
        public int UserUsageCount { get; set; }
        public int? UsageLimit { get; set; }
        public DateTime AssignedAt { get; set; }
    }
}
