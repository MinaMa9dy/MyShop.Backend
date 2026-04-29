using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Dtos.Profile
{
    public class UpdateProfileDto
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }
        [MaxLength(100)]
        public string? LastName { get; set; }


        [MaxLength(500)]
        public string? Address { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
