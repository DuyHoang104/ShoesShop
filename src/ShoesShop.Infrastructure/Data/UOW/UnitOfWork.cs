using Microsoft.EntityFrameworkCore.Storage;
using ShoesShop.Infrastructure.Collections;
using ShoesShop.Infrastructure.Data.Databases.Context;

namespace ShoesShop.Infrastructure.Data.UOW
{
    public class UnitOfWork : RepositoryCollection, IUnitOfWork
    {
        private bool _disposed = false;
        public IDbContextTransaction? Transaction { get; private set; }

        public UnitOfWork(ShoesShopDBContext dbContext, IServiceProvider serviceProvider) : base(dbContext, serviceProvider)
        {
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
            => Transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (Transaction == null)
            {
                throw new InvalidOperationException("No active transaction to commit.");
            }

            await Transaction!.CommitAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Transaction?.Dispose();
                    _dbContext?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
