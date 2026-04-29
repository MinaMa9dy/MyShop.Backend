using AutoMapper;
using MyShop.CORE.Dtos.Wish;
using MyShop.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.AutoMapping
{
    public class WishProfile:Profile
    {
        public WishProfile()
        {
            CreateMap<WishDto, WishList>()
                //.ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap();

            
        }
    }
}
