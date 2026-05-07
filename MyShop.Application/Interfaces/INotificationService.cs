using MyShop.Application.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Interfaces
{
    
    public interface INotificationService
    {
       
        Task BroadcastNewOrderAsync(Guid sellerId, OrderDto order);
    }
}
