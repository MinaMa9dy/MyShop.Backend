using System;
using System.Collections.Generic;

namespace MyShop.CORE.Dtos.Product
{
    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int OldPrice { get; set; }
        public int NewPrice { get; set; }
        public int StockQuantity { get; set; }
        public List<VariantAttributeDto> Attributes { get; set; } = new List<VariantAttributeDto>();
    }
}
