using System;

namespace MyShop.CORE.Dtos.Product
{
    public class ProductPhotoDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }
}
