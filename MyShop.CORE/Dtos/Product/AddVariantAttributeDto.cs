using System;

namespace MyShop.CORE.Dtos.Product
{
    public class AddVariantAttributeDto
    {
        public Guid AttributeId { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}
