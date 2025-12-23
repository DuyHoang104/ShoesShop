
using Microsoft.EntityFrameworkCore;
using ShoesShop.Domain.Collections;
using ShoesShop.Domain.Modules.Messages.Entity;
using ShoesShop.Domain.Modules.Shares.Image.Entities;
using ShoesShop.Domain.Modules.Shares.Review.Entity;
using ShoesShop.Domain.Modules.User.Carts.Entities;
using ShoesShop.Domain.Modules.User.Categories.Entities;
using ShoesShop.Domain.Modules.User.Commons.Entities;
using ShoesShop.Domain.Modules.User.Commons.Repositories;
using ShoesShop.Domain.Modules.User.Orders.Entities;
using ShoesShop.Domain.Modules.User.Products.Entities;
using ShoesShop.Domain.Modules.User.Shares.Entities;
using ShoesShop.Infrastructure.Data.Databases.Context;
using ShoesShop.Infrastructure.Modules.Commons.Repositories;

namespace ShoesShop.Infrastructure.Collections
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

        public IGenericRepository<ShoesShop.Domain.Modules.User.Users.Entities.User, int> User => GetRepository<ShoesShop.Domain.Modules.User.Users.Entities.User, int>();

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