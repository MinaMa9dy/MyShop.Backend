using MyShop.Domain.RepositoryInterfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShop.Application.DTOs.Product;
using MyShop.Domain.Entities;
using MyShop.Domain.Enums;
using MyShop.Application.Common;
using MyShop.Application.Common.ResultPattern;
using MyShop.Domain.Identity;
using MyShop.Application.Interfaces;
using MyShop.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MyShop.Domain.Entities.ProductEntities;
using AutoMapper.QueryableExtensions;

namespace MyShop.Application.Services
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
            var category = await _unitOfWork.Categories.Query().FirstOrDefaultAsync(c => c.Id == dto.CategoryId);
            if (category is null)
                return Result<ProductDto>.Failure("Category not found", ErrorCode.NOT_FOUND);

            var supplier = await _unitOfWork.Sellers.Query()
                .Include(s => s.User)
                .FirstOrDefaultAsync(c => c.UserId == dto.SupplierId);
            if (supplier is null)
                return Result<ProductDto>.Failure("Supplier not found", ErrorCode.NOT_FOUND);

            var product = _mapper.Map<Product>(dto);
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            if (dto.Photos != null && dto.Photos.Any())
            {
                await UploadPhotosInternalAsync(product.Id, dto.Photos);
            }

            await _cacheService.RemoveByPrefixAsync("products:");
            return await GetProductByIdAsync(product.Id);
        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(Guid id)
        {
            if (id == null || id == Guid.Empty) return Result<ProductDto>.Failure("Product ID is required", ErrorCode.VALIDATION_ERROR);

            //string cacheKey = $"product:{id}";
            //var cached = await _cacheService.GetAsync<ProductDto>(cacheKey);
            //if (cached != null) return Result<ProductDto>.Ok(cached);
            var product = await _unitOfWork.Products.Query()
                .Include(p => p.Category)
                .Include(p => p.productPhotos)
                .Include(p => p.productVariants)
                    .ThenInclude(pv => pv.VariantAttributes)
                        .ThenInclude(va => va.Attribute)
                .Include(p => p.Supplier)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(p => p.Id == id);


            if (product is null) return Result<ProductDto>.Failure("Product not found", ErrorCode.NOT_FOUND);

            var productDto = _mapper.Map<ProductDto>(product);

            //await _cacheService.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(30));
            return Result<ProductDto>.Ok(productDto);
        }
        public async Task<Result<IEnumerable<ProductDto>>> GetAllProductsAsync(SearchFilterOptions? filter)
        {
            //string cacheKey = $"products:all_{filter?.PageNumber}_{filter?.PageSize}_{filter?.SearchTerm}_{filter?.CategoryId}_{filter?.MinPrice}_{filter?.MaxPrice}_{filter?.IsFasting}_{filter?.HaveSale}";
            //var cached = await _cacheService.GetAsync<PageResult<ProductDto>>(cacheKey);
            //if (cached != null) return Result<PageResult<ProductDto>>.Ok(cached);
            
            int page = filter?.PageNumber ?? 1;
            int size = filter?.PageSize ?? 8;
            
            var parentCategories  = await _unitOfWork.Categories.GetSelfAndDescendantIdsAsync(filter?.CategoryId ?? Guid.Empty);
            
            Expression<Func<Product, bool>> criteria = p =>
                (string.IsNullOrEmpty(filter.SearchTerm) || p.Name.Contains(filter.SearchTerm)) &&
                (!filter.CategoryId.HasValue || parentCategories.Contains(p.CategoryId)) &&
                (!filter.HaveSale.HasValue || p.HaveSale == filter.HaveSale) &&
                (!filter.IsFasting.HasValue || p.IsFasting == filter.IsFasting) &&
                (!filter.MinPrice.HasValue || p.productVariants.Take(1).Any(v => v.NewPrice >= filter.MinPrice.Value)) &&
                (!filter.MaxPrice.HasValue || p.productVariants.Take(1).Any(v => v.NewPrice <= filter.MaxPrice.Value));
            var products = await _unitOfWork.Products.Query()
                .Where(criteria)
                .Include(p => p.productPhotos.Where(pp => pp.IsMain))
                .Include(p => p.productVariants.Take(1))
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Include(p => p.Supplier).ThenInclude(s => s.User)
                .OrderByDescending(p => p.Popularity)
                .Skip((page - 1) * size)
                .Take(size)
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            


            var count = await _unitOfWork.Products.Query().Where(criteria).CountAsync();
            var meta = new Meta
            {
                Total = count,
                Page = page,
                PerPage = size
            };

            //await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
            return Result<IEnumerable<ProductDto>>.Ok(products, "Success", meta);
        }

        public async Task<Result<ProductDto>> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _unitOfWork.Products.Query(false).FirstOrDefaultAsync(p => p.Id == dto.Id);
            if (product is null) return Result<ProductDto>.Failure("Product not found", ErrorCode.NOT_FOUND);

            _mapper.Map(dto, product);
            
            // Handle photo deletions
            if (dto.PhotoIdsToDelete != null && dto.PhotoIdsToDelete.Any())
            {
                foreach (var photoId in dto.PhotoIdsToDelete)
                {
                    var photo = await _unitOfWork.ProductPhotos.Query(false).FirstOrDefaultAsync(p => p.Id == photoId);
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
            var product = await _unitOfWork.Products.Query(false)
                .Include(p => p.productPhotos)
                .Include(p => p.productVariants)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return Result<bool>.Failure("Product not found", ErrorCode.NOT_FOUND);

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

            return Result<bool>.Ok(true, "Product deleted successfully");
        }

        public async Task<Result<IEnumerable<ProductDto>>> GetHotSelledProducts(int n)
        {
            //string cacheKey = $"products:hot_{n}";
            //var cached = await _cacheService.GetAsync<PageResult<ProductDto>>(cacheKey);
            //if (cached != null) return Result<PageResult<ProductDto>>.Ok(cached);

            var products = await _unitOfWork.Products.Query()
                .Include(p => p.productPhotos.Where(p => p.IsMain))
                .Include(p => p.productVariants.Take(1))
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Include(p => p.Supplier).ThenInclude(s => s.User)
                .OrderByDescending(p => p.Popularity)
                .Take(n)
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
            var count = await _unitOfWork.Products.Query().CountAsync();

            var meta = new Meta
            {
                Total = count,
                Page = 1,
                PerPage = n
            };

            //await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromDays(1));
            return Result<IEnumerable<ProductDto>>.Ok(products, "Success", meta);
        }

        public async Task<Result<List<ProductPhotoDto>>> UploadPhotosAsync(Guid productId, List<IFormFile> files)
        {
            var product = await _unitOfWork.Products.Query().FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return Result<List<ProductPhotoDto>>.Failure("Product not found", ErrorCode.NOT_FOUND);

            var photos = await UploadPhotosInternalAsync(productId, files);
            await _cacheService.RemoveAsync($"product:{productId}");
            
            var photoDtos = _mapper.Map<List<ProductPhotoDto>>(photos);
            for (int i = 0; i < photos.Count; i++)
            {
                photoDtos[i].Url = _fileService.GetFilePath(photos[i].FileName, PhotosFolder).Data;
            }

            return Result<List<ProductPhotoDto>>.Ok(photoDtos);
        }

        public async Task<Result<bool>> DeletePhotoAsync(Guid productId, Guid photoId)
        {
            var photo = await _unitOfWork.ProductPhotos.Query(false).FirstOrDefaultAsync(p => p.Id == photoId);
            if (photo == null || photo.ProductId != productId) return Result<bool>.Failure("Photo not found", ErrorCode.NOT_FOUND);

            _fileService.DeleteFile(photo.FileName, PhotosFolder);
            _unitOfWork.ProductPhotos.Delete(photo);

            if (photo.IsMain)
            {
                var nextPhoto = await _unitOfWork.ProductPhotos.Query(false)
                    .Where(p => p.ProductId == productId && p.Id != photoId)
                    .FirstOrDefaultAsync();
                if (nextPhoto != null)
                {
                    nextPhoto.IsMain = true;
                    _unitOfWork.ProductPhotos.Update(nextPhoto);
                }
            }

            await _unitOfWork.CompleteAsync();
            await _cacheService.RemoveAsync($"product:{productId}");
            return Result<bool>.Ok(true, "Photo deleted successfully");
        }

        public async Task<Result<ProductPhotoDto>> SetMainPhotoAsync(Guid productId, Guid photoId)
        {
            var photos = await _unitOfWork.ProductPhotos.Query(false)
                .Where(p => p.ProductId == productId)
                .ToListAsync();
            var targetPhoto = photos.FirstOrDefault(p => p.Id == photoId);
            if (targetPhoto == null) return Result<ProductPhotoDto>.Failure("Photo not found", ErrorCode.NOT_FOUND);

            foreach (var photo in photos)
            {
                photo.IsMain = (photo.Id == photoId);
                _unitOfWork.ProductPhotos.Update(photo);
            }

            await _unitOfWork.CompleteAsync();
            await _cacheService.RemoveAsync($"product:{productId}");

            var dto = _mapper.Map<ProductPhotoDto>(targetPhoto);
            dto.Url = _fileService.GetFilePath(targetPhoto.FileName, PhotosFolder).Data;
            return Result<ProductPhotoDto>.Ok(dto);
        }

        public async Task<Result<ProductVariantDto>> AddVariantAsync(Guid productId, AddProductVariantDto dto)
        {
            var product = await _unitOfWork.Products.Query().FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return Result<ProductVariantDto>.Failure("Product not found", ErrorCode.NOT_FOUND);

            var existingVariant = await _unitOfWork.ProductVariants.Query()
                .FirstOrDefaultAsync(v => v.ProductId == productId && v.SKU == dto.SKU);
            if (existingVariant != null) return Result<ProductVariantDto>.Failure("SKU already exists for this product", ErrorCode.DUPLICATE_ENTRY);

            var variant = _mapper.Map<ProductVariant>(dto);
            variant.ProductId = productId;
            variant.OldPrice = dto.Price;
            variant.NewPrice = dto.Price;
            
            await _unitOfWork.ProductVariants.AddAsync(variant);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveAsync($"product:{productId}");
            return Result<ProductVariantDto>.Ok(_mapper.Map<ProductVariantDto>(variant), "Variant added successfully");
        }

        public async Task<Result<ProductVariantDto>> UpdateVariantAsync(Guid productId, Guid variantId, AddProductVariantDto dto)
        {
            var variant = await _unitOfWork.ProductVariants.Query(false)
                .Include(v => v.VariantAttributes)
                .FirstOrDefaultAsync(v => v.Id == variantId);
            if (variant == null || variant.ProductId != productId) return Result<ProductVariantDto>.Failure("Variant not found", ErrorCode.NOT_FOUND);

            if (dto.Price < variant.NewPrice)
                variant.OldPrice = variant.NewPrice;
            else
                variant.OldPrice = dto.Price;
            variant.NewPrice = dto.Price;
            
            _mapper.Map(dto, variant);
            
            _unitOfWork.ProductVariants.Update(variant);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveAsync($"product:{productId}");
            return Result<ProductVariantDto>.Ok(_mapper.Map<ProductVariantDto>(variant));
        }

        public async Task<Result<bool>> DeleteVariantAsync(Guid productId, Guid variantId)
        {
            var variant = await _unitOfWork.ProductVariants.Query(false).FirstOrDefaultAsync(v => v.Id == variantId);
            if (variant == null || variant.ProductId != productId) return Result<bool>.Failure("Variant not found", ErrorCode.NOT_FOUND);

            _unitOfWork.ProductVariants.Delete(variant);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveAsync($"product:{productId}");
            return Result<bool>.Ok(true, "Variant deleted successfully");
        }


        public async Task<Result<IEnumerable<ProductDto>>> GetProductsBySellerAsync(Guid sellerId, SearchFilterOptions? filter)
        {
            if(filter == null)
                filter = new SearchFilterOptions();
            filter.SearchTerm = filter.SearchTerm ?? ""; 
            
            int page = filter.PageNumber ?? 1;
            int size = filter.PageSize ?? 8;

            Expression<Func<Product, bool>> criteria = p =>
                p.SupplierId == sellerId &&
                (string.IsNullOrEmpty(filter.SearchTerm) || p.Name.Contains(filter.SearchTerm)) &&
                (!filter.CategoryId.HasValue || p.CategoryId == filter.CategoryId);

            var products = await _unitOfWork.Products.Query()
                .Where(criteria)
                .OrderByDescending(p => p.Popularity)
                .Skip((page - 1) * size)
                .Take(size)
                .Include(p => p.productPhotos)
                .Include(p => p.productVariants)
                .ToListAsync();

            var count = await _unitOfWork.Products.Query().CountAsync(criteria);
            var productDtos = _mapper.Map<List<ProductDto>>(products.ToList());
            
            var meta = new Meta
            {
                Total = count,
                Page = page,
                PerPage = size
            };

            return Result<IEnumerable<ProductDto>>.Ok(productDtos, "Success", meta);
        }

        public async Task<Result<IEnumerable<ProductDto>>> GetRelatedProductsAsync(Guid productId, int n = 4)
        {
            var product = await _unitOfWork.Products.Query().FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return Result<IEnumerable<ProductDto>>.Failure("Product not found", ErrorCode.NOT_FOUND);

            var relatedProducts = await _unitOfWork.Products.Query()
                .Where(p => p.CategoryId == product.CategoryId && p.Id != productId)
                .Include(p => p.productPhotos.Where(pp => pp.IsMain))
                .Include(p => p.productVariants.Take(1))
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Include(p => p.Supplier).ThenInclude(s => s.User)
                .OrderByDescending(p => p.Popularity)
                .Take(n)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductDto>>(relatedProducts);
            return Result<IEnumerable<ProductDto>>.Ok(productDtos);
        }

        public async Task<Result<List<MyShop.Domain.Entities.Attribute>>> GetAttributesAsync()
        {
            var attributes = await _unitOfWork.Attributes.Query().ToListAsync();
            return Result<List<MyShop.Domain.Entities.Attribute>>.Ok(attributes.ToList());
        }

        private async Task<List<ProductPhoto>> UploadPhotosInternalAsync(Guid productId, List<IFormFile> files)
        {
            bool hasMain = await _unitOfWork.ProductPhotos.Query()
                .AnyAsync(p => p.ProductId == productId && p.IsMain);
            List<ProductPhoto> photos = new List<ProductPhoto>();

            foreach (var file in files)
            {
                var saveResult = await _fileService.SaveFileAsync(file, PhotosFolder);
                if (!saveResult.Success) continue;

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
