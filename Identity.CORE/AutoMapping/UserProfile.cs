using AutoMapper;
using MyShop.Core.Dtos;
using MyShop.CORE.Dtos.Profile;
using MyShop.CORE.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.AutoMapping
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
                .ForMember(e => e.FullName, opt => opt.MapFrom(src => src.FirstName + ' ' + src.LastName))
                .ForMember(e=>e.ImageUrl,opt=>opt.MapFrom(src=>src.userPhoto.RelativePath))
                .ReverseMap();
            CreateMap<UpdateProfileDto,ApplicationUser>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ReverseMap();
        }
    }
}
