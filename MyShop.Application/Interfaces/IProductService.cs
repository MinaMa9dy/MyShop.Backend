using Microsoft.AspNetCore.Http;
using MyShop.Application.DTOs.Product;
using MyShop.Domain.Entities;
using MyShop.Application.Common;
using MyShop.Application.Common.ResultPattern;
using MyShop.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyShop.Application.Interfaces
{
    public interface IProductService
    {
        // Core CRUD
        Task<Result<ProductDto>> AddProductAsync(AddProductDto dto);
        Task<Result<ProductDto>> GetProductByIdAsync(Guid id);
        Task<Result<IEnumerable<ProductDto>>> GetAllProductsAsync(SearchFilterOptions? filter);
        Task<Result<ProductDto>> UpdateProductAsync(UpdateProductDto dto);
        Task<Result<bool>> DeleteProductAsync(Guid id);
        Task<Result<IEnumerable<ProductDto>>> GetHotSelledProducts(int n);
        Task<Result<List<MyShop.Domain.Entities.Attribute>>> GetAttributesAsync();

        // Photo management
        Task<Result<List<ProductPhotoDto>>> UploadPhotosAsync(Guid productId, List<IFormFile> files);
        Task<Result<bool>> DeletePhotoAsync(Guid productId, Guid photoId);
        Task<Result<ProductPhotoDto>> SetMainPhotoAsync(Guid productId, Guid photoId);

        // Variant management
        Task<Result<ProductVariantDto>> AddVariantAsync(Guid productId, AddProductVariantDto dto);
        Task<Result<ProductVariantDto>> UpdateVariantAsync(Guid productId, Guid variantId, AddProductVariantDto dto); // Reusing AddProductVariantDto for simplicity or create UpdateVariantDto
        Task<Result<bool>> DeleteVariantAsync(Guid productId, Guid variantId);

        // Seller-specific
        Task<Result<IEnumerable<ProductDto>>> GetProductsBySellerAsync(Guid sellerId, SearchFilterOptions? filter);

        Task<Result<IEnumerable<ProductDto>>> GetRelatedProductsAsync(Guid productId, int n = 4);
    }
}
