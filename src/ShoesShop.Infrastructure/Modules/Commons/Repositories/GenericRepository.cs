using ShoesShop.Domain.Modules.Commons.Repositories;
using ShoesShop.Domain.Modules.Commons.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using ShoesShop.Crosscutting.Utilities.Exceptions;
using System.Linq.Expressions;

namespace ShoesShop.Infrastructure.Modules.Commons.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class, IBaseEntity
        {
            protected readonly DbContext _context;

            protected readonly DbSet<TEntity> _dbSet;

            public GenericRepository(DbContext context)
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

            public virtual async Task<TEntity> UpdateAsync([Required] TEntity entity)
            {
                _dbSet.Update(entity);
                return entity;
            }
        }

    public class GenericRepository<TEntity, TKey> : GenericRepository<TEntity>, IGenericRepository<TEntity, TKey>
        where TEntity : class, IBaseEntity<TKey>
        where TKey : struct
    {
        public GenericRepository(DbContext context) : base(context)
        {
        }

        public virtual async Task<int> CountAsync(IQueryable<TEntity> queryable)
        {
            return await queryable.CountAsync();
        }

        public virtual async Task DeleteAsync([Required] TKey id)
        {
            var entity = await _dbSet.FindAsync(id) ?? throw new NotFoundException($"Entity with id {id} not found");
            _dbSet.Remove(entity!);
        }

        public virtual async Task<bool> ExistAsync(IQueryable<TEntity> queryable)
        {
            return await queryable.AnyAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                                Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            IQueryable<TEntity> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }
        
        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        
        public virtual async Task<TEntity?> GetByIdAsync([Required] TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}