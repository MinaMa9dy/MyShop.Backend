using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MyShop.CORE.Dtos.CartItem;
using MyShop.CORE.Dtos.Product;
using MyShop.CORE.Entities;
using MyShop.CORE.Helpers.ResultPattern;
using MyShop.CORE.Identity;
using MyShop.CORE.Interfaces;
using MyShop.CORE.RepositoriyInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Implmentations
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
            var customer = await _unitOfWork.Customers.FindAsync(c => c.UserId == userId);
            if(customer is null)
            {
                return Result<CartItemDto>.Failure("Customer not found", "404");
            }
            if (dto.Quantity <= 0)
            {
                return Result<CartItemDto>.Failure("Quantity must be greater than 0", "400");
            }

            var variant = await _unitOfWork.ProductVariants.FindAsync(
                v => v.Id == dto.ProductVariantId, 
                includes: new[] { "Product.productPhotos", "Product.Category", "VariantAttributes.Attribute" });

            if (variant is null)
            {
                return Result<CartItemDto>.Failure("Product variant not found", "404");
            }

            if (dto.Quantity > variant.StockQuantity)
            {
                return Result<CartItemDto>.Failure($"Insufficient stock. Only {variant.StockQuantity} items available.", "400");
            }

            var existingItem = await _unitOfWork.CartItems.FindAsync(
                c => c.ProductVariantId == dto.ProductVariantId && c.CustomerId == userId);

            if (existingItem != null)
            {
                int newQuantity = existingItem.Quantity + dto.Quantity;
                if (newQuantity > variant.StockQuantity)
                {
                    return Result<CartItemDto>.Failure($"Cannot add more. Total in cart ({newQuantity}) would exceed stock ({variant.StockQuantity}).", "400");
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
            var finalItem = await _unitOfWork.CartItems.FindAsync(
                c => c.ProductVariantId == dto.ProductVariantId && c.CustomerId == userId,
                includes: new[] { "ProductVariant.Product.productPhotos", "ProductVariant.Product.Category", "ProductVariant.VariantAttributes.Attribute" });

            return Result<CartItemDto>.Success(_mapper.Map<CartItemDto>(finalItem));
        }

        public async Task<Result<bool>> RemoveFromCartAsync(Guid userId, Guid productVariantId)
        {
            var cartItem = await _unitOfWork.CartItems.FindAsync(
                c => c.CustomerId == userId && c.ProductVariantId == productVariantId);

            if (cartItem is null)
            {
                return Result<bool>.Failure("Item not found in cart", "404");
            }

            _unitOfWork.CartItems.Delete(cartItem);
            await _unitOfWork.CompleteAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<CartItemDto>> UpdateQuantityAsync(Guid userId, CartItemUpdateDto dto)
        {
            var customer = await _unitOfWork.Customers.FindAsync(c => c.UserId == userId);
            if (dto.Quantity <= 0)
            {
                return Result<CartItemDto>.Failure("Quantity must be greater than 0", "400");
            }

            var variant = await _unitOfWork.ProductVariants.FindAsync(v => v.Id == dto.ProductVariantId);
            if (variant == null)
            {
                return Result<CartItemDto>.Failure("Product variant not found", "404");
            }

            if (dto.Quantity > variant.StockQuantity)
            {
                return Result<CartItemDto>.Failure($"Insufficient stock. Only {variant.StockQuantity} items available.", "400");
            }

            var cartItem = await _unitOfWork.CartItems.FindAsync(
                c => c.CustomerId == userId && c.ProductVariantId == dto.ProductVariantId,
                includes: new[] { "ProductVariant.Product.productPhotos", "ProductVariant.Product.Category", "ProductVariant.VariantAttributes.Attribute" });

            if (cartItem is null)
            {
                return Result<CartItemDto>.Failure("Item not found in cart", "404");
            }

            cartItem.Quantity = dto.Quantity;
            _unitOfWork.CartItems.Update(cartItem);
            await _unitOfWork.CompleteAsync();

            return Result<CartItemDto>.Success(_mapper.Map<CartItemDto>(cartItem));
        }
    }
}
