using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyShop.Application.DTOs.Coupon
{
    public class BulkAssignCouponDto
    {
        [Required]
        public Guid CouponId { get; set; }
        public List<Guid> UserIds { get; set; } = new List<Guid>();
        public int? UsageLimit { get; set; }
    }

    public class BulkAssignResultDto
    {
        public int TotalProcessed { get; set; }
        public int AlreadyAssigned { get; set; }
        public int NewlyAssigned { get; set; }
    }
}
