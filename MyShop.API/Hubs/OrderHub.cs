using Microsoft.AspNetCore.SignalR;
using MyShop.CORE.Dtos.Order;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyShop.API.Hubs
{
    
    public class OrderHub : Hub
    {
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderHub(IHubContext<OrderHub> hubContext)
        {
            _hubContext = hubContext;
        }

        
        public async Task JoinSellerGroup(string sellerId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"seller_{sellerId}");
        }

        
        public async Task LeaveSellerGroup(string sellerId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"seller_{sellerId}");
        }

        
        public async Task BroadcastNewOrder(string sellerId, OrderDto order)
        {
            await _hubContext.Clients.Group($"seller_{sellerId}")
                .SendAsync("ReceiveNewOrder", order);
        }

        
        public async Task BroadcastToAll(OrderDto order)
        {
            await Clients.All.SendAsync("ReceiveNewOrder", order);
        }

        
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
