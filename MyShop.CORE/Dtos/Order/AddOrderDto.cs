using MyShop.CORE.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Dtos.Order
{
    public class AddOrderDto
    {
        public Guid CustomerId { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public CitiesOptions City { get; set; }

        public string Street { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public Guid? CouponCode { get; set; }
    }
}
