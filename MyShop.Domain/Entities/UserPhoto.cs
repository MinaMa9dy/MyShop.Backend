using MyShop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyShop.Domain.Entities
{
    public class UserPhoto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [JsonIgnore]
        public ApplicationUser User { get; set; }

        // File info
        public string FileName { get; set; } = null!;
        public string RelativePath { get; set; } = null!;
        
        // Auditing
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
