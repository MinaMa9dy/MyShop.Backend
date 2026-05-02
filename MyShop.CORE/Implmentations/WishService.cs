using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MyShop.CORE.Dtos.Wish;
using MyShop.CORE.Entities;
using MyShop.CORE.Enums;
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
                return Result<WishDto>.Failure("Wish is null", "400");
            }
            var user = await _unitOfWork.Users.FindAsync(u => u.Id == userId);
            if (user is null)
            {
                return Result<WishDto>.Failure("User not found", "400");
            }
            
            if (!await _identityService.IsInRoleAsync(user.Id.ToString(),nameof(RoleOptions.Customer)))
               return Result<WishDto>.Failure("User is not a customer", "400");

            var product = await _unitOfWork.Products.FindAsync(p => p.Id == wish.ProductId);
            if (product == null)
            {
                return Result<WishDto>.Failure("Product not found", "400");
            }
            var Exist = await _unitOfWork.Wishes.FindAsync(w => w.ProductId == wish.ProductId && w.CustomerId == userId);
            if (Exist != null)
            {
                return Result<WishDto>.Success(_mapper.Map<WishDto>(wish));
            }
            var wishDto = _mapper.Map<WishList>(wish);
            wishDto.CustomerId = userId;
            await _unitOfWork.Wishes.AddAsync(wishDto);
            var result = await _unitOfWork.CompleteAsync();
            if(result == 0)
            {
                return Result<WishDto>.Failure("Failed to save The Wish", "500");
            }
            return Result<WishDto>.Success(_mapper.Map<WishDto>(wish));

        }

        public async Task<Result<List<WishDto>>> GetWishesByUserId(Guid UserId)
        {
            if(UserId == null)
            {
                return Result<List<WishDto>>.Failure("UserId is null", "400");
            }
            if(await _unitOfWork.Users.FindAsync(u => u.Id == UserId) is null)
            {
                return Result<List<WishDto>>.Failure("User not found", "400");
            }
            var wishes = (List<WishList>)await _unitOfWork.Wishes.FindAllAsync(u => u.CustomerId == UserId, includes: new[] { nameof(WishList.Product), "Product.productPhotos" });
            var wishesDto = _mapper.Map<List<WishDto>>(wishes);
            return Result<List<WishDto>>.Success(wishesDto);

        }

        public async Task<Result<bool>> RemoveWish(Guid userId, WishDto wishDto)
        {
            if (wishDto == null)
                return Result<bool>.Failure("Wish is null", "400");

            if (userId == Guid.Empty || wishDto.ProductId == Guid.Empty)
                return Result<bool>.Failure("UserId or ProductId is null", "400");

            var wish = await _unitOfWork.Wishes
                .FindAsync(w => w.ProductId == wishDto.ProductId &&
                                w.CustomerId == userId);

            if (wish == null)
                return Result<bool>.Success(true);

            _unitOfWork.Wishes.Delete(wish);

            var result = await _unitOfWork.CompleteAsync();

            if (result == 0)
                return Result<bool>.Failure("Failed to remove the wish", "500");

            return Result<bool>.Success(true);
        }
    }
}
