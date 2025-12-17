using FluentValidation;
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


            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        }


    }
}
