using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Entities
{
    public class ProductCoupon
    {
        public int Id { get; set; }
        public Guid ProductVariantId { get; set; }
        [ForeignKey(nameof(ProductVariantId))]
        public ProductVariant ProductVariant { get; set; }
        public Guid CouponId { get; set; }
        [ForeignKey(nameof(CouponId))]
        public Coupon Coupon { get; set; }

    }
}
