using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyShop.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.INFRASTRUCTURE.Configs
{
    public class ProductCouponConfig : IEntityTypeConfiguration<ProductCoupon>
    {
        public void Configure(EntityTypeBuilder<ProductCoupon> modelBuilder)
        {
            
            modelBuilder
                .HasKey(pc => new { pc.ProductVariantId, pc.CouponId}); // Composite Key

            modelBuilder
                .HasOne(pc => pc.ProductVariant)
                .WithMany(p => p.productCoupons)
                .HasForeignKey(pc => pc.ProductVariantId);

            modelBuilder
                .HasOne(pc => pc.Coupon)
                .WithMany(c => c.ProductCoupons)
                .HasForeignKey(pc => pc.CouponId);
        }
    }
}
