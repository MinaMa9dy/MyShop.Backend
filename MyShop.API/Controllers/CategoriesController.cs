using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.CORE.Dtos.Category;
using MyShop.CORE.Interfaces;

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
            if(result.IsSuccess == false)
            {
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            }
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(categories);
        }
    }
}
