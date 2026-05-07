using Microsoft.AspNetCore.SignalR;
using MyShop.Application.DTOs.Order;
using MyShop.Application.Interfaces;
using MyShop.API.Hubs;
using System;
using System.Threading.Tasks;

namespace MyShop.API.Services
{
    
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<OrderHub> _hubContext;

        public SignalRNotificationService(IHubContext<OrderHub> hubContext)
        {
            _hubContext = hubContext;
        }

        
        public async Task BroadcastNewOrderAsync(Guid sellerId, OrderDto order)
        {
            // Send to the seller's specific group
            await _hubContext.Clients.Group($"seller_{sellerId}")
                .SendAsync("ReceiveNewOrder", order);
            
            
        }
    }
}
