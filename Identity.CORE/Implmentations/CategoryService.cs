using AutoMapper;
using MyShop.CORE.Dtos.Category;
using MyShop.CORE.Entities;
using MyShop.CORE.Helpers.ResultPattern;
using MyShop.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MyShop.CORE.Implmentations
{
    public class CategoryService:ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Category>> AddCategoryAsync(AddCategoryDto? addCategoryDto)
        {
            if(addCategoryDto is null)
            {
                return Result<Category>.Failure("Entity is null", "400");
            }
            if (await _unitOfWork.Categories.FindAsync(c => c.Name == addCategoryDto.Name) is not null)
            {
                return Result<Category>.Failure("Category with the same name already exists", "400");
            }
            if (addCategoryDto.SuperCategoryId.HasValue && await _unitOfWork.Categories.FindAsync(c => c.Id == addCategoryDto.SuperCategoryId) is null)
            {
                return Result<Category>.Failure("Super category not found", "400");
            }
            var categoryEntity = _mapper.Map<Category>(addCategoryDto);
            await _unitOfWork.Categories.AddAsync(categoryEntity);
            var result = await _unitOfWork.CompleteAsync();
            if(result > 0)
            {
                return Result<Category>.Success(categoryEntity);
            }
            else
            {
                return Result<Category>.Failure("Failed to add category", "500");
            }


        }

        public async Task<Result<IEnumerable<GetCategoryDto>>> GetAllCategories()
        {
            var categories = await _unitOfWork.Categories.GetCategoriesWithProductsCount();

            
            return Result<IEnumerable<GetCategoryDto>>.Success(categories);
        }
    }
}
