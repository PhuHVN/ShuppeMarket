using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ShuppeMarket.Application.Interfaces;
using ShuppeMarket.Application.Services;

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


            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        }


    }
}
