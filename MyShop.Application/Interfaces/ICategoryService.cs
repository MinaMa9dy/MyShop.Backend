using MyShop.Application.DTOs.Category;
using MyShop.Domain.Entities;
using MyShop.Application.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<Category>> AddCategoryAsync(AddCategoryDto? addCategoryDto);
        Task<Result<IEnumerable<CategoryDto>>> GetAllCategories();
    }
}
