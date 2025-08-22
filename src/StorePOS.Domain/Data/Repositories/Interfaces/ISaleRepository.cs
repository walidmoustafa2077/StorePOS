using StorePOS.Domain.Models;
using System.Linq.Expressions;

namespace StorePOS.Domain.Data.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for Sale entities with specialized query operations for Point-of-Sale systems.
    /// Extends the generic repository pattern with sales-specific functionality including cart line management.
    /// </summary>
    public interface ISaleRepository : IGenericRepository<Sale>
    {
        /// <summary>
        /// Retrieves a sale record with its associated cart lines and optionally product details.
        /// </summary>
        /// <param name="id">The unique identifier of the sale to retrieve</param>
        /// <param name="includeProducts">
        /// When true, includes full product information for each cart line.
        /// When false, only includes cart line data without product details.
        /// </param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>
        /// A Sale entity with populated cart lines, or null if not found.
        /// Uses no-tracking queries for optimal read performance.
        /// </returns>
        /// <remarks>
        /// This method is optimized for sale detail views where cart composition is required.
        /// Product inclusion is optional to support different UI scenarios:
        /// - Receipt generation (products needed)
        /// - Quick sale summary (products not needed)
        /// </remarks>
        Task<Sale?> GetWithLinesAsync(object id, bool includeProducts = false, CancellationToken ct = default);

        /// <summary>
        /// Retrieves multiple sales with cart lines based on optional filtering criteria.
        /// </summary>
        /// <param name="predicate">
        /// Optional filter expression to limit results (e.g., date range, status, customer).
        /// When null, returns all sales with cart lines.
        /// </param>
        /// <param name="includeProducts">
        /// When true, includes full product information for each cart line.
        /// When false, only includes cart line data without product details.
        /// </param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>
        /// A list of Sale entities with populated cart lines matching the criteria.
        /// Uses no-tracking queries for optimal read performance.
        /// </returns>
        /// <remarks>
        /// Designed for reporting and listing scenarios such as:
        /// - Daily sales reports
        /// - Product movement analysis
        /// - Customer purchase history
        /// Consider performance implications when querying large date ranges with product inclusion.
        /// </remarks>
        Task<List<Sale>> ListWithLinesAsync(Expression<Func<Sale, bool>>? predicate = null,
                                            bool includeProducts = false,
                                            CancellationToken ct = default);
    }
}