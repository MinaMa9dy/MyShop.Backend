using AutoMapper;
using Identity.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.CORE.Dtos.Order;
using MyShop.CORE.Entities;
using MyShop.CORE.Interfaces;
using System.Security.Claims;

namespace MyShop.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.GetAllOrdersAsync(page, pageSize);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            return Ok(result);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            
            var result = await _orderService.GetOrdersByUserId(userId);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(AddOrderDto? order)
        {
            if (order == null) return BadRequest(new { message = "Order cannot be null" });
            
            var result = await _orderService.CreateOrder(order);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            return Ok(result);
        }

        [Authorize(Roles="Seller")]
        [HttpGet("SellerOrders")]
        public async Task<IActionResult> GetOrdersBySellerId([FromQuery] Guid sellerId)
        {
            var result = await _orderService.GetOrdersBySellerId(sellerId);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, dto.Status);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            return Ok(result);
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var result = await _orderService.CancelOrderAsync(id);
            if (!result.IsSuccess) return StatusCode(int.Parse(result.Error.Code), result.Error.Message);
            return Ok(result);
        }
    }
}
