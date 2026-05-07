using MyShop.Domain.RepositoryInterfaces;
using AutoMapper;
using MyShop.Application.DTOs.Coupon;
using MyShop.Application.DTOs.Product;
using MyShop.Domain.Entities;
using MyShop.Domain.Enums;
using MyShop.Application.Common.ResultPattern;
using MyShop.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyShop.Domain.Entities.ProductEntities;
using MyShop.Domain.Entities.CouponEntities;
using Microsoft.EntityFrameworkCore;

namespace MyShop.Application.Services
{
    public class CouponService : ICouponService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CouponService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Admin Actions

        public async Task<Result<IEnumerable<CouponDto>>> GetAllCouponsAsync()
        {
            var coupons = await _unitOfWork.Coupons.Query().ToListAsync();
            var dtos = _mapper.Map<IEnumerable<CouponDto>>(coupons);
            return Result<IEnumerable<CouponDto>>.Ok(dtos);
        }

        public async Task<Result<IEnumerable<CouponDto>>> GetActiveCouponsAsync()
        {
            var now = DateTime.UtcNow;
            var coupons = await _unitOfWork.Coupons.Query()
                .Where(c => c.IsActive && (!c.ExpirationDate.HasValue || c.ExpirationDate > now))
                .ToListAsync();
            var dtos = _mapper.Map<IEnumerable<CouponDto>>(coupons);
            return Result<IEnumerable<CouponDto>>.Ok(dtos);
        }

        public async Task<Result<CouponDto>> GetCouponByIdAsync(Guid id)
        {
            var coupon = await _unitOfWork.Coupons.Query().FirstOrDefaultAsync(c => c.Id == id);
            if (coupon == null)
                return Result<CouponDto>.Failure("Coupon not found", ErrorCode.NOT_FOUND);

            var dto = _mapper.Map<CouponDto>(coupon);
            return Result<CouponDto>.Ok(dto);
        }

        public async Task<Result<CouponDto>> GetCouponByCodeAsync(string code)
        {
            var coupon = await _unitOfWork.Coupons.Query().FirstOrDefaultAsync(c => c.CouponCode == code);
            if (coupon == null)
                return Result<CouponDto>.Failure("Coupon not found", ErrorCode.NOT_FOUND);

            var dto = _mapper.Map<CouponDto>(coupon);
            return Result<CouponDto>.Ok(dto);
        }

        public async Task<Result<CouponDto>> CreateCouponAsync(CreateCouponDto createCouponDto)
        {
            if (await _unitOfWork.Coupons.Query().AnyAsync(c => c.CouponCode == createCouponDto.CouponCode))
                return Result<CouponDto>.Failure("Coupon code already exists", ErrorCode.DUPLICATE_ENTRY);

            var coupon = _mapper.Map<Coupon>(createCouponDto);
            await _unitOfWork.Coupons.AddAsync(coupon);
            
            if (await _unitOfWork.CompleteAsync() <= 0)
                return Result<CouponDto>.Failure("Failed to create coupon", ErrorCode.SERVER_ERROR);

            return Result<CouponDto>.Created(_mapper.Map<CouponDto>(coupon), "Coupon created successfully");
        }

        public async Task<Result<CouponDto>> UpdateCouponAsync(Guid id, UpdateCouponDto updateCouponDto)
        {
            var coupon = await _unitOfWork.Coupons.Query(false).FirstOrDefaultAsync(c => c.Id == id);
            if (coupon == null)
                return Result<CouponDto>.Failure("Coupon not found", ErrorCode.NOT_FOUND);

            // Check if code changed and if new code already exists
            if (coupon.CouponCode != updateCouponDto.CouponCode)
            {
                if (await _unitOfWork.Coupons.Query().AnyAsync(c => c.CouponCode == updateCouponDto.CouponCode))
                    return Result<CouponDto>.Failure("New coupon code already exists", ErrorCode.DUPLICATE_ENTRY);
            }

            _mapper.Map(updateCouponDto, coupon);
            
            if (await _unitOfWork.CompleteAsync() <= 0)
                return Result<CouponDto>.Failure("No changes made or update failed", ErrorCode.SERVER_ERROR);

            return Result<CouponDto>.Ok(_mapper.Map<CouponDto>(coupon));
        }

        public async Task<Result<bool>> DeleteCouponAsync(Guid id)
        {
            var coupon = await _unitOfWork.Coupons.Query(false).FirstOrDefaultAsync(c => c.Id == id);
            if (coupon == null)
                return Result<bool>.Failure("Coupon not found", ErrorCode.NOT_FOUND);

            _unitOfWork.Coupons.Delete(coupon);
            return Result<bool>.Ok(await _unitOfWork.CompleteAsync() > 0, "Coupon deleted successfully");
        }

        #endregion

        #region User & Assignment Actions

        public async Task<Result<CouponResponseDto>> ValidateCouponAsync(string code, Guid userId)
        {
            var coupon = await _unitOfWork.Coupons.Query()
                .Include(c => c.ProductCoupons)
                .FirstOrDefaultAsync(c => c.CouponCode == code);

            if (coupon == null || !coupon.IsActive)
                return Result<CouponResponseDto>.Failure("Invalid or inactive coupon", ErrorCode.VALIDATION_ERROR);

            if (coupon.ExpirationDate.HasValue && coupon.ExpirationDate < DateTime.UtcNow)
                return Result<CouponResponseDto>.Failure("Coupon has expired", ErrorCode.VALIDATION_ERROR);

            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
                return Result<CouponResponseDto>.Failure("Coupon usage limit reached", ErrorCode.VALIDATION_ERROR);



            // Check if coupon is assigned to user (if not public - assuming we might want this logic)
            // For now, let's just check if user has reached their personal limit if assigned
            var userCoupon = await _unitOfWork.UserCoupons.Query()
                .FirstOrDefaultAsync(uc => uc.CustomerId == userId && uc.CouponId == coupon.Id);

            if (userCoupon == null || !userCoupon.CanUse)
                return Result<CouponResponseDto>.Failure("You cannot use this coupon", ErrorCode.FORBIDDEN);
            if (userCoupon.UserUsageCount >= userCoupon.UsageLimit)
                return Result<CouponResponseDto>.Failure("You have reached your usage limit for this coupon", ErrorCode.VALIDATION_ERROR);

            var cartItems = await _unitOfWork.CartItems.Query()
                .Where(ci => ci.CustomerId == userId)
                .Include(ci => ci.ProductVariant)
                .ToListAsync();

            if (!cartItems.Any())
                return Result<CouponResponseDto>.Failure("Cart is empty", ErrorCode.VALIDATION_ERROR);

            // Calculate Discount
            var applicableProductIds = coupon.ProductCoupons.Select(pc => pc.ProductVariantId).ToList();
            bool hasRestrictions = applicableProductIds.Any();

            decimal totalOriginal = 0;
            decimal totalDiscount = 0;
            var itemPrices = new Dictionary<Guid, decimal>();

            foreach (var item in cartItems)
            {
                decimal price = item.ProductVariant.NewPrice;
                totalOriginal += price * item.Quantity;

                bool isEligible = !hasRestrictions || applicableProductIds.Contains(item.ProductVariantId);
                decimal discountPerItem = 0;

                if (isEligible)
                {
                    if (coupon.DiscountType == DiscountType.Percentage)
                        discountPerItem = price * (coupon.DiscountValue / 100.0m);
                    else
                        discountPerItem = Math.Min(price, coupon.DiscountValue);
                }

                totalDiscount += discountPerItem * item.Quantity;
                itemPrices[item.ProductVariantId] = price - discountPerItem;
            }

            if (totalOriginal < coupon.MinAmount)
                return Result<CouponResponseDto>.Failure($"Minimum amount {coupon.MinAmount} not met", ErrorCode.VALIDATION_ERROR);

            return Result<CouponResponseDto>.Ok(new CouponResponseDto
            {
                Coupon = _mapper.Map<CouponDto>(coupon),
                TotalDiscount = totalDiscount,
                FinalSubtotal = totalOriginal - totalDiscount,
                ItemPrices = itemPrices
            });
        }

        public async Task<Result<IEnumerable<UserCouponDto>>> GetMyCouponsAsync(Guid userId)
        {
            var userCoupons = await _unitOfWork.UserCoupons.Query()
                .Where(uc => uc.CustomerId == userId)
                .Include(uc => uc.Coupon)
                .Include(uc => uc.Customer)
                    .ThenInclude(c => c.User)
                .ToListAsync();
            
            var dtos = _mapper.Map<IEnumerable<UserCouponDto>>(userCoupons);
            return Result<IEnumerable<UserCouponDto>>.Ok(dtos);
        }

        public async Task<Result<bool>> AssignCouponToUserAsync(AssignCouponDto dto)
        {
            var existing = await _unitOfWork.UserCoupons.Query(false)
                .FirstOrDefaultAsync(uc => uc.CustomerId == dto.UserId && uc.CouponId == dto.CouponId);

            if (existing != null)
            {
                existing.UsageLimit = dto.UsageLimit;
                return Result<bool>.Ok(await _unitOfWork.CompleteAsync() >= 0);
            }

            await _unitOfWork.UserCoupons.AddAsync(new UserCoupon
            {
                CustomerId = dto.UserId,
                CouponId = dto.CouponId,
                AssignedAt = DateTime.UtcNow,
                CanUse = true,
                UsageLimit = dto.UsageLimit
            });

            return Result<bool>.Ok(await _unitOfWork.CompleteAsync() > 0);
        }

        public async Task<Result<bool>> RemoveCouponFromUserAsync(Guid couponId, Guid userId)
        {
            var userCoupon = await _unitOfWork.UserCoupons.Query(false)
                .FirstOrDefaultAsync(uc => uc.CustomerId == userId && uc.CouponId == couponId);

            if (userCoupon == null)
                return Result<bool>.Failure("Assignment not found", ErrorCode.NOT_FOUND);

            _unitOfWork.UserCoupons.Delete(userCoupon);
            return Result<bool>.Ok(await _unitOfWork.CompleteAsync() > 0);
        }

        public async Task<Result<BulkAssignResultDto>> BulkAssignCouponAsync(BulkAssignCouponDto dto)
        {
            var coupon = await _unitOfWork.Coupons.Query().FirstOrDefaultAsync(c => c.Id == dto.CouponId);
            if (coupon == null)
                return Result<BulkAssignResultDto>.Failure("Coupon not found", ErrorCode.NOT_FOUND);

            IEnumerable<Guid> targetUserIds;
            if (dto.UserIds != null && dto.UserIds.Any())
            {
                targetUserIds = dto.UserIds;
            }
            else
            {
                var allCustomers = await _unitOfWork.Customers.Query().ToListAsync();
                targetUserIds = allCustomers.Select(c => c.UserId);
            }

            int alreadyAssigned = 0;
            int newlyAssigned = 0;

            foreach (var userId in targetUserIds)
            {
                var exists = await _unitOfWork.UserCoupons.Query(false)
                    .FirstOrDefaultAsync(uc => uc.CustomerId == userId && uc.CouponId == dto.CouponId);

                if (exists != null)
                {
                    exists.UsageLimit = dto.UsageLimit;
                    alreadyAssigned++;
                }
                else
                {
                    await _unitOfWork.UserCoupons.AddAsync(new UserCoupon
                    {
                        CustomerId = userId,
                        CouponId = dto.CouponId,
                        AssignedAt = DateTime.UtcNow,
                        CanUse = true,
                        UsageLimit = dto.UsageLimit
                    });
                    newlyAssigned++;
                }
            }

            await _unitOfWork.CompleteAsync();

            return Result<BulkAssignResultDto>.Ok(new BulkAssignResultDto
            {
                TotalProcessed = targetUserIds.Count(),
                AlreadyAssigned = alreadyAssigned,
                NewlyAssigned = newlyAssigned
            });
        }

        public async Task<Result<IEnumerable<UserCouponDto>>> GetCouponUsersAsync(Guid couponId)
        {
            var userCoupons = await _unitOfWork.UserCoupons.Query()
                .Where(uc => uc.CouponId == couponId)
                .Include(uc => uc.Customer)
                    .ThenInclude(c => c.User)
                .Include(uc => uc.Coupon)
                .ToListAsync();
            
            var dtos = _mapper.Map<IEnumerable<UserCouponDto>>(userCoupons);
            return Result<IEnumerable<UserCouponDto>>.Ok(dtos);
        }

        #endregion

        #region Product Actions

        public async Task<Result<IEnumerable<ProductDto>>> GetAssignedProductsAsync(Guid couponId)
        {
            var productCoupons = await _unitOfWork.ProductCoupons.Query()
                .Where(pc => pc.CouponId == couponId)
                .Include(pc => pc.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                .ToListAsync();
            
            var products = productCoupons.Select(pc => pc.ProductVariant.Product).Distinct();
            var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Result<IEnumerable<ProductDto>>.Ok(dtos);
        }

        public async Task<Result<bool>> AssignCouponToProductsAsync(Guid couponId, IEnumerable<Guid> productIds)
        {
            var coupon = await _unitOfWork.Coupons.Query().FirstOrDefaultAsync(c => c.Id == couponId);
            if (coupon == null) return Result<bool>.Failure("Coupon not found", ErrorCode.NOT_FOUND);

            foreach (var productId in productIds)
            {
                var variants = await _unitOfWork.ProductVariants.Query().Where(v => v.ProductId == productId).ToListAsync();
                foreach (var variant in variants)
                {
                    var exists = await _unitOfWork.ProductCoupons.Query()
                        .FirstOrDefaultAsync(pc => pc.CouponId == coupon.Id && pc.ProductVariantId == variant.Id);

                    if (exists == null)
                    {
                        await _unitOfWork.ProductCoupons.AddAsync(new ProductCoupon
                        {
                            CouponId = coupon.Id,
                            ProductVariantId = variant.Id
                        });
                    }
                }
            }

            return Result<bool>.Ok(await _unitOfWork.CompleteAsync() > 0);
        }

        public async Task<Result<bool>> RemoveCouponFromProductsAsync(Guid couponId, IEnumerable<Guid> productIds)
        {
            foreach (var productId in productIds)
            {
                var variants = await _unitOfWork.ProductVariants.Query().Where(v => v.ProductId == productId).ToListAsync();
                foreach (var variant in variants)
                {
                    var pc = await _unitOfWork.ProductCoupons.Query(false)
                        .FirstOrDefaultAsync(x => x.CouponId == couponId && x.ProductVariantId == variant.Id);

                    if (pc != null)
                        _unitOfWork.ProductCoupons.Delete(pc);
                }
            }
            return Result<bool>.Ok(await _unitOfWork.CompleteAsync() > 0);
        }

        #endregion
    }
}
