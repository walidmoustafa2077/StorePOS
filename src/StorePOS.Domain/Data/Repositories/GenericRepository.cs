using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Data;
using StorePOS.Domain.Data.Repositories.Interfaces;
using System.Linq.Expressions;

namespace StorePOS.Domain.Repositories
{
    /// <summary>
    /// Generic repository implementation providing common CRUD operations for all entities.
    /// Serves as the base class for entity-specific repositories and implements the Repository pattern
    /// with Entity Framework Core as the underlying data access technology.
    /// </summary>
    /// <typeparam name="T">The entity type that this repository manages</typeparam>
    /// <remarks>
    /// This implementation provides:
    /// - Consistent data access patterns across all entities
    /// - Performance optimizations through configurable change tracking
    /// - Expression-based filtering that translates to efficient SQL
    /// - Proper async/await patterns for scalability
    /// - Flexible query composition for complex scenarios
    /// </remarks>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        /// <summary>
        /// The Entity Framework Core database context for data access operations.
        /// </summary>
        protected readonly AppDbContext _db;
        
        /// <summary>
        /// The DbSet representing the table for entity type T in the database.
        /// </summary>
        protected readonly DbSet<T> _set;

        /// <summary>
        /// Initializes a new instance of the GenericRepository with the specified database context.
        /// </summary>
        /// <param name="db">The database context to use for data access operations</param>
        public GenericRepository(AppDbContext db)
        {
            _db = db;
            _set = _db.Set<T>();
        }

        /// <inheritdoc />
        /// <remarks>
        /// Returns an IQueryable that can be further composed by calling code.
        /// When asNoTracking is true (default), the query will not track changes to entities,
        /// which improves performance for read-only scenarios.
        /// </remarks>
        public IQueryable<T> Query(bool asNoTracking = true)
            => asNoTracking ? _set.AsNoTracking() : _set.AsQueryable();

        /// <inheritdoc />
        /// <remarks>
        /// Uses Entity Framework's Find method for optimal performance when searching by primary key.
        /// When asNoTracking is requested, the entity is detached from the context after retrieval
        /// to prevent change tracking overhead.
        /// </remarks>
        public async Task<T?> GetByIdAsync(object id, bool asNoTracking = true, CancellationToken ct = default)
        {
            var entity = await _set.FindAsync(new object?[] { id }, ct);
            if (entity is null) return null;

            // Detach entity from context if no tracking is requested
            if (asNoTracking)
            {
                _db.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Builds a query with optional related entity inclusion using Entity Framework's Include method.
        /// The predicate expression is translated to a SQL WHERE clause for efficient server-side filtering.
        /// </remarks>
        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true, CancellationToken ct = default, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _set;
            if (asNoTracking) query = query.AsNoTracking();

            // Apply includes for eager loading of related entities
            if (includes is { Length: > 0 })
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(predicate, ct);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Provides comprehensive querying capabilities with:
        /// - Optional server-side filtering via predicate expressions
        /// - Custom ordering logic through orderBy function
        /// - Eager loading of related entities via includes
        /// - Performance optimization through configurable change tracking
        /// All query composition is done before database execution for optimal SQL generation.
        /// </remarks>
        public async Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate = null,
                                             Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                             bool asNoTracking = true,
                                             CancellationToken ct = default,
                                             params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _set;
            if (asNoTracking) query = query.AsNoTracking();

            // Apply filtering - translates to SQL WHERE clause
            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            // Apply eager loading for related entities
            if (includes is { Length: > 0 })
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // Apply custom ordering logic
            if (orderBy is not null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync(ct);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Efficiently checks for entity existence using AsNoTracking and Any() which translates
        /// to an optimized SQL EXISTS query without loading entity data into memory.
        /// </remarks>
        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => _set.AsNoTracking().AnyAsync(predicate, ct);

        /// <inheritdoc />
        /// <remarks>
        /// Performs efficient counting using SQL COUNT operations. When no predicate is provided,
        /// uses the optimized Count() method. With a predicate, translates to SQL COUNT with WHERE clause.
        /// </remarks>
        public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
            => predicate is null ? _set.CountAsync(ct) : _set.CountAsync(predicate, ct);

        /// <inheritdoc />
        /// <remarks>
        /// Stages the entity for insertion. The entity is not immediately persisted to the database.
        /// Call SaveChanges on the Unit of Work to commit the changes.
        /// </remarks>
        public Task AddAsync(T entity, CancellationToken ct = default) => _set.AddAsync(entity, ct).AsTask();

        /// <inheritdoc />
        /// <remarks>
        /// Bulk operation for staging multiple entities for insertion. More efficient than multiple
        /// individual AddAsync calls as it batches the operations.
        /// </remarks>
        public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default) => _set.AddRangeAsync(entities, ct);

        /// <inheritdoc />
        /// <remarks>
        /// Immediately marks the entity as modified in the context. Entity Framework will detect
        /// which properties have changed and generate appropriate SQL UPDATE statements.
        /// </remarks>
        public void Update(T entity) => _set.Update(entity);

        /// <inheritdoc />
        /// <remarks>
        /// Async wrapper around the synchronous Update method for API consistency.
        /// The underlying operation is still synchronous as it only modifies the context state.
        /// </remarks>
        public Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            _set.Update(entity);
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Marks the entity for deletion. The entity is not immediately removed from the database.
        /// Call SaveChanges on the Unit of Work to commit the deletion.
        /// </remarks>
        public void Remove(T entity) => _set.Remove(entity);

        /// <inheritdoc />
        /// <remarks>
        /// Bulk operation for staging multiple entities for deletion. More efficient than multiple
        /// individual Remove calls as it batches the operations.
        /// </remarks>
        public void RemoveRange(IEnumerable<T> entities) => _set.RemoveRange(entities);
    }
}