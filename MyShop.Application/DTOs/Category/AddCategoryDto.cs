using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.DTOs.Category
{
    public class AddCategoryDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? SuperCategoryId { get; set; }
    }
}
