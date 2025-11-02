using Microsoft.EntityFrameworkCore.Storage;
using ShoesShop.Domain.Collections;

namespace ShoesShop.Infrastructure.Data.UOW
{
    public interface IUnitOfWork : IRepositoryCollection, IDisposable
    {
        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        public Task CommitAsync(CancellationToken cancellationToken = default);
    }
}