using MyShop.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace MyShop.Application.DTOs.Coupon
{
    public class CreateCouponDto
    {
        [Required]
        public string CouponCode { get; set; } = string.Empty;

        [Required]
        public DiscountType DiscountType { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal DiscountValue { get; set; }
        
        public string? CouponDescription { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal MinAmount { get; set; }
        
        public DateTime? ExpirationDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int? UsageLimit { get; set; }
    }
}
