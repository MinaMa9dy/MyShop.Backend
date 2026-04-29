using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyShop.CORE.DTOs.Coupon
{
    public class BulkAssignCouponDto
    {
        [Required]
        public Guid CouponId { get; set; }
        
        /// <summary>
        /// If empty or null, assign to all users.
        /// </summary>
        public List<Guid>? UserIds { get; set; }
    }

    public class BulkAssignResultDto
    {
        public int TotalProcessed { get; set; }
        public int AlreadyAssigned { get; set; }
        public int NewlyAssigned { get; set; }
    }
}
