using ShoesShop.Infrastructure.Data.Databases.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ShoesShop.Infrastructure.Data.Databases
{
    public static class DatabaseModule
    {
        public static void AddDatabaseModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ShoesShopDBContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ShoesShopDBConnection"));
            });
        }

        public static void TestingConnectionDatabase(this IApplicationBuilder context)
        {
            using (var scope = context.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ShoesShopDBContext>();
                dbContext.Database.Migrate();
                dbContext.Database.EnsureCreated();
            }
        }
    }
}