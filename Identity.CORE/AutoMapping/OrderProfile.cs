using AutoMapper;
using MyShop.CORE.Dtos.Order;
using MyShop.CORE.Entities.OrderEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.AutoMapping
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>().ReverseMap();
        }
    }
}
