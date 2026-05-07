using AutoMapper;
using MyShop.Application.DTOs;
using MyShop.Application.DTOs.Profile;
using MyShop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Mapping
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser,LoginDto> ().ReverseMap();
            CreateMap<RegisterDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForSourceMember(src => src.ConfirmPassword, opt => opt.DoNotValidate()).ReverseMap();
            CreateMap<ApplicationUser, ProfileDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.userPhoto != null ? src.userPhoto.RelativePath : null))
                .ReverseMap();
            CreateMap<UpdateProfileDto,ApplicationUser>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ReverseMap();
        }
    }
}
