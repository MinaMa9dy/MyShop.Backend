using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShop.CORE.Dtos.Product;
using MyShop.CORE.Entities;
using MyShop.CORE.Enums;
using MyShop.CORE.Helpers;
using MyShop.CORE.Helpers.ResultPattern;
using MyShop.CORE.Identity;
using MyShop.CORE.Interfaces;
using MyShop.CORE.Shared;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyShop.CORE.Implmentations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly ICacheService _cacheService;
        private const string PhotosFolder = "Photos";

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
            _cacheService = cacheService;
        }

        public async Task<Result<ProductDto>> AddProductAsync(AddProductDto dto)
        {
            if (dto is null) return Result<ProductDto>.Failure("Product data is required", "400");

            var category = await _unitOfWork.Categories.FindAsync(c => c.Id == dto.CategoryId);
            if (category is null) return Result<ProductDto>.Failure("Category not found", "404");

            var supplier = await _unitOfWork.Customers.FindAsync(c => c.UserId == dto.SupplierId, includes: new[] { nameof(Customer.User) });
            if (supplier is null) return Result<ProductDto>.Failure("Supplier not found", "404");

            var product = _mapper.Map<Product>(dto);
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
                        
            

            // Handle photos
            if (dto.Photos != null && dto.Photos.Any())
            {
                await UploadPhotosInternalAsync(product.Id, dto.Photos);
            }

            await _cacheService.RemoveByPrefixAsync("products:");
            return await GetProductByIdAsync(product.Id);
        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(Guid? id)
        {
            if (id == null || id == Guid.Empty) return Result<ProductDto>.Failure("Product ID is required", "400");

            //string cacheKey = $"product:{id}";
            //var cached = await _cacheService.GetAsync<ProductDto>(cacheKey);
            //if (cached != null) return Result<ProductDto>.Success(cached);

            var product = await _unitOfWork.Products.FindAsync(p => p.Id == id, 
                includes: new[] { "Category", "productPhotos", "productVariants.VariantAttributes.Attribute", "Supplier.User" });

            if (product is null) return Result<ProductDto>.Failure("Product not found", "404");

            var productDto = _mapper.Map<ProductDto>(product);
            
            // Resolve photo URLs
            foreach (var photo in productDto.ProductPhotos)
            {
                photo.Url = $"{PhotosFolder}/{photo.Url}";
            }

            //await _cacheService.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(30));
            return Result<ProductDto>.Success(productDto);
        }

        public async Task<Result<PageResult<ProductDto>>> GetAllProductsAsync(SearchFilterOptions? filter)
        {
            string cacheKey = $"products:all_{filter?.PageNumber}_{filter?.PageSize}_{filter?.SearchTerm}_{filter?.CategoryId}_{filter?.MinPrice}_{filter?.MaxPrice}_{filter?.IsFasting}_{filter?.HaveSale}";
            var cached = await _cacheService.GetAsync<PageResult<ProductDto>>(cacheKey);
            if (cached != null) return Result<PageResult<ProductDto>>.Success(cached);

            int page = filter?.PageNumber ?? 1;
            int size = filter?.PageSize ?? 8;

            // Note: price filtering might need adjustment if prices are only in variants
            // For now, filtering by variants' price
            Expression<Func<Product, bool>> criteria = p =>
                (string.IsNullOrEmpty(filter.SearchTerm) || p.Name.Contains(filter.SearchTerm)) &&
                (!filter.CategoryId.HasValue || p.CategoryId == filter.CategoryId) &&
                (!filter.HaveSale.HasValue || p.HaveSale == filter.HaveSale) &&
                (!filter.IsFasting.HasValue || p.IsFasting == filter.IsFasting);

            var products = await _unitOfWork.Products.FindAllAsync(
                criteria,
                (page - 1) * size,
                size,
                p => p.Popularity,
                OrderByOptions.Descending,
                new[] { "productPhotos", "productVariants" }
            );

            var count = await _unitOfWork.Products.CountAsync(criteria);

            var productDtos = _mapper.Map<List<ProductDto>>(products.ToList());
            
            // Resolve URLs
            foreach (var p in productDtos)
            {
                foreach (var photo in p.ProductPhotos)
                {
                    photo.Url = $"{PhotosFolder}/{photo.Url}";
                }
            }

            var result = new PageResult<ProductDto>
            {
                Items = productDtos,
                TotalItems = count,
                Page = page,
                PageSize = size
            };

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
            return Result<PageResult<ProductDto>>.Success(result);
        }

        public async Task<Result<ProductDto>> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _unitOfWork.Products.FindAsync(p => p.Id == dto.Id);
            if (product is null) return Result<ProductDto>.Failure("Product not found", "404");

            _mapper.Map(dto, product);
            
            // Handle photo deletions
            if (dto.PhotoIdsToDelete != null && dto.PhotoIdsToDelete.Any())
            {
                foreach (var photoId in dto.PhotoIdsToDelete)
                {
                    var photo = await _unitOfWork.ProductPhotos.FindAsync(p => p.Id == photoId);
                    if (photo != null)
                    {
                        _fileService.DeleteFile(photo.FileName, PhotosFolder);
                        _unitOfWork.ProductPhotos.Delete(photo);
                    }
                }
            }

            // Handle new photos
            if (dto.Photos != null && dto.Photos.Any())
            {
                await UploadPhotosInternalAsync(product.Id, dto.Photos);
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveByPrefixAsync("products:");
            await _cacheService.RemoveAsync($"product:{dto.Id}");

            return await GetProductByIdAsync(product.Id);
        }

        public async Task<Result<bool>> DeleteProductAsync(Guid id)
        {
            var product = await _unitOfWork.Products.FindAsync(p => p.Id == id, includes: new[] { "productPhotos", "productVariants" });
            if (product is null) return Result<bool>.Failure("Product not found", "404");

            foreach (var photo in product.productPhotos)
            {
                _fileService.DeleteFile(photo.FileName, PhotosFolder);
            }

            foreach (var variant in product.productVariants)
            {
                _unitOfWork.ProductVariants.Delete(variant);
            }
            
            _unitOfWork.Products.Delete(product);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveByPrefixAsync("products:");
            await _cacheService.RemoveAsync($"product:{id}");

            return Result<bool>.Success(true);
        }

        public async Task<Result<PageResult<ProductDto>>> GetHotSelledProducts(int n)
        {
            //string cacheKey = $"products:hot_{n}";
            //var cached = await _cacheService.GetAsync<PageResult<ProductDto>>(cacheKey);
            //if (cached != null) return Result<PageResult<ProductDto>>.Success(cached);

            var products = await _unitOfWork.Products.HotSelledProductsAsync(n);
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            // Resolve photo URLs
            foreach (var p in productDtos)
            {
                foreach (var photo in p.ProductPhotos)
                {
                    photo.Url = $"{PhotosFolder}/{photo.Url}";
                }
            }

            var result = new PageResult<ProductDto>
            {
                Items = productDtos,
                TotalItems = products.Count,
                Page = 1,
                PageSize = n
            };

            //await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromDays(1));
            return Result<PageResult<ProductDto>>.Success(result);
        }

        public async Task<Result<List<ProductPhotoDto>>> UploadPhotosAsync(Guid productId, List<IFormFile> files)
        {
            var product = await _unitOfWork.Products.FindAsync(p => p.Id == productId);
            if (product == null) return Result<List<ProductPhotoDto>>.Failure("Product not found", "404");

            var photos = await UploadPhotosInternalAsync(productId, files);
            await _cacheService.RemoveAsync($"product:{productId}");
            
            var photoDtos = _mapper.Map<List<ProductPhotoDto>>(photos);
            for (int i = 0; i < photos.Count; i++)
            {
                photoDtos[i].Url = _fileService.GetFilePath(photos[i].FileName, PhotosFolder).Data;
            }

            return Result<List<ProductPhotoDto>>.Success(photoDtos);
        }

        public async Task<Result<bool>> DeletePhotoAsync(Guid productId, Guid photoId)
        {
            var photo = await _unitOfWork.ProductPhotos.FindAsync(p => p.Id == photoId);
            if (photo == null || photo.ProductId != productId) return Result<bool>.Failure("Photo not found", "404");

            _fileService.DeleteFile(photo.FileName, PhotosFolder);
            _unitOfWork.ProductPhotos.Delete(photo);

            if (photo.IsMain)
            {
                var nextPhoto = (await _unitOfWork.ProductPhotos.FindAllAsync(p => p.ProductId == productId && p.Id != photoId)).FirstOrDefault();
                if (nextPhoto != null)
                {
                    nextPhoto.IsMain = true;
                    _unitOfWork.ProductPhotos.Update(nextPhoto);
                }
            }

            await _unitOfWork.CompleteAsync();
            await _cacheService.RemoveAsync($"product:{productId}");
            return Result<bool>.Success(true);
        }

        public async Task<Result<ProductPhotoDto>> SetMainPhotoAsync(Guid productId, Guid photoId)
        {
            var photos = (await _unitOfWork.ProductPhotos.FindAllAsync(p => p.ProductId == productId)).ToList();
            var targetPhoto = photos.FirstOrDefault(p => p.Id == photoId);
            if (targetPhoto == null) return Result<ProductPhotoDto>.Failure("Photo not found", "404");

            foreach (var photo in photos)
            {
                photo.IsMain = (photo.Id == photoId);
                _unitOfWork.ProductPhotos.Update(photo);
            }

            await _unitOfWork.CompleteAsync();
            await _cacheService.RemoveAsync($"product:{productId}");

            var dto = _mapper.Map<ProductPhotoDto>(targetPhoto);
            dto.Url = _fileService.GetFilePath(targetPhoto.FileName, PhotosFolder).Data;
            return Result<ProductPhotoDto>.Success(dto);
        }

        public async Task<Result<ProductVariantDto>> AddVariantAsync(Guid productId, AddProductVariantDto dto)
        {
            var product = await _unitOfWork.Products.FindAsync(p => p.Id == productId);
            if (product == null) return Result<ProductVariantDto>.Failure("Product not found", "404");

            var existingVariant = await _unitOfWork.ProductVariants.FindAsync(v => v.ProductId == productId && v.SKU == dto.SKU);
            if (existingVariant != null) return Result<ProductVariantDto>.Failure("SKU already exists for this product", "400");

            var variant = _mapper.Map<ProductVariant>(dto);
            variant.ProductId = productId;
            variant.OldPrice = dto.Price;
            variant.NewPrice = dto.Price;
            foreach(var attr in dto.Attributes)
            {
                variant.VariantAttributes.Add(new VariantAttribute
                {
                    AttributeId = attr.AttributeId,
                    Value = attr.Value
                });
            }
            await _unitOfWork.ProductVariants.AddAsync(variant);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveAsync($"product:{productId}");
            return Result<ProductVariantDto>.Success(_mapper.Map<ProductVariantDto>(variant));
        }

        public async Task<Result<ProductVariantDto>> UpdateVariantAsync(Guid productId, Guid variantId, AddProductVariantDto dto)
        {
            var variant = await _unitOfWork.ProductVariants.FindAsync(v => v.Id == variantId, includes: new[] { "VariantAttributes" });
            if (variant == null || variant.ProductId != productId) return Result<ProductVariantDto>.Failure("Variant not found", "404");

            if (dto.Price < variant.NewPrice) variant.OldPrice = variant.NewPrice;
            else variant.OldPrice = dto.Price;
            
            _mapper.Map(dto, variant);
            _unitOfWork.ProductVariants.Update(variant);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveAsync($"product:{productId}");
            return Result<ProductVariantDto>.Success(_mapper.Map<ProductVariantDto>(variant));
        }

        public async Task<Result<bool>> DeleteVariantAsync(Guid productId, Guid variantId)
        {
            var variant = await _unitOfWork.ProductVariants.FindAsync(v => v.Id == variantId);
            if (variant == null || variant.ProductId != productId) return Result<bool>.Failure("Variant not found", "404");

            _unitOfWork.ProductVariants.Delete(variant);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveAsync($"product:{productId}");
            return Result<bool>.Success(true);
        }


        public async Task<Result<PageResult<ProductDto>>> GetProductsBySellerAsync(Guid sellerId, SearchFilterOptions filter)
        {
            
            filter.SearchTerm = filter.SearchTerm ?? ""; 
            
            int page = filter.PageNumber ?? 1;
            int size = filter.PageSize ?? 8;

            Expression<Func<Product, bool>> criteria = p =>
                p.SupplierId == sellerId &&
                (string.IsNullOrEmpty(filter.SearchTerm) || p.Name.Contains(filter.SearchTerm)) &&
                (!filter.CategoryId.HasValue || p.CategoryId == filter.CategoryId);

            var products = await _unitOfWork.Products.FindAllAsync(
                criteria,
                (page - 1) * size,
                size,
                p => p.Popularity,
                OrderByOptions.Descending,
                new[] { "productPhotos", "productVariants" }
            );

            var count = await _unitOfWork.Products.CountAsync(criteria);
            var productDtos = _mapper.Map<List<ProductDto>>(products.ToList());
            
            var result = new PageResult<ProductDto>
            {
                Items = productDtos,
                TotalItems = count,
                Page = page,
                PageSize = size
            };

            return Result<PageResult<ProductDto>>.Success(result);
        }
        public async Task<Result<List<Entities.Attribute>>> GetAttributesAsync()
        {
            var attributes = await _unitOfWork.Attributes.GetAllAsync();
            return Result<List<Entities.Attribute>>.Success(attributes.ToList());
        }

        private async Task<List<ProductPhoto>> UploadPhotosInternalAsync(Guid productId, List<IFormFile> files)
        {
            bool hasMain = (await _unitOfWork.ProductPhotos.FindAsync(p => p.ProductId == productId && p.IsMain)) != null;
            List<ProductPhoto> photos = new List<ProductPhoto>();

            foreach (var file in files)
            {
                var saveResult = await _fileService.SaveFileAsync(file, PhotosFolder);
                if (!saveResult.IsSuccess) continue;

                var photo = new ProductPhoto
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    FileName = saveResult.Data,
                    RelativePath = Path.Combine(PhotosFolder, saveResult.Data),
                    IsMain = !hasMain,
                    CreatedAt = DateTime.UtcNow
                };
                photos.Add(photo);
                hasMain = true;
            }

            await _unitOfWork.ProductPhotos.AddRangeAsync(photos);
            await _unitOfWork.CompleteAsync();
            return photos;
        }
    }
}
