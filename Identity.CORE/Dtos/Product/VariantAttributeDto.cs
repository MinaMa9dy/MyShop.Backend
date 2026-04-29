using System;

namespace MyShop.CORE.Dtos.Product
{
    public class VariantAttributeDto
    {
        public Guid AttributeId { get; set; }
        public string AttributeName { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
