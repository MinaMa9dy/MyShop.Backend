using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.Application.DTOs.Wish;
using MyShop.Domain.Entities;
using MyShop.Application.Interfaces;
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
            return StatusCode(result.Status, result);
        }
        [HttpGet]
        public async Task<IActionResult> GetWishesByUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _wishService.GetWishesByUserId(userId);
            return StatusCode(result.Status, result);
        }
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveWish(Guid productId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var wishDto = new WishDto { ProductId = productId };
            var result = await _wishService.RemoveWish(userId,wishDto);
            return StatusCode(result.Status, result);
        }
    }
}
