using AutoMapper;
using MyShop.CORE.Dtos.Review;
using MyShop.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.AutoMapping
{
    public class ReviewProfile:Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, AddReviewDto>().ReverseMap();
            CreateMap<Review, ReviewResponseDto>().ReverseMap();

        }
    }
}
