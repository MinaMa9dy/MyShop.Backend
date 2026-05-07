using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using MyShop.Application.Interfaces;
using MyShop.Application.Interfaces.Auth;
using MyShop.Application.Services;
using MyShop.Application.Services.Auth;

namespace MyShop.Application;

public static class ApplicationRegistration
{
    public static IServiceCollection AddApplicationDependencies(
        this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationRegistration).Assembly);

        services.AddValidatorsFromAssembly(typeof(ApplicationRegistration).Assembly);

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<ICartItemsService, CartItemsService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IWishService, WishService>();
        services.AddScoped<IFileService, FileService>();

        return services;
    }
}
