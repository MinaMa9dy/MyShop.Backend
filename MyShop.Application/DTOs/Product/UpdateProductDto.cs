using Microsoft.AspNetCore.Http;
using MyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.DTOs.Product
{
    public class UpdateProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool HaveSale { get; set; } = false;
        public bool IsFasting { get; set; } = false;
        public int Popularity { get; set; } = 0;
        public Guid? SupplierId { get; set; }
        public Guid? CategoryId { get; set; }
        public List<IFormFile> Photos { get; set; } = new List<IFormFile>();
        public List<Guid>? PhotoIdsToDelete { get; set; }
    }
}
