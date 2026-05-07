using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShop.Application.DTOs.Order;
using MyShop.Domain.Entities;
using MyShop.Application.Interfaces;
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
            return StatusCode(result.Status, result);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            
            var result = await _orderService.GetOrdersByCustomerId(userId);
            return StatusCode(result.Status, result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var result = await _orderService.GetOrderByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(AddOrderDto order)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _orderService.CreateOrder(userId, order);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles="Seller")]
        [HttpGet("SellerOrders")]
        public async Task<IActionResult> GetOrdersBySellerId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _orderService.GetOrdersBySellerId(userId);
            return StatusCode(result.Status, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(dto);
            return StatusCode(result.Status, result);
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (userIdClaim == null) return Unauthorized();
            var userId = Guid.Parse(userIdClaim);
            var result = await _orderService.CancelOrderAsync(userId, id);
            return StatusCode(result.Status, result);
        }
    }
}
