using System;
using System.Collections.Generic;

namespace MyShop.Application.DTOs.Product
{
    public class AddProductVariantDto
    {
        public string SKU { get; set; } = string.Empty;
        public int Price { get; set; }
        public int StockQuantity { get; set; }
        public List<AddVariantAttributeDto> Attributes { get; set; } = new List<AddVariantAttributeDto>();
    }
}
