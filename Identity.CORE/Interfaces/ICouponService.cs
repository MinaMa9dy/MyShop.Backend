using MyShop.CORE.DTOs.Coupon;
using MyShop.CORE.Dtos.Product;
using MyShop.CORE.Helpers.ResultPattern;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyShop.CORE.Interfaces
{
    public interface ICouponService
    {
        // Admin Actions
        Task<Result<IEnumerable<CouponDto>>> GetAllCouponsAsync();
        Task<Result<IEnumerable<CouponDto>>> GetActiveCouponsAsync();
        Task<Result<CouponDto>> GetCouponByIdAsync(Guid id);
        Task<Result<CouponDto>> GetCouponByCodeAsync(string code);
        Task<Result<CouponDto>> CreateCouponAsync(CreateCouponDto createCouponDto);
        Task<Result<CouponDto>> UpdateCouponAsync(Guid id, UpdateCouponDto updateCouponDto);
        Task<Result<bool>> DeleteCouponAsync(Guid id);
        
        // User & Assignment Actions
        Task<Result<CouponResponseDto>> ValidateCouponAsync(string code, Guid userId);
        Task<Result<IEnumerable<UserCouponDto>>> GetMyCouponsAsync(Guid userId);
        Task<Result<bool>> AssignCouponToUserAsync(AssignCouponDto dto);
        Task<Result<bool>> RemoveCouponFromUserAsync(Guid couponId, Guid userId);
        Task<Result<BulkAssignResultDto>> BulkAssignCouponAsync(BulkAssignCouponDto dto);
        Task<Result<IEnumerable<UserCouponDto>>> GetCouponUsersAsync(Guid couponId);

        // Product Actions (Keeping for existing functionality)
        Task<Result<IEnumerable<ProductDto>>> GetAssignedProductsAsync(Guid couponId);
        Task<Result<bool>> AssignCouponToProductsAsync(Guid couponId, IEnumerable<Guid> productIds);
        Task<Result<bool>> RemoveCouponFromProductsAsync(Guid couponId, IEnumerable<Guid> productIds);
    }
}
