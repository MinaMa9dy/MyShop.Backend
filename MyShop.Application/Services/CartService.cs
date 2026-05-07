using MyShop.Domain.RepositoryInterfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.Application.DTOs.Cart;
using MyShop.Application.DTOs.CartItem;
using MyShop.Application.DTOs.Product;
using MyShop.Domain.Entities;
using MyShop.Domain.Enums;
using MyShop.Application.Common;
using MyShop.Application.Common.ResultPattern;
using MyShop.Domain.Identity;
using MyShop.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Services
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
            var cartItems = await _unitOfWork.CartItems.Query().Where(c => c.CustomerId == userId).ToListAsync();
            _unitOfWork.CartItems.DeleteRange(cartItems);
            await _unitOfWork.CompleteAsync();
            return Result<bool>.Ok(true);

        }

        public Task<Result<IEnumerable<CartDto>>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        

        public async Task<Result<CartDto>> GetByUserIdAsync(Guid userId)
        {
            
            var cartitems = await _unitOfWork.CartItems.Query()
                .Where(c => c.CustomerId == userId)
                .Include(c => c.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                        .ThenInclude(p => p.productPhotos)
                .Include(c => c.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                        .ThenInclude(p => p.Category)
                .Include(c => c.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                        .ThenInclude(p => p.Supplier)
                            .ThenInclude(s => s.User)
                .ToListAsync();
            
            var cartDto = new CartDto
            {
                UserId = userId.ToString(),
                Items = _mapper.Map<List<CartItemDto>>(cartitems)
            };
            return Result<CartDto>.Ok(cartDto);
        }

        
    }
}
