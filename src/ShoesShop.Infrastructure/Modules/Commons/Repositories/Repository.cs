using ShoesShop.Domain.Modules.Commons.Repositories;
using ShoesShop.Domain.Modules.Commons.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using ShoesShop.Crosscutting.Utilities.Exceptions;

namespace ShoesShop.Infrastructure.Modules.Commons.Repositories
{
    public abstract class Repository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class, IBaseEntity
        {
            private readonly DbContext _context;

            protected readonly DbSet<TEntity> _dbSet;

            protected Repository(DbContext context)
            {
                _context = context;
                _dbSet = _context.Set<TEntity>();
            }

            public virtual async Task DeleteAsync([Required] TEntity entity)
            {
                _dbSet.Remove(entity);
            }

            public virtual async Task<TEntity> InsertAsync([Required] TEntity entity)
            {
                await _dbSet.AddAsync(entity);
                return entity;
            }

            public virtual async Task<TEntity> SaveAsync([Required] TEntity entity)
            {
                var existingEntity = await _dbSet.FindAsync(entity);
                return existingEntity != null
                ? await UpdateAsync(existingEntity)
                : await InsertAsync(entity);
            }

            public async Task<TEntity> UpdateAsync([Required] TEntity entity)
            {
                _dbSet.Update(entity);
                return entity;
            }
        }

    public abstract class Repository<TEntity, TKey> : Repository<TEntity>, IGenericRepository<TEntity, TKey>
        where TEntity : class, IBaseEntity<TKey>
        where TKey : struct
        {
            protected Repository(DbContext context) : base(context)
            {
            }

            public async Task DeleteAsync([Required] TKey id)
            {
                var entity = await _dbSet.FindAsync(id) ?? throw new NotFoundException($"Entity with id {id} not found");
                _dbSet.Remove(entity!);
            }
        }
}