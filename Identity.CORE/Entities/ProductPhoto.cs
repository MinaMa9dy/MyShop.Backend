using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyShop.CORE.Entities
{
    public class ProductPhoto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        [JsonIgnore]
        public Product Product { get; set; }

        // File info
        public string FileName { get; set; } = null!; //
        public string RelativePath { get; set; } = null!; //
        public bool IsMain { get; set; } = false;

        // Auditing
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
