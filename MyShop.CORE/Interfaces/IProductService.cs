using Microsoft.AspNetCore.Http;
using MyShop.CORE.Dtos.Product;
using MyShop.CORE.Entities;
using MyShop.CORE.Helpers;
using MyShop.CORE.Helpers.ResultPattern;
using MyShop.CORE.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyShop.CORE.Interfaces
{
    public interface IProductService
    {
        // Core CRUD
        Task<Result<ProductDto>> AddProductAsync(AddProductDto dto);
        Task<Result<ProductDto>> GetProductByIdAsync(Guid id);
        Task<Result<PageResult<ProductDto>>> GetAllProductsAsync(SearchFilterOptions? filter);
        Task<Result<ProductDto>> UpdateProductAsync(UpdateProductDto dto);
        Task<Result<bool>> DeleteProductAsync(Guid id);
        Task<Result<PageResult<ProductDto>>> GetHotSelledProducts(int n);
        Task<Result<List<Entities.Attribute>>> GetAttributesAsync();

        // Photo management
        Task<Result<List<ProductPhotoDto>>> UploadPhotosAsync(Guid productId, List<IFormFile> files);
        Task<Result<bool>> DeletePhotoAsync(Guid productId, Guid photoId);
        Task<Result<ProductPhotoDto>> SetMainPhotoAsync(Guid productId, Guid photoId);

        // Variant management
        Task<Result<ProductVariantDto>> AddVariantAsync(Guid productId, AddProductVariantDto dto);
        Task<Result<ProductVariantDto>> UpdateVariantAsync(Guid productId, Guid variantId, AddProductVariantDto dto); // Reusing AddProductVariantDto for simplicity or create UpdateVariantDto
        Task<Result<bool>> DeleteVariantAsync(Guid productId, Guid variantId);

        // Seller-specific
        Task<Result<PageResult<ProductDto>>> GetProductsBySellerAsync(Guid sellerId, SearchFilterOptions? filter);
    }
}
