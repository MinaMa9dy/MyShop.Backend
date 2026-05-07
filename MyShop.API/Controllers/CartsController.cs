using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.Application.DTOs.CartItem;
using MyShop.Domain.Entities;
using MyShop.Application.Interfaces;

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
            return StatusCode(result.Status, result);
        }
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _cartService.ClearCartAsync(userId);
            return StatusCode(result.Status, result);
        }
    }
}
