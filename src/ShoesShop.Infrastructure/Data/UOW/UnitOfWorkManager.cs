using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using ShoesShop.Domain.Carts.Entities;
using ShoesShop.Domain.Categories.Entities;
using ShoesShop.Domain.Commons.Repositories;
using ShoesShop.Domain.Orders.Entities;
using ShoesShop.Domain.Products.Entities;
using ShoesShop.Domain.Shares.Addresses.Entities;
using ShoesShop.Domain.Shares.Image.Entities;
using ShoesShop.Domain.Shares.Messages.Entity;
using ShoesShop.Domain.Shares.Review.Entity;
using ShoesShop.Domain.Users.Entities;
using ShoesShop.Infrastructure.Data.Databases.Context;

namespace ShoesShop.Infrastructure.Data.UOW;

public class UnitOfWorkManager : IUnitOfWorkManager
{
    private readonly SemaphoreSlim _semaphore;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWorkManager(
        IServiceProvider serviceProvider,
        IServiceScopeFactory scopeFactory,
        int maxConcurrency = 1
    )
    {
        _serviceProvider = serviceProvider;
        _scopeFactory = scopeFactory;
        _semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
    }

    public IUnitOfWork Create()
    {  
        var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ShoesShopDBContext>();
        var uow = new UnitOfWork(dbContext, scope.ServiceProvider);
        return uow;
    }

    public async Task<IUnitOfWork> RentAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        var scope = _scopeFactory.CreateScope();
        var uow = Create();

        return new PooledUnitOfWork(uow, scope, _semaphore);
    }
}

public class PooledUnitOfWork : IUnitOfWork
{
    private readonly IUnitOfWork _inner;
    private readonly IServiceScope _scope;
    private readonly SemaphoreSlim _semaphore;
    private bool _disposed;

    public PooledUnitOfWork(
        IUnitOfWork inner,
        IServiceScope scope,
        SemaphoreSlim semaphore)
    {
        _inner = inner;
        _scope = scope;
        _semaphore = semaphore;
    }

    public IGenericRepository<User, int> User => _inner.User;
    public IGenericRepository<Product, int> Product => _inner.Product;
    public IGenericRepository<Category, int> Category => _inner.Category;
    public IGenericRepository<Cart, int> Cart => _inner.Cart;
    public IGenericRepository<Order, int> Order => _inner.Order;
    public IGenericRepository<Address, int> Address => _inner.Address;
    public IGenericRepository<Message, int> Message => _inner.Message;
    public IGenericRepository<Image, int> Image => _inner.Image;
    public IGenericRepository<Review, int> Review => _inner.Review;

    public Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
        => _inner.BeginTransactionAsync(cancellationToken);

    public Task CommitAsync(CancellationToken cancellationToken = default)
        => _inner.CommitAsync(cancellationToken);

    public void Dispose()
    {
        if (_disposed) return;

        _inner.Dispose();
        _scope.Dispose();
        _semaphore.Release();

        _disposed = true;
    }

    IGenericRepository<TEntity> IRepositoryCollection.GetRepository<TEntity>()
    {
        return _inner.GetRepository<TEntity>();
    }

    IGenericRepository<TEntity, TKey> IRepositoryCollection.GetRepository<TEntity, TKey>()
    {
        return _inner.GetRepository<TEntity, TKey>();
    }
}
