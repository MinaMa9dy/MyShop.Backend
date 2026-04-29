using AutoMapper;
using MyShop.CORE.DTOs.Coupon;
using MyShop.CORE.Dtos.Product;
using MyShop.CORE.Entities;
using MyShop.CORE.Enums;
using MyShop.CORE.Helpers.ResultPattern;
using MyShop.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyShop.CORE.Implmentations
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
            var coupons = await _unitOfWork.Coupons.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<CouponDto>>(coupons);
            return Result<IEnumerable<CouponDto>>.Success(dtos);
        }

        public async Task<Result<IEnumerable<CouponDto>>> GetActiveCouponsAsync()
        {
            var now = DateTime.UtcNow;
            var coupons = await _unitOfWork.Coupons.FindAllAsync(c => 
                c.IsActive && (!c.ExpirationDate.HasValue || c.ExpirationDate > now));
            var dtos = _mapper.Map<IEnumerable<CouponDto>>(coupons);
            return Result<IEnumerable<CouponDto>>.Success(dtos);
        }

        public async Task<Result<CouponDto>> GetCouponByIdAsync(Guid id)
        {
            var coupon = await _unitOfWork.Coupons.FindAsync(c => c.Id == id);
            if (coupon == null)
                return Result<CouponDto>.Failure("Coupon not found", "404");

            var dto = _mapper.Map<CouponDto>(coupon);
            return Result<CouponDto>.Success(dto);
        }

        public async Task<Result<CouponDto>> GetCouponByCodeAsync(string code)
        {
            var coupon = await _unitOfWork.Coupons.FindAsync(c => c.CouponCode == code);
            if (coupon == null)
                return Result<CouponDto>.Failure("Coupon not found", "404");

            var dto = _mapper.Map<CouponDto>(coupon);
            return Result<CouponDto>.Success(dto);
        }

        public async Task<Result<CouponDto>> CreateCouponAsync(CreateCouponDto createCouponDto)
        {
            if (await _unitOfWork.Coupons.FindAsync(c => c.CouponCode == createCouponDto.CouponCode) != null)
                return Result<CouponDto>.Failure("Coupon code already exists", "400");

            var coupon = _mapper.Map<Coupon>(createCouponDto);
            await _unitOfWork.Coupons.AddAsync(coupon);
            
            if (await _unitOfWork.CompleteAsync() <= 0)
                return Result<CouponDto>.Failure("Failed to create coupon", "500");

            return Result<CouponDto>.Success(_mapper.Map<CouponDto>(coupon));
        }

        public async Task<Result<CouponDto>> UpdateCouponAsync(Guid id, UpdateCouponDto updateCouponDto)
        {
            var coupon = await _unitOfWork.Coupons.FindAsync(c => c.Id == id);
            if (coupon == null)
                return Result<CouponDto>.Failure("Coupon not found", "404");

            // Check if code changed and if new code already exists
            if (coupon.CouponCode != updateCouponDto.CouponCode)
            {
                if (await _unitOfWork.Coupons.FindAsync(c => c.CouponCode == updateCouponDto.CouponCode) != null)
                    return Result<CouponDto>.Failure("New coupon code already exists", "400");
            }

            _mapper.Map(updateCouponDto, coupon);
            
            if (await _unitOfWork.CompleteAsync() <= 0)
                return Result<CouponDto>.Failure("No changes made or update failed", "500");

            return Result<CouponDto>.Success(_mapper.Map<CouponDto>(coupon));
        }

        public async Task<Result<bool>> DeleteCouponAsync(Guid id)
        {
            var coupon = await _unitOfWork.Coupons.FindAsync(c => c.Id == id);
            if (coupon == null)
                return Result<bool>.Failure("Coupon not found", "404");

            _unitOfWork.Coupons.Delete(coupon);
            return Result<bool>.Success(await _unitOfWork.CompleteAsync() > 0);
        }

        #endregion

        #region User & Assignment Actions

        public async Task<Result<CouponResponseDto>> ValidateCouponAsync(string code, Guid userId)
        {
            var coupon = await _unitOfWork.Coupons.FindAsync(c => c.CouponCode == code, includes: new[] { "ProductCoupons" });
            if (coupon == null || !coupon.IsActive)
                return Result<CouponResponseDto>.Failure("Invalid or inactive coupon", "400");

            if (coupon.ExpirationDate.HasValue && coupon.ExpirationDate < DateTime.UtcNow)
                return Result<CouponResponseDto>.Failure("Coupon has expired", "400");

            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
                return Result<CouponResponseDto>.Failure("Coupon usage limit reached", "400");

            // Check if coupon is assigned to user (if not public - assuming we might want this logic)
            // For now, let's just check if user has reached their personal limit if assigned
            var userCoupon = await _unitOfWork.UserCoupons.FindAsync(uc => uc.CustomerId == userId && uc.CouponId == coupon.Id);
            if (userCoupon == null || !userCoupon.CanUse)
                return Result<CouponResponseDto>.Failure("You cannot use this coupon", "400");

            var cartItems = await _unitOfWork.CartItems.FindAllAsync(ci => ci.CustomerId == userId, includes: new[] { nameof(CartItem.ProductVariant) });
            if (!cartItems.Any())
                return Result<CouponResponseDto>.Failure("Cart is empty", "400");

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
                return Result<CouponResponseDto>.Failure($"Minimum amount {coupon.MinAmount} not met", "400");

            return Result<CouponResponseDto>.Success(new CouponResponseDto
            {
                Coupon = _mapper.Map<CouponDto>(coupon),
                TotalDiscount = totalDiscount,
                FinalSubtotal = totalOriginal - totalDiscount,
                ItemPrices = itemPrices
            });
        }

        public async Task<Result<IEnumerable<UserCouponDto>>> GetMyCouponsAsync(Guid userId)
        {
            var userCoupons = await _unitOfWork.UserCoupons.FindAllAsync(
                uc => uc.CustomerId == userId, 
                includes: new[] { "Coupon", "Customer.User" });
            
            var dtos = _mapper.Map<IEnumerable<UserCouponDto>>(userCoupons);
            return Result<IEnumerable<UserCouponDto>>.Success(dtos);
        }

        public async Task<Result<bool>> AssignCouponToUserAsync(AssignCouponDto dto)
        {
            var existing = await _unitOfWork.UserCoupons.FindAsync(uc => uc.CustomerId == dto.UserId && uc.CouponId == dto.CouponId);
            if (existing != null)
                return Result<bool>.Failure("Coupon already assigned to this user", "400");

            await _unitOfWork.UserCoupons.AddAsync(new UserCoupon
            {
                CustomerId = dto.UserId,
                CouponId = dto.CouponId,
                AssignedAt = DateTime.UtcNow,
                CanUse = true
            });

            return Result<bool>.Success(await _unitOfWork.CompleteAsync() > 0);
        }

        public async Task<Result<bool>> RemoveCouponFromUserAsync(Guid couponId, Guid userId)
        {
            var userCoupon = await _unitOfWork.UserCoupons.FindAsync(uc => uc.CustomerId == userId && uc.CouponId == couponId);
            if (userCoupon == null)
                return Result<bool>.Failure("Assignment not found", "404");

            _unitOfWork.UserCoupons.Delete(userCoupon);
            return Result<bool>.Success(await _unitOfWork.CompleteAsync() > 0);
        }

        public async Task<Result<BulkAssignResultDto>> BulkAssignCouponAsync(BulkAssignCouponDto dto)
        {
            var coupon = await _unitOfWork.Coupons.FindAsync(c => c.Id == dto.CouponId);
            if (coupon == null)
                return Result<BulkAssignResultDto>.Failure("Coupon not found", "404");

            IEnumerable<Guid> targetUserIds;
            if (dto.UserIds != null && dto.UserIds.Any())
            {
                targetUserIds = dto.UserIds;
            }
            else
            {
                var allCustomers = await _unitOfWork.Customers.GetAllAsync();
                targetUserIds = allCustomers.Select(c => c.UserId);
            }

            int alreadyAssigned = 0;
            int newlyAssigned = 0;

            foreach (var userId in targetUserIds)
            {
                var exists = await _unitOfWork.UserCoupons.FindAsync(uc => uc.CustomerId == userId && uc.CouponId == dto.CouponId);
                if (exists != null)
                {
                    alreadyAssigned++;
                }
                else
                {
                    await _unitOfWork.UserCoupons.AddAsync(new UserCoupon
                    {
                        CustomerId = userId,
                        CouponId = dto.CouponId,
                        AssignedAt = DateTime.UtcNow,
                        CanUse = true
                    });
                    newlyAssigned++;
                }
            }

            await _unitOfWork.CompleteAsync();

            return Result<BulkAssignResultDto>.Success(new BulkAssignResultDto
            {
                TotalProcessed = targetUserIds.Count(),
                AlreadyAssigned = alreadyAssigned,
                NewlyAssigned = newlyAssigned
            });
        }

        public async Task<Result<IEnumerable<UserCouponDto>>> GetCouponUsersAsync(Guid couponId)
        {
            var userCoupons = await _unitOfWork.UserCoupons.FindAllAsync(
                uc => uc.CouponId == couponId, 
                includes: new[] { "Customer.User", "Coupon" });
            
            var dtos = _mapper.Map<IEnumerable<UserCouponDto>>(userCoupons);
            return Result<IEnumerable<UserCouponDto>>.Success(dtos);
        }

        #endregion

        #region Product Actions

        public async Task<Result<IEnumerable<ProductDto>>> GetAssignedProductsAsync(Guid couponId)
        {
            var productCoupons = await _unitOfWork.ProductCoupons.FindAllAsync(
                pc => pc.CouponId == couponId, 
                includes: new[] { "ProductVariant.Product" });
            
            var products = productCoupons.Select(pc => pc.ProductVariant.Product).Distinct();
            var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Result<IEnumerable<ProductDto>>.Success(dtos);
        }

        public async Task<Result<bool>> AssignCouponToProductsAsync(Guid couponId, IEnumerable<Guid> productIds)
        {
            var coupon = await _unitOfWork.Coupons.FindAsync(c => c.Id == couponId);
            if (coupon == null) return Result<bool>.Failure("Coupon not found", "404");

            foreach (var productId in productIds)
            {
                // Note: Assuming we link to variants. If linking to product, we might need a different table or logic.
                // Currently ProductCoupon has ProductVariantId.
                var variants = await _unitOfWork.ProductVariants.FindAllAsync(v => v.ProductId == productId);
                foreach (var variant in variants)
                {
                    var exists = await _unitOfWork.ProductCoupons.FindAsync(pc => pc.CouponId == coupon.Id && pc.ProductVariantId == variant.Id);
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

            return Result<bool>.Success(await _unitOfWork.CompleteAsync() > 0);
        }

        public async Task<Result<bool>> RemoveCouponFromProductsAsync(Guid couponId, IEnumerable<Guid> productIds)
        {
            foreach (var productId in productIds)
            {
                var variants = await _unitOfWork.ProductVariants.FindAllAsync(v => v.ProductId == productId);
                foreach (var variant in variants)
                {
                    var pc = await _unitOfWork.ProductCoupons.FindAsync(x => x.CouponId == couponId && x.ProductVariantId == variant.Id);
                    if (pc != null)
                        _unitOfWork.ProductCoupons.Delete(pc);
                }
            }
            return Result<bool>.Success(await _unitOfWork.CompleteAsync() > 0);
        }

        #endregion
    }
}
