using Microsoft.AspNetCore.Identity;
using MyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Domain.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => FirstName + " " + LastName;
        public bool IsBanned { get; set; } = false;
        public bool Gender { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public UserPhoto? userPhoto { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();


    }
}
