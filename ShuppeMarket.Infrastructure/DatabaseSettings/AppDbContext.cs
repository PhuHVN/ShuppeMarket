using Microsoft.EntityFrameworkCore;
using ShuppeMarket.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShuppeMarket.Infrastructure.DatabaseSettings
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        //DbSets 
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            // Model configurations go here

            
        }
    }
}
//dotnet ef migrations add AddAccountTable -p ShuppeMarket.Infrastructure -s ShuppeMarket.Api
//dotnet ef database update -p ShuppeMarket.Infrastructure -s ShuppeMarket.Api
