using ShoesShop.Domain.Modules.Carts.Entities;
using ShoesShop.Domain.Modules.Categories.Entities;
using ShoesShop.Domain.Modules.Commons.Entities;
using ShoesShop.Domain.Modules.Commons.Repositories;
using ShoesShop.Domain.Modules.Orders.Entities;
using ShoesShop.Domain.Modules.Products.Entities;
using ShoesShop.Domain.Modules.Shares.Entities;
using ShoesShop.Domain.Modules.Users.Entities;

namespace ShoesShop.Domain.Collections
{
    public interface IRepositoryCollection
    {
        public IGenericRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class, IBaseEntity;

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : class, IBaseEntity<TKey>
            where TKey : struct;

        public IGenericRepository<User, int> User { get; }

        public IGenericRepository<Product, int> Product { get; }

        public IGenericRepository<Category, int> Category { get; }

        public IGenericRepository<CartItem, int> Cart { get; }

        public IGenericRepository<Order, int> Order { get; }

        public IGenericRepository<Address, int> Address { get; }
    }
}