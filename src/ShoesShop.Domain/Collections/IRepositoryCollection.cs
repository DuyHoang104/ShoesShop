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
using ShoesShop.Domain.Modules.User.Users.Entities;

namespace  ShoesShop.Domain.Collections;

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