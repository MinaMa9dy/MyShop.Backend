using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Dtos.Review
{
    public class ReviewResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public string Content { get; set; } = string.Empty;
        public int stars { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
