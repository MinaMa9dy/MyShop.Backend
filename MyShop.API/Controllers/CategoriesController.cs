using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.Application.DTOs.Category;
using MyShop.Application.Interfaces;

namespace MyShop.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;


        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [Authorize(Roles ="Admin")]
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory(AddCategoryDto addCategoryDto)
        {
            var result = await _categoryService.AddCategoryAsync(addCategoryDto);
            return StatusCode(result.Status, result);
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _categoryService.GetAllCategories();
            return StatusCode(result.Status, result);
        }
    }
}
