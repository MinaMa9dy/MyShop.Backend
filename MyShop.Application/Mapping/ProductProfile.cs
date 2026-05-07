using AutoMapper;
using MyShop.Application.DTOs.Product;
using MyShop.Domain.Entities.ProductEntities;
using System.Linq;

namespace MyShop.Application.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<AddProductDto, Product>()
                .ForMember(dest => dest.productPhotos, opt => opt.Ignore())
                .ForMember(dest => dest.productVariants, opt => opt.Ignore());

            CreateMap<UpdateProductDto, Product>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.User.FullName))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Any() ? src.Reviews.Count : 0))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews.Any() ? Math.Round((decimal)src.Reviews.Average(r => r.stars), 2)
                            : 0))
                .ForMember(dest => dest.ProductPhotos, opt => opt.MapFrom(src => src.productPhotos))
                .ForMember(dest => dest.ProductVariants, opt => opt.MapFrom(src => src.productVariants))
                .ForMember(dest => dest.HaveSale, opt => opt.MapFrom(src => src.HaveSale))
                .ForMember(dest => dest.IsFasting, opt => opt.MapFrom(src => src.IsFasting))
                .ForMember(dest => dest.Popularity, opt => opt.MapFrom(src => src.Popularity));

            CreateMap<ProductPhoto, ProductPhotoDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.RelativePath));

            CreateMap<ProductVariant, ProductVariantDto>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.VariantAttributes));

            CreateMap<VariantAttribute, VariantAttributeDto>()
                .ForMember(dest => dest.AttributeId, opt => opt.MapFrom(src => src.AttributeId))
                .ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src.Attribute.Name))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));

            CreateMap<AddProductVariantDto, ProductVariant>()
                .ForMember(des => des.VariantAttributes, src => src.MapFrom(x => x.Attributes))
                .ReverseMap();
            CreateMap<AddVariantAttributeDto, VariantAttribute>()
                .ReverseMap();
        }
    }
}
