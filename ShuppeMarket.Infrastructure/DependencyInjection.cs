
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MuseumSystem.Infrastructure.Seed;
using ShuppeMarket.Domain.Abstractions;
using ShuppeMarket.Infrastructure.Implemention;

namespace ShuppeMarket.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Infrastructure service registrations go here
            services.AddLogging();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<DataSeeder>();
        }

    }
}
