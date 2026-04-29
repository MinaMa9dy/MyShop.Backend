using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyShop.CORE.Dtos.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ReviewCount { get; set; }
        public int Popularity { get; set; }
        public bool HaveSale { get; set; }
        public bool IsFasting { get; set; }
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        
        // Default/Selected Variant fields for quick display
        public int OldPrice { get; set; }
        public int NewPrice { get; set; }
        public int StockQuantity { get; set; }

        public List<ProductPhotoDto> ProductPhotos { get; set; } = new List<ProductPhotoDto>();
        public List<ProductVariantDto> ProductVariants { get; set; } = new List<ProductVariantDto>();
    }
}
