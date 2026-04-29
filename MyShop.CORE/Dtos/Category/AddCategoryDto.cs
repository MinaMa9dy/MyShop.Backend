using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Dtos.Category
{
    public class AddCategoryDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? SuperCategoryId { get; set; }
    }
}
