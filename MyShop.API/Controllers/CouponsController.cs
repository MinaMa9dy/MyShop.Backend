using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.CORE.DTOs.Coupon;
using MyShop.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        #region Admin Endpoints
        
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _couponService.GetAllCouponsAsync();
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Active")]
        public async Task<IActionResult> GetActive()
        {
            var result = await _couponService.GetActiveCouponsAsync();
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _couponService.GetCouponByIdAsync(id);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ByCode/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await _couponService.GetCouponByCodeAsync(code);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateCouponDto dto)
        {
            var result = await _couponService.CreateCouponAsync(dto);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCouponDto dto)
        {
            var result = await _couponService.UpdateCouponAsync(id, dto);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _couponService.DeleteCouponAsync(id);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AssignToUser")]
        public async Task<IActionResult> AssignToUser([FromBody] AssignCouponDto dto)
        {
            var result = await _couponService.AssignCouponToUserAsync(dto);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("RemoveFromUser/{couponId}/{userId}")]
        public async Task<IActionResult> RemoveFromUser(Guid couponId, Guid userId)
        {
            var result = await _couponService.RemoveCouponFromUserAsync(couponId, userId);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("BulkAssign")]
        public async Task<IActionResult> BulkAssign([FromBody] BulkAssignCouponDto dto)
        {
            var result = await _couponService.BulkAssignCouponAsync(dto);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Users/{couponId}")]
        public async Task<IActionResult> GetCouponUsers(Guid couponId)
        {
            var result = await _couponService.GetCouponUsersAsync(couponId);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAssignedProducts/{couponId}")]
        public async Task<IActionResult> GetAssignedProducts(Guid couponId)
        {
            var result = await _couponService.GetAssignedProductsAsync(couponId);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AssignProducts/{couponId}")]
        public async Task<IActionResult> AssignProducts(Guid couponId, [FromBody] IEnumerable<Guid> productIds)
        {
            var result = await _couponService.AssignCouponToProductsAsync(couponId, productIds);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("RemoveProducts/{couponId}")]
        public async Task<IActionResult> RemoveProducts(Guid couponId, [FromBody] IEnumerable<Guid> productIds)
        {
            var result = await _couponService.RemoveCouponFromProductsAsync(couponId, productIds);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        #endregion

        #region User Endpoints

        [Authorize]
        [HttpGet("MyCoupons")]
        public async Task<IActionResult> GetMyCoupons()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                return Unauthorized();

            var result = await _couponService.GetMyCouponsAsync(userId);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("Validate/{code}")]
        public async Task<IActionResult> Validate(string code)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId))
                return Unauthorized();

            var result = await _couponService.ValidateCouponAsync(code, userId);
            if (!result.IsSuccess)
                return StatusCode(int.Parse(result.Error.Code), result);

            return Ok(result);
        }

        #endregion
    }
}
