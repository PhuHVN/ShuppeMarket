using CloudinaryDotNet;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Application.Services;
using StackExchange.Redis;

namespace ShuppeMarket.Application
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Application service registrations go here
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IGenerateTokenService, GenerateTokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOtpCacheService, OtpCacheService>();
            services.AddScoped<ISellerService, SellerService>();
            services.AddScoped<ICloudinary, Cloudinary>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();      
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        }


    }
}
