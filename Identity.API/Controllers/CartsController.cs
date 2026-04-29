using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.CORE.Dtos.CartItem;
using MyShop.CORE.Entities;
using MyShop.CORE.Interfaces;

using System.Security.Claims;

namespace MyShop.API.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;
        
        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
            

        }
        [HttpGet("my-cart")]
        public async Task<IActionResult> GetMyCartItems()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _cartService.GetByUserIdAsync(userId);
            if (!result.IsSuccess)
            {
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            }
            return Ok(result.Data);
        }
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _cartService.ClearCartAsync(userId);
            if (!result.IsSuccess)
            {
                return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            }
            return Ok(result.Data);
        }
        
        


    }
}
