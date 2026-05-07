using MyShop.Domain.Entities.OrderEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Domain.RepositoryInterfaces
{
    public interface IOrderItemRepository:IBaseRepository<OrderItem>
    {
    }
}
