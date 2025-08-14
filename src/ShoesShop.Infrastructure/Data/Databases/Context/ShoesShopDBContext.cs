using Microsoft.EntityFrameworkCore;

namespace ShoesShop.Infrastructure.Data.Databases.Context
{
    public class ShoesShopDBContext : DbContext
    {
        public ShoesShopDBContext(DbContextOptions<ShoesShopDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShoesShopDBContext).Assembly);
                        
            base.OnModelCreating(modelBuilder);
        }
    }
}