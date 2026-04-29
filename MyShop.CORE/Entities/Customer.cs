using MyShop.CORE.Entities.OrderEntities;
using MyShop.CORE.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Entities
{
    public class Customer
    {
        [Key]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
        public List<WishList> Wishes { get; set; } = new List<WishList>();
        public List<CartItem> cartItems { get; set; } = new List<CartItem>();
        public List<Order> Orders { get; set; }= new List<Order>(); // orders
        public List<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();
    }
}
