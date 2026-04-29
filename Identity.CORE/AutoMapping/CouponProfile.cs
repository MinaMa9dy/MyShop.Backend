using AutoMapper;
using MyShop.CORE.DTOs.Coupon;
using MyShop.CORE.Entities;
using System;

namespace MyShop.CORE.AutoMapping
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
            CreateMap<Coupon, CouponDto>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.CouponCode))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => 
                    src.CreatedDate.ToLocalTime()))
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => 
                    src.ExpirationDate.HasValue ? src.ExpirationDate.Value.ToLocalTime() : (DateTime?)null))
                .ReverseMap()
                .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => 
                    src.ExpirationDate.HasValue ? src.ExpirationDate.Value.ToUniversalTime() : (DateTime?)null));

            CreateMap<CreateCouponDto, Coupon>()
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => 
                    src.ExpirationDate.HasValue ? src.ExpirationDate.Value.ToUniversalTime() : (DateTime?)null));

            CreateMap<UpdateCouponDto, Coupon>()
                .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => 
                    src.ExpirationDate.HasValue ? src.ExpirationDate.Value.ToUniversalTime() : (DateTime?)null));

            CreateMap<UserCoupon, UserCouponDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.User.FirstName + " " + src.Customer.User.LastName));
        }
    }
}
