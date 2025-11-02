using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ShoesShop.Crosscutting.Utilities.Exceptions;
using ShoesShop.Infrastructure.Data.UOW;

namespace ShoesShop.Infrastructure.Modules
{
    public static class RepositoryModule
    {
        // public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        // {
        //     var repositoryTypes = (Assembly.GetAssembly(typeof(RepositoryModule))?.GetTypes()
        //         .Where(type => type.IsClass && !type.IsAbstract && type.Name.EndsWith("Repository"))
        //         .Select(type => new
        //         {
        //             Interface = Array.Find(type.GetInterfaces(), x => x.IsInterface && x.Name.Equals("I" + type.Name)) ?? throw new NotFoundException("Interface not found"),
        //             Repository = type
        //         })
        //         .ToList()) ?? throw new NotFoundException("No repository types found");

        //     foreach (var repository in repositoryTypes)
        //     {
        //         context.Services.AddScoped(repository.Interface, repository.Repository);
        //     }

        //     return Task.CompletedTask;
        // }

        public static async Task AddRepositoryModule(this IServiceCollection services)
        {
            // services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            // services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            // services.AddScoped(typeof(IAuditRepository<>), typeof(AuditRepository<>));
            // services.AddScoped(typeof(IAuditRepository<,>), typeof(AuditRepository<,>));
            // services.AddScoped(typeof(IQueryRepository<,>), typeof(QueryRepository<,>));
            // services.AddScoped(typeof(IQueryRepository<,,>), typeof(QueryRepository<,,>));

            var repositoryTypes = (Assembly.GetAssembly(typeof(RepositoryModule))?.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.Name.EndsWith("Repository"))
                .Select(type => new
                {
                    Interface = Array.Find(type.GetInterfaces(), x => x.IsInterface && x.Name.Equals("I" + type.Name)) ?? throw new NotFoundException("Interface not found"),
                    Repository = type
                })
                .ToList()) ?? throw new NotFoundException("No repository types found");

            foreach (var repository in repositoryTypes)
            {
                services.AddScoped(repository.Interface, repository.Repository);
            }
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}