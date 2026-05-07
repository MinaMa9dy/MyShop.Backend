using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.Application.DTOs.CartItem;

using MyShop.Application.Interfaces;
using System.Security.Claims;

namespace MyShop.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemsService _cartItemsService;
        public CartItemsController(ICartItemsService cartItemsService)
        {
            _cartItemsService = cartItemsService;
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] CartItemCreateDto cartItem)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _cartItemsService.AddToCartAsync(userId, cartItem);
            return StatusCode(result.Status, result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto cartItem)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _cartItemsService.UpdateQuantityAsync(userId, cartItem);
            return StatusCode(result.Status, result);

        }
        [HttpDelete]
        public async Task<IActionResult> RemoveFromCart(Guid cartItemId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _cartItemsService.RemoveFromCartAsync(userId, cartItemId);
            return StatusCode(result.Status, result);
        }
    }
}
