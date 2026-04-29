using AutoMapper;
using MyShop.CORE.Dtos.Category;
using MyShop.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.AutoMapping
{
    public class CategoryProfile:Profile
    {
        public CategoryProfile()
        {
            CreateMap<AddCategoryDto, Category>().ReverseMap();
        }
    }
}
