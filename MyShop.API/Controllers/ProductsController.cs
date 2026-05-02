using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.CORE.Dtos.Product;
using MyShop.CORE.Enums;
using MyShop.CORE.Interfaces;
using MyShop.CORE.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] SearchFilterOptions search)
        {
            var result = await _productService.GetAllProductsAsync(search);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);
        }

        [HttpGet("hot")]
        public async Task<IActionResult> GetHotestProducts([FromQuery] int numberOfProducts = 8)
        {
            var result = await _productService.GetHotSelledProducts(numberOfProducts);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductDto addProductDto)
        {
            var result = await _productService.AddProductAsync(addProductDto);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id,UpdateProductDto updateProductDto)
        {
            updateProductDto.Id = id;
            var result = await _productService.UpdateProductAsync(updateProductDto);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);
        }

        // Photo Endpoints
        [Authorize(Roles = "Seller")]
        [HttpPost("{id}/photos")]
        public async Task<IActionResult> UploadPhotos(Guid id, List<IFormFile> files)
        {
            var result = await _productService.UploadPhotosAsync(id, files);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{id}/photos/{photoId}")]
        public async Task<IActionResult> DeletePhoto(Guid id, Guid photoId)
        {
            var result = await _productService.DeletePhotoAsync(id, photoId);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpPut("{id}/photos/{photoId}/set-main")]
        public async Task<IActionResult> SetMainPhoto(Guid id, Guid photoId)
        {
            var result = await _productService.SetMainPhotoAsync(id, photoId);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);
        }

        // Variant Endpoints
        [Authorize(Roles = "Seller")]
        [HttpPost("{id}/variants")]
        public async Task<IActionResult> AddVariant(Guid id, AddProductVariantDto dto)
        {
            var result = await _productService.AddVariantAsync(id, dto);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpPut("{id}/variants/{variantId}")]
        public async Task<IActionResult> UpdateVariant(Guid id, Guid variantId, AddProductVariantDto dto)
        {
            var result = await _productService.UpdateVariantAsync(id, variantId, dto);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{id}/variants/{variantId}")]
        public async Task<IActionResult> DeleteVariant(Guid id, Guid variantId)
        {
            var result = await _productService.DeleteVariantAsync(id, variantId);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);
        }


        [Authorize(Roles = "Seller")]
        [HttpGet("seller/{sellerId}")]
        public async Task<IActionResult> GetSellerProducts(Guid sellerId, [FromQuery] SearchFilterOptions search)
        {
            var result = await _productService.GetProductsBySellerAsync(sellerId, search);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);
        }
        [HttpGet("Attributes")]
        public async Task<IActionResult> GetProductAttributes()
        {
            var result = await _productService.GetAttributesAsync();
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result);
            return Ok(result);

        }
        [HttpGet("Cities")]
        public IActionResult GetCities()
        {
            var Cities = Enum.GetNames<CitiesOptions>().Skip(1);
            return Ok(Cities);
        }
    }
}
