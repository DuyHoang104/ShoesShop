using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using ShoesShop.Domain.Modules.Commons.Entities;

namespace ShoesShop.Domain.Modules.Commons.Repositories
{
    public interface IRepository
    {}

    public interface IGenericRepository<TEntity> : IRepository
        where TEntity : IBaseEntity
    {
        Task DeleteAsync([Required] TEntity entity);

        Task<TEntity> SaveAsync([Required] TEntity entity);

        Task<TEntity> UpdateAsync([Required] TEntity entity);

        Task<TEntity> InsertAsync([Required] TEntity entity);
    }

    public interface IGenericRepository<TEntity, TKey> : IGenericRepository<TEntity>
        where TEntity : IBaseEntity<TKey>
        where TKey : struct
    {
        Task DeleteAsync([Required] TKey id);

        Task<int> CountAsync(IQueryable<TEntity> queryable);

        Task<bool> ExistAsync(IQueryable<TEntity> queryable);

        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                               Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);

        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<TEntity?> GetByIdAsync([Required] TKey id);

        Task DeleteRangeAsync(IEnumerable<TEntity> entities);
    }
}