using MyShop.Domain.Entities.CouponEntities;
using MyShop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Domain.Entities
{
    public class UserCoupon
    {
        [Key]
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }
        public Guid CouponId { get; set; }
        [ForeignKey(nameof(CouponId))]
        public Coupon Coupon { get; set; }
        public bool CanUse { get; set; } = true;
        public int UserUsageCount { get; set; } = 0;
        public int? UsageLimit { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
