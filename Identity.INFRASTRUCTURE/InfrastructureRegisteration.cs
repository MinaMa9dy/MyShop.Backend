using Identity.Core.Interfaces;
using Identity.Core.Services;
using Identity.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using MyShop.CORE.Interfaces;
using MyShop.INFRASTRUCTURE.Repositories;
using MyShop.INFRASTRUCTURE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.INFRASTRUCTURE
{
    public static class InfrastructureRegisteration
    {
        public static IServiceCollection AddInfrasturctureDependances(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork,UnitOfWork>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ICacheService, CacheService>();
            return services;
        }
    }
}
