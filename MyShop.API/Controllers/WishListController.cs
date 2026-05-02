using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.CORE.Dtos.Wish;
using MyShop.CORE.Entities;
using MyShop.CORE.Interfaces;
using System.Security.Claims;

namespace MyShop.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WishListController : ControllerBase
    {
        private readonly IWishService _wishService;
        public WishListController(IWishService wishService)
        {
            _wishService = wishService;


        }
        [HttpPost]
        public async Task<IActionResult> AddWish(WishDto wish)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _wishService.AddWish(userId,wish);
            if (!result.IsSuccess)
            {
                return StatusCode(int.Parse(result.Error.Code), result);
            }
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetWishesByUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _wishService.GetWishesByUserId(userId);
            if (!result.IsSuccess)
            {
                return StatusCode(int.Parse(result.Error.Code), result);
            }
            return Ok(result);
        }
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveWish(Guid productId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var wishDto = new WishDto { ProductId = productId };
            var result = await _wishService.RemoveWish(userId,wishDto);
            if (!result.IsSuccess)
            {
                return StatusCode(int.Parse(result.Error.Code), result);
            }
            return Ok(result);
        }
    }
}
