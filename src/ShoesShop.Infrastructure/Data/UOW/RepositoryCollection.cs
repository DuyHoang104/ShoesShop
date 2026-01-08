
using Microsoft.EntityFrameworkCore;
using ShoesShop.Domain.Carts.Entities;
using ShoesShop.Domain.Categories.Entities;
using ShoesShop.Domain.Commons.Entities;
using ShoesShop.Domain.Commons.Repositories;
using ShoesShop.Domain.Orders.Entities;
using ShoesShop.Domain.Products.Entities;
using ShoesShop.Domain.Shares.Addresses.Entities;
using ShoesShop.Domain.Shares.Image.Entities;
using ShoesShop.Domain.Shares.Messages.Entity;
using ShoesShop.Domain.Shares.Review.Entity;
using ShoesShop.Domain.Users.Entities;
using ShoesShop.Infrastructure.Data.Databases.Context;
using ShoesShop.Infrastructure.Modules.Commons.Repositories;

namespace ShoesShop.Infrastructure.Data.UOW
{
    public class RepositoryCollection : IRepositoryCollection
    {
        protected readonly DbContext _dbContext;
        protected readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, IRepository> _repositories = new();

        public RepositoryCollection(ShoesShopDBContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
        }

        public IGenericRepository<User, int> User => GetRepository<User, int>();

        public IGenericRepository<Product, int> Product => GetRepository<Product, int>();

        public IGenericRepository<Category, int> Category => GetRepository<Category, int>();

        public IGenericRepository<Cart, int> Cart => GetRepository<Cart, int>();

        public IGenericRepository<Order, int> Order => GetRepository<Order, int>();

        public IGenericRepository<Address, int> Address => GetRepository<Address, int>();

        public IGenericRepository<Message, int> Message => GetRepository<Message, int>();

        public IGenericRepository<Image, int> Image => GetRepository<Image, int>();
        
        public IGenericRepository<Review, int> Review => GetRepository<Review, int>();

        public IGenericRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class, IBaseEntity
        {
            var repository = _repositories.GetValueOrDefault(typeof(TEntity));
            if (repository == null)
            {
                repository = new GenericRepository<TEntity>(_dbContext);
                _repositories.Add(typeof(TEntity), repository);
            }
            return (IGenericRepository<TEntity>)repository;
        }

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : class, IBaseEntity<TKey>
            where TKey : struct
        {
            var repository = _repositories.GetValueOrDefault(typeof(TEntity));
            if (repository == null)
            {
                repository = new GenericRepository<TEntity, TKey>(_dbContext);
                _repositories.Add(typeof(TEntity), repository);
            }
            return (IGenericRepository<TEntity, TKey>)repository;
        }
    }
}