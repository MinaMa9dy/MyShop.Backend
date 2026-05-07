using AutoMapper;
using MyShop.Application.DTOs.Review;
using MyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Mapping
{
    public class ReviewProfile:Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, AddReviewDto>().ReverseMap();
            CreateMap<Review, ReviewDto>().ReverseMap();

        }
    }
}
