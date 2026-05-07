using System;

namespace MyShop.Application.DTOs.Product
{
    public class AddVariantAttributeDto
    {
        public Guid AttributeId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
