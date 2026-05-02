using MyShop.CORE.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using MyShop.CORE.Entities;
using MyShop.CORE.Entities.OrderEntities;

namespace MyShop.INFRASTRUCTURE.Context
{
    public class AppDbContext: IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CORE.Entities.Attribute>(builder =>
            {
                builder.Property(a => a.Name).IsUnicode(true);
                builder.Property(a => a.DataType).IsUnicode(true);
            });

            modelBuilder.Entity<VariantAttribute>(builder =>
            {
                builder.Property(va => va.Value).IsUnicode(true);
            });

            modelBuilder.Entity<Product>(builder =>
            {
                builder.Property(p => p.Name).IsUnicode(true);
                builder.Property(p => p.Description).IsUnicode(true);
            });

            modelBuilder.Entity<Category>(builder =>
            {
                builder.Property(c => c.Name).IsUnicode(true);
                builder.Property(c => c.Description).IsUnicode(true);
            });

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public AppDbContext()
        {
            
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<UserPhoto> UserPhotos { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<ProductCoupon> ProductCoupons { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<CORE.Entities.Attribute> Attributes { get; set; }
        public DbSet<VariantAttribute> VariantAttributes { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<UserCoupon> UserCoupons { get; set; }




        }
}
