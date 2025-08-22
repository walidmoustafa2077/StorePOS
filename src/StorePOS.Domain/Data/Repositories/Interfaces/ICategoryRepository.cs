using StorePOS.Domain.Models;

namespace StorePOS.Domain.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Category entities with specialized lookup operations for product categorization.
    /// Extends the generic repository pattern with category-specific query methods.
    /// </summary>
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        /// <summary>
        /// Retrieves a category by its unique name identifier.
        /// </summary>
        /// <param name="name">The exact name of the category to locate</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>
        /// The Category entity with matching name, or null if not found.
        /// Uses case-sensitive comparison and no-tracking query for optimal performance.
        /// </returns>
        /// <remarks>
        /// This method supports category lookup scenarios such as:
        /// - Product import/sync operations
        /// - Category validation during product creation
        /// - Administrative category management
        /// 
        /// Consider implementing case-insensitive search if business requirements demand it.
        /// For bulk operations, prefer using the generic repository's batch methods.
        /// </remarks>
        Task<Category?> GetByNameAsync(string name, CancellationToken ct = default);
    }
}