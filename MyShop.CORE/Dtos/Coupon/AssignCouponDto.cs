using System;
using System.ComponentModel.DataAnnotations;

namespace MyShop.CORE.DTOs.Coupon
{
    public class AssignCouponDto
    {
        [Required]
        public Guid CouponId { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        public int? UsageLimit { get; set; }
    }
}
