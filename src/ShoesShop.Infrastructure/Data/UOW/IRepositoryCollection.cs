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

namespace ShoesShop.Infrastructure.Data.UOW;

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

    public IGenericRepository<Cart, int> Cart { get; }

    public IGenericRepository<Order, int> Order { get; }

    public IGenericRepository<Address, int> Address { get; }

    public IGenericRepository<Message, int> Message { get; }

    public IGenericRepository<Image, int> Image { get; }

    public IGenericRepository<Review, int> Review { get; }
}