using MyShop.Domain.Entities;
using MyShop.Domain.Entities.OrderEntities;
using MyShop.Domain;
using MyShop.Domain.RepositoryInterfaces;
using MyShop.INFRASTRUCTURE.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShop.Domain.Entities.OrderEntities;

namespace MyShop.INFRASTRUCTURE.Repositories
{
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
