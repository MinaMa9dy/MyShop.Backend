using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MyShop.Application.DTOs.CartItem;
using MyShop.Application.DTOs.Product;
using MyShop.Domain.Entities;
using MyShop.Application.Common.ResultPattern;
using MyShop.Domain.Identity;
using MyShop.Application.Interfaces;
using MyShop.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyShop.Application.Services
{
    public class CartItemsService : ICartItemsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartItemsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CartItemDto>> AddToCartAsync(Guid userId, CartItemCreateDto dto)
        {
            var customer = await _unitOfWork.Customers.Query().FirstOrDefaultAsync(c => c.UserId == userId);
            if(customer is null)
            {
                return Result<CartItemDto>.Failure("Customer not found", ErrorCode.NOT_FOUND);
            }
            if (dto.Quantity <= 0)
            {
                return Result<CartItemDto>.Failure("Quantity must be greater than 0", ErrorCode.VALIDATION_ERROR);
            }

            var variant = await _unitOfWork.ProductVariants.Query()
                .Include(v => v.Product)
                    .ThenInclude(p => p.productPhotos)
                .Include(v => v.Product)
                    .ThenInclude(p => p.Category)
                .Include(v => v.VariantAttributes)
                    .ThenInclude(va => va.Attribute)
                .FirstOrDefaultAsync(v => v.Id == dto.ProductVariantId);

            if (variant is null)
            {
                return Result<CartItemDto>.Failure("Product variant not found", ErrorCode.NOT_FOUND);
            }

            if (dto.Quantity > variant.StockQuantity)
            {
                return Result<CartItemDto>.Failure($"Insufficient stock. Only {variant.StockQuantity} items available.", ErrorCode.VALIDATION_ERROR);
            }

            var existingItem = await _unitOfWork.CartItems.Query(false)
                .FirstOrDefaultAsync(c => c.ProductVariantId == dto.ProductVariantId && c.CustomerId == userId);

            if (existingItem != null)
            {
                int newQuantity = existingItem.Quantity + dto.Quantity;
                if (newQuantity > variant.StockQuantity)
                {
                    return Result<CartItemDto>.Failure($"Cannot add more. Total in cart ({newQuantity}) would exceed stock ({variant.StockQuantity}).", ErrorCode.VALIDATION_ERROR);
                }
                existingItem.Quantity = newQuantity;
                _unitOfWork.CartItems.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CustomerId = userId,
                    ProductVariantId = dto.ProductVariantId,
                    Quantity = dto.Quantity
                };
                await _unitOfWork.CartItems.AddAsync(cartItem);
            }

            await _unitOfWork.CompleteAsync();

            // Refresh item to get all included data for the DTO
            var finalItem = await _unitOfWork.CartItems.Query()
                .Include(c => c.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                        .ThenInclude(p => p.productPhotos)
                .Include(c => c.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                        .ThenInclude(p => p.Category)
                .Include(c => c.ProductVariant)
                    .ThenInclude(pv => pv.VariantAttributes)
                        .ThenInclude(va => va.Attribute)
                .FirstOrDefaultAsync(c => c.ProductVariantId == dto.ProductVariantId && c.CustomerId == userId);

            return Result<CartItemDto>.Ok(_mapper.Map<CartItemDto>(finalItem));
        }

        public async Task<Result<bool>> RemoveFromCartAsync(Guid userId, Guid productVariantId)
        {
            var cartItem = await _unitOfWork.CartItems.Query(false)
                .FirstOrDefaultAsync(c => c.CustomerId == userId && c.ProductVariantId == productVariantId);

            if (cartItem is null)
            {
                return Result<bool>.Failure("Item not found in cart", ErrorCode.NOT_FOUND);
            }

            _unitOfWork.CartItems.Delete(cartItem);
            await _unitOfWork.CompleteAsync();
            return Result<bool>.Ok(true);
        }

        public async Task<Result<CartItemDto>> UpdateQuantityAsync(Guid userId, UpdateCartItemDto dto)
        {
            var customer = await _unitOfWork.Customers.Query().FirstOrDefaultAsync(c => c.UserId == userId);
            if (dto.Quantity <= 0)
            {
                return Result<CartItemDto>.Failure("Quantity must be greater than 0", ErrorCode.VALIDATION_ERROR);
            }

            var variant = await _unitOfWork.ProductVariants.Query().FirstOrDefaultAsync(v => v.Id == dto.ProductVariantId);
            if (variant == null)
            {
                return Result<CartItemDto>.Failure("Product variant not found", ErrorCode.NOT_FOUND);
            }

            if (dto.Quantity > variant.StockQuantity)
            {
                return Result<CartItemDto>.Failure($"Insufficient stock. Only {variant.StockQuantity} items available.", ErrorCode.VALIDATION_ERROR);
            }

            var cartItem = await _unitOfWork.CartItems.Query(false)
                .Include(c => c.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                        .ThenInclude(p => p.productPhotos)
                .Include(c => c.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                        .ThenInclude(p => p.Category)
                .Include(c => c.ProductVariant)
                    .ThenInclude(pv => pv.VariantAttributes)
                        .ThenInclude(va => va.Attribute)
                .FirstOrDefaultAsync(c => c.CustomerId == userId && c.ProductVariantId == dto.ProductVariantId);

            if (cartItem is null)
            {
                return Result<CartItemDto>.Failure("Item not found in cart", ErrorCode.NOT_FOUND);
            }

            cartItem.Quantity = dto.Quantity;
            _unitOfWork.CartItems.Update(cartItem);
            await _unitOfWork.CompleteAsync();

            return Result<CartItemDto>.Ok(_mapper.Map<CartItemDto>(cartItem));
        }
    }
}
