using Microsoft.EntityFrameworkCore;
using StorePOS.Domain.Data;
using StorePOS.Domain.Data.Repositories.Interfaces;
using StorePOS.Domain.Models;

namespace StorePOS.Domain.Repositories
{
    /// <summary>
    /// Repository implementation for Product entities with specialized search capabilities.
    /// Extends the generic repository to provide product-specific query operations optimized for POS scenarios.
    /// </summary>
    /// <remarks>
    /// This repository implements intelligent product search functionality designed for Point-of-Sale operations:
    /// - Prioritized exact match searching (SKU > Barcode > Name)
    /// - Fallback partial matching for flexible product lookup
    /// - Optimized for barcode scanning and manual product search scenarios
    /// - Performance-optimized queries using AsNoTracking for read operations
    /// </remarks>
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        /// <summary>
        /// Initializes a new instance of the ProductRepository with the specified database context.
        /// </summary>
        /// <param name="db">The database context for data access operations</param>
        public ProductRepository(AppDbContext db) : base(db) { }

        /// <inheritdoc />
        /// <remarks>
        /// Implements intelligent product search with two-phase matching strategy:
        /// 
        /// Phase 1 - Exact Match Priority:
        /// 1. SKU exact match (highest priority - primary identifier)
        /// 2. Barcode exact match (medium priority - scanning operations)
        /// 3. Name exact match (lower priority - manual lookup)
        /// 
        /// Phase 2 - Partial Match Fallback:
        /// If no exact matches found, performs partial matching across all fields
        /// using SQL LIKE operations, ordered by name for consistent results.
        /// 
        /// This approach optimizes for common POS scenarios:
        /// - Barcode scanning returns immediate exact matches
        /// - Manual entry can find products by partial SKU/name
        /// - Consistent ordering for predictable user experience
        /// </remarks>
        public async Task<Product?> GetAsync(string query, CancellationToken ct = default)
        {
            // Input validation - return null for invalid queries
            if (string.IsNullOrWhiteSpace(query))
                return null;

            query = query.Trim();

            // Phase 1: Exact match priority search
            // Order by match priority: SKU (highest) -> Barcode -> Name (lowest)
            var exact = await _set.AsNoTracking()
                                  .Where(p => p.Sku == query || p.Barcode == query || p.Name == query)
                                  .OrderByDescending(p => p.Sku == query)        // SKU match gets highest priority
                                  .ThenByDescending(p => p.Barcode == query)     // Barcode match gets medium priority
                                  .ThenByDescending(p => p.Name == query)        // Name match gets lowest priority
                                  .FirstOrDefaultAsync(ct);

            if (exact is not null)
                return exact;

            // Phase 2: Partial match fallback for flexible searching
            // Uses SQL LIKE operations for substring matching across key fields
            return await _set.AsNoTracking()
                             .Where(p => p.Sku.Contains(query) || p.Barcode.Contains(query) || p.Name.Contains(query))
                             .OrderBy(p => p.Name)  // Consistent ordering for predictable results
                             .FirstOrDefaultAsync(ct);
        }
    }
}