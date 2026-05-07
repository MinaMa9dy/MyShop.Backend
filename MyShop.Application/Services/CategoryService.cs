using MyShop.Domain.RepositoryInterfaces;
using AutoMapper;
using MyShop.Application.DTOs.Category;
using MyShop.Domain.Entities;
using MyShop.Application.Common.ResultPattern;
using MyShop.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;

namespace MyShop.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<Category>> AddCategoryAsync(AddCategoryDto? addCategoryDto)
        {
            if (addCategoryDto is null)
            {
                return Result<Category>.Failure("Entity is null", ErrorCode.VALIDATION_ERROR);
            }
            if (await _unitOfWork.Categories.Query().FirstOrDefaultAsync(c => c.Name == addCategoryDto.Name) is not null)
            {
                return Result<Category>.Failure("Category with the same name already exists", ErrorCode.DUPLICATE_ENTRY);
            }
            if (addCategoryDto.SuperCategoryId.HasValue && await _unitOfWork.Categories.Query().FirstOrDefaultAsync(c => c.Id == addCategoryDto.SuperCategoryId) is null)
            {
                return Result<Category>.Failure("Super category not found", ErrorCode.NOT_FOUND);
            }
            var categoryEntity = _mapper.Map<Category>(addCategoryDto);
            await _unitOfWork.Categories.AddAsync(categoryEntity);
            var result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                return Result<Category>.Created(categoryEntity, "Category created successfully");
            }
            else
            {
                return Result<Category>.Failure("Failed to add category", ErrorCode.SERVER_ERROR);
            }
        }

        public async Task<Result<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            var categories = await _unitOfWork.Categories.Query().Where(c => c.SuperCategoryId != null)
                .Include(c => c.Products)
                .Select(e => _mapper.Map<CategoryDto>(e))
                .ToListAsync();
            return Result<IEnumerable<CategoryDto>>.Ok(categories);
        }
    }
}
