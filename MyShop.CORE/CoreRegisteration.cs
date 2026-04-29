using FluentValidation;
using Identity.Core.Interfaces;
using Identity.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using MyShop.CORE.FluentValidation.Auth;
using MyShop.CORE.Implmentations;

using MyShop.CORE.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE
{
    public static class CoreRegisteration
    {
        public static IServiceCollection AddCoreDependances(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IFileService, FileService>();

            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IWishService, WishService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<ICartItemsService, CartItemsService>();
            services.AddValidatorsFromAssembly(typeof(CoreRegisteration).Assembly);
            services.AddAutoMapper(typeof(CoreRegisteration).Assembly);
            
            return services;
        }
    }
}
