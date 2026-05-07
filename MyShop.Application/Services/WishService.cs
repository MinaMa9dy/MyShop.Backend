using MyShop.Domain.RepositoryInterfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MyShop.Application.DTOs.Wish;
using MyShop.Domain.Entities;
using MyShop.Domain.Enums;
using MyShop.Application.Common.ResultPattern;
using MyShop.Domain.Identity;
using MyShop.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyShop.Application.Services
{
    public class WishService : IWishService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        public WishService(IUnitOfWork unitOfWork, IMapper mapper, IIdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _identityService = identityService;
        }
        public async Task<Result<WishDto>> AddWish(Guid userId, WishDto wish)
        {
            if (wish == null)
            {
                return Result<WishDto>.Failure("Wish is null", ErrorCode.VALIDATION_ERROR);
            }
            var user = await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Id == userId);
            if (user is null)
            {
                return Result<WishDto>.Failure("User not found", ErrorCode.NOT_FOUND);
            }
            
            if (!await _identityService.IsInRoleAsync(user.Id.ToString(),nameof(RoleOptions.Customer)))
               return Result<WishDto>.Failure("User is not a customer", ErrorCode.FORBIDDEN);

            var product = await _unitOfWork.Products.Query().FirstOrDefaultAsync(p => p.Id == wish.ProductId);
            if (product == null)
            {
                return Result<WishDto>.Failure("Product not found", ErrorCode.NOT_FOUND);
            }
            var Exist = await _unitOfWork.Wishes.Query().FirstOrDefaultAsync(w => w.ProductId == wish.ProductId && w.CustomerId == userId);
            if (Exist != null)
            {
                return Result<WishDto>.Ok(_mapper.Map<WishDto>(wish));
            }
            var wishDto = _mapper.Map<WishList>(wish);
            wishDto.CustomerId = userId;
            await _unitOfWork.Wishes.AddAsync(wishDto);
            var result = await _unitOfWork.CompleteAsync();
            if(result == 0)
            {
                return Result<WishDto>.Failure("Failed to save The Wish", ErrorCode.SERVER_ERROR);
            }
            return Result<WishDto>.Created(_mapper.Map<WishDto>(wish));

        }

        public async Task<Result<List<WishDto>>> GetWishesByUserId(Guid UserId)
        {
            if(UserId == null)
            {
                return Result<List<WishDto>>.Failure("UserId is null", ErrorCode.VALIDATION_ERROR);
            }
            if(await _unitOfWork.Users.Query().FirstOrDefaultAsync(u => u.Id == UserId) is null)
            {
                return Result<List<WishDto>>.Failure("User not found", ErrorCode.NOT_FOUND);
            }
            var wishes = await _unitOfWork.Wishes.Query()
                .Where(u => u.CustomerId == UserId)
                .Include(u => u.Product)
                    .ThenInclude(p => p.productPhotos)
                .ToListAsync();
            var wishesDto = _mapper.Map<List<WishDto>>(wishes);
            return Result<List<WishDto>>.Ok(wishesDto);

        }

        public async Task<Result<bool>> RemoveWish(Guid userId, WishDto wishDto)
        {
            if (wishDto == null)
                return Result<bool>.Failure("Wish is null", ErrorCode.VALIDATION_ERROR);

            if (userId == Guid.Empty || wishDto.ProductId == Guid.Empty)
                return Result<bool>.Failure("UserId or ProductId is null", ErrorCode.VALIDATION_ERROR);

            var wish = await _unitOfWork.Wishes.Query(false)
                .FirstOrDefaultAsync(w => w.ProductId == wishDto.ProductId &&
                                w.CustomerId == userId);

            if (wish == null)
                return Result<bool>.Ok(true);

            _unitOfWork.Wishes.Delete(wish);

            var result = await _unitOfWork.CompleteAsync();

            if (result == 0)
                return Result<bool>.Failure("Failed to remove the wish", ErrorCode.SERVER_ERROR);

            return Result<bool>.Ok(true);
        }
    }
}
