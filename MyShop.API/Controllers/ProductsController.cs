using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.Application.DTOs.Product;
using MyShop.Domain.Enums;
using MyShop.Application.Interfaces;
using MyShop.Domain.Shared;
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
            return StatusCode(result.Status, result);
        }

        [HttpGet("{id}/related")]
        public async Task<IActionResult> GetRelatedProducts(Guid id, [FromQuery] int n = 4)
        {
            var result = await _productService.GetRelatedProductsAsync(id, n);
            return StatusCode(result.Status, result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] SearchFilterOptions search)
        {
            var result = await _productService.GetAllProductsAsync(search);
            return StatusCode(result.Status, result);
        }

        [HttpGet("hot")]
        public async Task<IActionResult> GetHotestProducts([FromQuery] int numberOfProducts = 8)
        {
            var result = await _productService.GetHotSelledProducts(numberOfProducts);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductDto addProductDto)
        {
            var result = await _productService.AddProductAsync(addProductDto);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Seller")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id,UpdateProductDto updateProductDto)
        {
            updateProductDto.Id = id;
            var result = await _productService.UpdateProductAsync(updateProductDto);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _productService.DeleteProductAsync(id);
            return StatusCode(result.Status, result);
        }

        // Photo Endpoints
        [Authorize(Roles = "Seller")]
        [HttpPost("{id}/photos")]
        public async Task<IActionResult> UploadPhotos(Guid id, List<IFormFile> files)
        {
            var result = await _productService.UploadPhotosAsync(id, files);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{id}/photos/{photoId}")]
        public async Task<IActionResult> DeletePhoto(Guid id, Guid photoId)
        {
            var result = await _productService.DeletePhotoAsync(id, photoId);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Seller")]
        [HttpPut("{id}/photos/{photoId}/set-main")]
        public async Task<IActionResult> SetMainPhoto(Guid id, Guid photoId)
        {
            var result = await _productService.SetMainPhotoAsync(id, photoId);
            return StatusCode(result.Status, result);
        }

        // Variant Endpoints
        [Authorize(Roles = "Seller")]
        [HttpPost("{id}/variants")]
        public async Task<IActionResult> AddVariant(Guid id, AddProductVariantDto dto)
        {
            var result = await _productService.AddVariantAsync(id, dto);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Seller")]
        [HttpPut("{id}/variants/{variantId}")]
        public async Task<IActionResult> UpdateVariant(Guid id, Guid variantId, AddProductVariantDto dto)
        {
            var result = await _productService.UpdateVariantAsync(id, variantId, dto);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{id}/variants/{variantId}")]
        public async Task<IActionResult> DeleteVariant(Guid id, Guid variantId)
        {
            var result = await _productService.DeleteVariantAsync(id, variantId);
            return StatusCode(result.Status, result);
        }

        //Should be only for seller to get their products with filter and pagination
        [Authorize(Roles = "Seller")]
        [HttpGet("seller/{sellerId}")]
        public async Task<IActionResult> GetSellerProducts(Guid sellerId, [FromQuery] SearchFilterOptions search)
        {
            var result = await _productService.GetProductsBySellerAsync(sellerId, search);
            return StatusCode(result.Status, result);
        }
        [Authorize(Roles = "Seller")]
        [HttpGet("Attributes")]
        public async Task<IActionResult> GetProductAttributes()
        {
            var result = await _productService.GetAttributesAsync();
            return StatusCode(result.Status, result);

        }
        [HttpGet("Cities")]
        public IActionResult GetCities()
        {
            var Cities = Enum.GetNames<CitiesOptions>().Skip(1);
            var result = Application.Common.ResultPattern.Result<IEnumerable<string>>.Ok(Cities);
            return StatusCode(result.Status, result);
        }
    }
}
