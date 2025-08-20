using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Data;
using StorePOS.Domain.Data.Repositories.Interfaces;
using System.Linq.Expressions;

namespace StorePOS.Domain.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _db;
        protected readonly DbSet<T> _set;

        public GenericRepository(AppDbContext db)
        {
            _db = db;
            _set = _db.Set<T>();
        }

        public IQueryable<T> Query(bool asNoTracking = true)
            => asNoTracking ? _set.AsNoTracking() : _set.AsQueryable();

        public async Task<T?> GetByIdAsync(object id, bool asNoTracking = true, CancellationToken ct = default)
        {
            var entity = await _set.FindAsync(new object?[] { id }, ct);
            if (entity is null) return null;

            if (asNoTracking)
            {
                _db.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true, CancellationToken ct = default, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _set;
            if (asNoTracking) query = query.AsNoTracking();

            if (includes is { Length: > 0 })
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(predicate, ct);
        }

        public async Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate = null,
                                             Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                             bool asNoTracking = true,
                                             CancellationToken ct = default,
                                             params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _set;
            if (asNoTracking) query = query.AsNoTracking();

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            if (includes is { Length: > 0 })
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (orderBy is not null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync(ct);
        }

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => _set.AsNoTracking().AnyAsync(predicate, ct);

        public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
            => predicate is null ? _set.CountAsync(ct) : _set.CountAsync(predicate, ct);

        public Task AddAsync(T entity, CancellationToken ct = default) => _set.AddAsync(entity, ct).AsTask();

        public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default) => _set.AddRangeAsync(entities, ct);

        public void Update(T entity) => _set.Update(entity);

        public Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _set.Update(entity);
            return Task.CompletedTask;
        }

        public void Remove(T entity) => _set.Remove(entity);

        public void RemoveRange(IEnumerable<T> entities) => _set.RemoveRange(entities);
    }
}