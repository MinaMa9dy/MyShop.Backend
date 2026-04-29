using MyShop.CORE.Dtos.Order;
using MyShop.CORE.Entities;
using MyShop.CORE.Helpers;
using MyShop.CORE.Enums;
using MyShop.CORE.Helpers.ResultPattern;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyShop.CORE.Interfaces
{
    public interface IOrderService
    {
        Task<Result<OrderDto>> CreateOrder(AddOrderDto Order);
        Task<Result<List<OrderDto>>> GetOrdersByUserId(Guid? customerId);
        Task<Result<List<OrderDto>>> GetOrdersBySellerId(Guid? SellerId);
        
        // New Additions
        Task<Result<PageResult<OrderDto>>> GetAllOrdersAsync(int page = 1, int pageSize = 10);
        Task<Result<OrderDto>> GetOrderByIdAsync(Guid orderId);
        Task<Result<OrderDto>> UpdateOrderStatusAsync(Guid orderId, DeliveryStatusOptions newStatus);
        Task<Result<OrderDto>> CancelOrderAsync(Guid orderId);
    }
}
