using AutoMapper;
using MyShop.Application.DTOs.Category;
using MyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Mapping
{
    public class CategoryProfile:Profile
    {
        public CategoryProfile()
        {
            CreateMap<AddCategoryDto, Category>().ReverseMap();
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ProductsCount, opt => opt.MapFrom(src => src.Products.Any() ? src.Products.Count : 0))
                .ReverseMap();
        }
    }
}
