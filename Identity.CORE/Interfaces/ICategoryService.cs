using MyShop.CORE.Dtos.Category;
using MyShop.CORE.Entities;
using MyShop.CORE.Helpers.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Interfaces
{
    public interface ICategoryService
    {
        Task<Result<Category>> AddCategoryAsync(AddCategoryDto? addCategoryDto);
        Task<Result<IEnumerable<GetCategoryDto>>> GetAllCategories();
    }
}
