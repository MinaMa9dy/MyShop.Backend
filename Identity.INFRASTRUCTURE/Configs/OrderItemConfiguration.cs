using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyShop.CORE.Entities.OrderEntities;

namespace MyShop.INFRASTRUCTURE.Configs
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {

            builder
                .HasOne(oi => oi.Order)
                .WithMany(o => o.orderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            
            builder
                .HasOne(oi => oi.ProductVariant)
                .WithMany() 
                .HasForeignKey(oi => oi.ProductVariantId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
