using System;

namespace MyShop.Application.DTOs.Product
{
    public class VariantAttributeDto
    {
        public Guid AttributeId { get; set; }
        public string AttributeName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
