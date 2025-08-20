using System.Linq.Expressions;

namespace StorePOS.Domain.Data.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        // Expose a queryable so API layers can compose includes/projections (e.g., SelectReadDto) efficiently.
        IQueryable<T> Query(bool asNoTracking = true);

        Task<T?> GetByIdAsync(object id, bool asNoTracking = true, CancellationToken ct = default);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true, CancellationToken ct = default, params Expression<Func<T, object>>[] includes);
        Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate = null,
                                Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                bool asNoTracking = true,
                                CancellationToken ct = default,
                                params Expression<Func<T, object>>[] includes);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);

        // CRUD operations
        Task AddAsync(T entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);
        void Update(T entity);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}