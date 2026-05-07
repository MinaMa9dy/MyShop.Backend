using AutoMapper;
using MyShop.Application.DTOs.Wish;
using MyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Mapping
{
    public class WishProfile:Profile
    {
        public WishProfile()
        {
            CreateMap<WishDto, WishList>()
                .ReverseMap();

            
        }
    }
}
