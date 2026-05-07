using MyShop.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Domain.Entities
{
    public class Attribute
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string DataType { get; set; }
        public List<VariantAttribute> AttributeValues { get; set; } = new List<VariantAttribute>();
    }
}
