using MyShop.CORE.Dtos.Order;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Interfaces
{
    
    public interface INotificationService
    {
       
        Task BroadcastNewOrderAsync(Guid sellerId, OrderDto order);
    }
}
