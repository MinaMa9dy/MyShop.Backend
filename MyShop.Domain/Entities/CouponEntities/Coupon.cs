using MyShop.Domain.Entities.ProductEntities;
using MyShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Domain.Entities.CouponEntities
{
    public class Coupon
    {
        [Key]
        public Guid Id { get; set; }
        public string CouponCode { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public string? CouponDescription { get; set; } = string.Empty;
        public decimal MinAmount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpirationDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int UsedCount { get; set; } = 0;
        public int? UsageLimit { get; set; }
        public List<ProductCoupon> ProductCoupons { get; set; } = new List<ProductCoupon>();
        public List<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();
    }
}
