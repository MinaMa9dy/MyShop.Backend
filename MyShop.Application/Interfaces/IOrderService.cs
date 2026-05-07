using MyShop.Application.DTOs.Order;
using MyShop.Domain.Entities;
using MyShop.Application.Common;
using MyShop.Domain.Enums;
using MyShop.Application.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyShop.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Result<OrderDto>> CreateOrder(Guid CustomerId, AddOrderDto Order);
        Task<Result<List<OrderDto>>> GetOrdersByCustomerId(Guid customerId);
        Task<Result<List<OrderDto>>> GetOrdersBySellerId(Guid SellerId);
        
        // New Additions
        Task<Result<IEnumerable<OrderDto>>> GetAllOrdersAsync(int page = 1, int pageSize = 10);
        Task<Result<OrderDto>> GetOrderByIdAsync(Guid orderId);
        Task<Result<OrderDto>> UpdateOrderStatusAsync(UpdateOrderStatusDto dto);
        Task<Result<OrderDto>> CancelOrderAsync(Guid userId,Guid orderId);
    }
}
