using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyShop.Domain.Entities;
using MyShop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.INFRASTRUCTURE.Configs
{
    public class AppUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(u => u.LastName)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(u => u.Email)
                .IsRequired();
            builder.Property(u => u.Gender)
            .IsRequired();

            builder
                .HasOne(u => u.userPhoto)
                .WithOne(p => p.User)
                .HasForeignKey<UserPhoto>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        

        }

    }
}
