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
            //Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);

            });
            modelBuilder.Entity<Product>()
                 .HasOne(p => p.Seller)
                 .WithMany(s => s.Products)
                 .HasForeignKey(p => p.SellerId)
                 .OnDelete(DeleteBehavior.Restrict);

            //Category-Product 
            modelBuilder.Entity<CategoryProduct>().HasKey(cp => new { cp.CategoryId, cp.ProductId });
            modelBuilder.Entity<CategoryProduct>()
                .HasOne(cp => cp.Category)
                .WithMany(c => c.CategoryProducts)
                .HasForeignKey(cp => cp.CategoryId);
            modelBuilder.Entity<CategoryProduct>()
                .HasOne(cp => cp.Product)
                .WithMany(p => p.CategoryProducts)
                .HasForeignKey(cp => cp.ProductId);

        }
    }
}
//dotnet ef migrations add AddAccountTable -p ShuppeMarket.Infrastructure -s ShuppeMarket.Api
//dotnet ef database update -p ShuppeMarket.Infrastructure -s ShuppeMarket.Api
