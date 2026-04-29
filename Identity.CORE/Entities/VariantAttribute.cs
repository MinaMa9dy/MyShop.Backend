using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Entities
{
    public class VariantAttribute
    {
        [Key]
        public Guid Id { get; set; }
        public Guid VariantId { get; set; }
        [ForeignKey(nameof(VariantId))]
        public ProductVariant ProductVariant { get; set; }
        public Guid AttributeId { get; set; }
        [ForeignKey(nameof(AttributeId))]
        public Attribute Attribute { get; set; }
        public string Value { get; set; } = null!;
    }
}
