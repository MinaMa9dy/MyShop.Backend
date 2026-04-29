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
        public async Task<IActionResult> AddWish(WishDto? wish)
        {
            var result = await _wishService.AddWish(wish);
            if (!result.IsSuccess)
            {
                return StatusCode(int.Parse(result.Error.Code), result);
            }
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetWishes(Guid userId)
        {
            var result = await _wishService.GetWishesByUserId(userId);
            if (!result.IsSuccess)
            {
                return StatusCode(int.Parse(result.Error.Code), result);
            }
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveWish([FromQuery] Guid userId, [FromQuery] Guid productId)
        {
            var wishDto = new WishDto { CustomerId = userId, ProductId = productId };
            var result = await _wishService.RemoveWish(wishDto);
            if (!result.IsSuccess)
            {
                return StatusCode(int.Parse(result.Error.Code), result);
            }
            return Ok(result);
        }
    }
}
