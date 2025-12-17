using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShuppeMarket.Domain.Entities;
using ShuppeMarket.Domain.Enums;
using ShuppeMarket.Infrastructure.DatabaseSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Infrastructure.Seed
{
    public class DataSeeder
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public DataSeeder(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public async Task SeedAdminAsync()
        {
            var existAdmin = _context.Accounts.FirstOrDefault(x => x.Role == RoleEnum.Admin);
            if (existAdmin != null)
                return;

            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (_context.Accounts.FirstOrDefault(x => x.Role == RoleEnum.Admin) is null)
            {
                var admin = new Account
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = adminEmail,
                    Password = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    FullName = "System Administrator",
                    Status = StatusEnum.Active,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                    Role = RoleEnum.Admin

                };
                _context.Accounts.Add(admin);
                await _context.SaveChangesAsync();
            }
        }
    }
}
