using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.CORE.Dtos.Cart;
using MyShop.CORE.Dtos.CartItem;
using MyShop.CORE.Dtos.Product;
using MyShop.CORE.Entities;
using MyShop.CORE.Enums;
using MyShop.CORE.Helpers;
using MyShop.CORE.Helpers.ResultPattern;
using MyShop.CORE.Identity;
using MyShop.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Implmentations
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }
        

        public async Task<Result<bool>> ClearCartAsync(Guid userId)
        {
            List<CartItem> cartItems = (List<CartItem>) await _unitOfWork.CartItems.FindAllAsync(c => c.CustomerId == userId);
            _unitOfWork.CartItems.DeleteRange(cartItems);
            await _unitOfWork.CompleteAsync();
            return Result<bool>.Success(true);

        }

        public Task<Result<PageResult<CartDto>>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        

        public async Task<Result<CartDto>> GetByUserIdAsync(Guid userId)
        {
            
            var cartitems = await _unitOfWork.CartItems.FindAllAsync(c => c.CustomerId == userId, includes: new[] { nameof(CartItem.ProductVariant), "ProductVariant.Product.productPhotos", "ProductVariant.Product.Category", "ProductVariant.Product.Supplier.User" });
            
            var cartDto = new CartDto
            {
                UserId = userId.ToString(),
                Items = _mapper.Map<List<CartItemDto>>(cartitems)
            };
            return Result<CartDto>.Success(cartDto);
        }

        

        
    }
}
