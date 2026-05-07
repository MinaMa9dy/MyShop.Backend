using System;

namespace MyShop.Application.DTOs.Product
{
    public class ProductPhotoDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }
}
