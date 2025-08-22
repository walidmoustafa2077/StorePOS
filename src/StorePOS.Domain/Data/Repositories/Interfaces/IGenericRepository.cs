using System.Linq.Expressions;

namespace StorePOS.Domain.Data.Repositories.Interfaces
{
    /// <summary>
    /// Generic repository interface providing common CRUD operations and query capabilities for all entities.
    /// Implements the Repository pattern with support for async operations, change tracking control, and expression-based filtering.
    /// </summary>
    /// <typeparam name="T">The entity type that this repository manages</typeparam>
    /// <remarks>
    /// This interface serves as the foundation for all entity-specific repositories, providing:
    /// - Consistent CRUD operations across all entity types
    /// - Performance optimization through configurable change tracking
    /// - Flexible querying with LINQ expressions
    /// - Efficient bulk operations
    /// - Async support for scalable operations
    /// </remarks>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Exposes a queryable interface for composing complex queries with includes and projections.
        /// </summary>
        /// <param name="asNoTracking">
        /// When true (default), entities are not tracked by the context, improving performance for read-only operations.
        /// When false, entities are tracked and changes can be detected and persisted.
        /// </param>
        /// <returns>An IQueryable that can be further composed with LINQ operations</returns>
        /// <remarks>
        /// This method enables API layers to efficiently compose complex queries including:
        /// - SelectReadDto projections to reduce data transfer
        /// - Dynamic filtering and sorting
        /// - Include operations for related entities
        /// - Pagination and limiting
        /// </remarks>
        IQueryable<T> Query(bool asNoTracking = true);

        /// <summary>
        /// Retrieves a single entity by its primary key identifier.
        /// </summary>
        /// <param name="id">The primary key value of the entity to retrieve</param>
        /// <param name="asNoTracking">Controls whether the entity should be tracked by the context</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>The entity if found, null otherwise</returns>
        Task<T?> GetByIdAsync(object id, bool asNoTracking = true, CancellationToken ct = default);

        /// <summary>
        /// Retrieves the first entity matching the specified predicate, with optional related data inclusion.
        /// </summary>
        /// <param name="predicate">Expression defining the filter criteria</param>
        /// <param name="asNoTracking">Controls entity change tracking</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <param name="includes">Navigation properties to include in the query</param>
        /// <returns>The first matching entity or null if no match is found</returns>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true, CancellationToken ct = default, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Retrieves a list of entities with optional filtering, ordering, and related data inclusion.
        /// </summary>
        /// <param name="predicate">Optional filter expression. If null, all entities are returned</param>
        /// <param name="orderBy">Optional ordering function for sorting results</param>
        /// <param name="asNoTracking">Controls entity change tracking for performance optimization</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <param name="includes">Navigation properties to eagerly load with the entities</param>
        /// <returns>A list of entities matching the specified criteria</returns>
        /// <remarks>
        /// This method provides flexible entity retrieval with:
        /// - Server-side filtering to reduce data transfer
        /// - Custom ordering logic
        /// - Eager loading of related entities
        /// - Performance optimization through change tracking control
        /// </remarks>
        Task<List<T>> ListAsync(Expression<Func<T, bool>>? predicate = null,
                                Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                bool asNoTracking = true,
                                CancellationToken ct = default,
                                params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Efficiently checks if any entity matches the specified predicate without loading the entity.
        /// </summary>
        /// <param name="predicate">Expression defining the existence criteria</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>True if at least one entity matches the predicate, false otherwise</returns>
        /// <remarks>
        /// This method is optimized for existence checks and typically translates to SQL EXISTS queries,
        /// making it more efficient than loading entities just to check existence.
        /// </remarks>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        /// <summary>
        /// Efficiently counts entities matching the optional predicate.
        /// </summary>
        /// <param name="predicate">Optional filter expression. If null, counts all entities</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>The number of entities matching the criteria</returns>
        /// <remarks>
        /// This method translates to SQL COUNT queries for optimal performance without loading entities into memory.
        /// </remarks>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default);

        // CRUD Operations

        /// <summary>
        /// Adds a new entity to the context for insertion during the next save operation.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>A task representing the async operation</returns>
        /// <remarks>
        /// The entity is not immediately persisted to the database. Call SaveChanges on the Unit of Work to persist changes.
        /// </remarks>
        Task AddAsync(T entity, CancellationToken ct = default);

        /// <summary>
        /// Adds multiple entities to the context for insertion during the next save operation.
        /// </summary>
        /// <param name="entities">The collection of entities to add</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>A task representing the async operation</returns>
        /// <remarks>
        /// Bulk operation for improved performance when adding multiple entities.
        /// All entities are staged for insertion but not immediately persisted.
        /// </remarks>
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

        /// <summary>
        /// Marks an entity as modified for update during the next save operation.
        /// </summary>
        /// <param name="entity">The entity with updated values</param>
        /// <remarks>
        /// This synchronous method immediately marks the entity as modified in the context.
        /// The actual database update occurs when SaveChanges is called.
        /// </remarks>
        void Update(T entity);

        /// <summary>
        /// Asynchronous version of Update for consistency with the async pattern.
        /// </summary>
        /// <param name="entity">The entity with updated values</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>A completed task</returns>
        /// <remarks>
        /// Provides async signature compatibility but performs the same synchronous operation as Update.
        /// </remarks>
        Task UpdateAsync(T entity, CancellationToken ct = default);

        /// <summary>
        /// Marks an entity for deletion during the next save operation.
        /// </summary>
        /// <param name="entity">The entity to remove</param>
        /// <remarks>
        /// The entity is staged for deletion but not immediately removed from the database.
        /// Call SaveChanges on the Unit of Work to persist the deletion.
        /// </remarks>
        void Remove(T entity);

        /// <summary>
        /// Marks multiple entities for deletion during the next save operation.
        /// </summary>
        /// <param name="entities">The collection of entities to remove</param>
        /// <remarks>
        /// Bulk operation for improved performance when removing multiple entities.
        /// All entities are staged for deletion but not immediately removed.
        /// </remarks>
        void RemoveRange(IEnumerable<T> entities);
    }
}