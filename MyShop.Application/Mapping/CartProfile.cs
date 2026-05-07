using AutoMapper;
using MyShop.Application.DTOs.CartItem;
using MyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Mapping
{
    public class CartProfile:Profile
    {
        public CartProfile()
        {
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductVariantId, opt => opt.MapFrom(src => src.ProductVariantId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant != null && src.ProductVariant.Product != null ? src.ProductVariant.Product.Name : string.Empty))
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.SKU : string.Empty))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductVariant != null ? src.ProductVariant.NewPrice : 0))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.ProductVariant != null && src.ProductVariant.Product != null && src.ProductVariant.Product.productPhotos != null && src.ProductVariant.Product.productPhotos.Any()
                    ? src.ProductVariant.Product.productPhotos.FirstOrDefault(p => p.IsMain).RelativePath ?? src.ProductVariant.Product.productPhotos.First().RelativePath
                    : string.Empty))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<CartItemCreateDto, CartItem>().ReverseMap();

        }
    }
}
