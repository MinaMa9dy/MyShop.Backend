using AutoMapper;
using MyShop.CORE.Dtos.Product;
using MyShop.CORE.Entities;
using System.Linq;

namespace MyShop.CORE.AutoMapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<AddProductDto, Product>()
                .ForMember(dest => dest.productPhotos, opt => opt.Ignore()) // Handled manually in service
                .ForMember(dest => dest.productVariants, opt => opt.Ignore()); // Handled manually in service

            CreateMap<UpdateProductDto, Product>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null && src.Supplier.User != null ? $"{src.Supplier.User.FirstName} {src.Supplier.User.LastName}" : string.Empty))
                .ForMember(dest => dest.ProductPhotos, opt => opt.MapFrom(src => src.productPhotos))
                .ForMember(dest => dest.ProductVariants, opt => opt.MapFrom(src => src.productVariants))
                // Map top-level display fields from the first active variant
                .ForMember(dest => dest.OldPrice, opt => opt.MapFrom(src => src.productVariants != null && src.productVariants.Any() ? src.productVariants.First().OldPrice : 0))
                .ForMember(dest => dest.NewPrice, opt => opt.MapFrom(src => src.productVariants != null && src.productVariants.Any() ? src.productVariants.First().NewPrice : 0))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.productVariants != null ? src.productVariants.Sum(v => v.StockQuantity) : 0))
                .ForMember(dest => dest.HaveSale, opt => opt.MapFrom(src => src.HaveSale))
                .ForMember(dest => dest.IsFasting, opt => opt.MapFrom(src => src.IsFasting))
                .ForMember(dest => dest.Popularity, opt => opt.MapFrom(src => src.Popularity));

            CreateMap<ProductPhoto, ProductPhotoDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.RelativePath));

            CreateMap<ProductVariant, ProductVariantDto>()
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.VariantAttributes));

            CreateMap<VariantAttribute, VariantAttributeDto>()
                .ForMember(dest => dest.AttributeId, opt => opt.MapFrom(src => src.AttributeId))
                .ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src.Attribute != null ? src.Attribute.Name : string.Empty))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));

            CreateMap<AddProductVariantDto, ProductVariant>();
            CreateMap<AddVariantAttributeDto, VariantAttribute>();
        }
    }
}
