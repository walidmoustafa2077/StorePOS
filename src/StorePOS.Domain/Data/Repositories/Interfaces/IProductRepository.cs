using StorePOS.Domain.Models;

namespace StorePOS.Domain.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Product entities with specialized search capabilities.
    /// Extends the generic repository pattern with product-specific query operations
    /// optimized for Point-of-Sale scenarios.
    /// </summary>
    /// <remarks>
    /// This interface defines product-specific operations beyond the standard CRUD functionality:
    /// - Intelligent product search across multiple fields
    /// - Prioritized matching for different identifier types
    /// - Optimized for barcode scanning and manual lookup scenarios
    /// </remarks>
    public interface IProductRepository : IGenericRepository<Product>
    {
        /// <summary>
        /// Performs intelligent product search across SKU, Barcode, and Name fields with prioritized matching.
        /// </summary>
        /// <param name="query">The search term to match against product identifiers</param>
        /// <param name="ct">Cancellation token for async operation control</param>
        /// <returns>
        /// The best matching product based on the following priority:
        /// 1. Exact SKU match (highest priority)
        /// 2. Exact Barcode match (medium priority) 
        /// 3. Exact Name match (lower priority)
        /// 4. Partial match across any field (fallback)
        /// Returns null if no matches are found.
        /// </returns>
        /// <remarks>
        /// This method is specifically designed for Point-of-Sale operations where:
        /// - Barcode scanning requires immediate exact matches
        /// - Manual entry should support partial matching for user convenience
        /// - Consistent results are needed for predictable user experience
        /// 
        /// The search strategy ensures optimal performance for common POS scenarios:
        /// - Fast exact matching for barcode scans
        /// - Flexible partial matching for manual product lookup
        /// - Priority-based ordering to return the most relevant result
        /// </remarks>
        Task<Product?> GetAsync(string query, CancellationToken ct = default);
    }
}