using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.DTOs.Review
{
    public class AddReviewDto
    {
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
        public int stars { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
