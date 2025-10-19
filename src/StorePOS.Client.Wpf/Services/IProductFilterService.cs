using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Services
{
    /// <summary>
    /// Interface for filtering products based on various criteria.
    /// Follows Single Responsibility Principle by handling only filtering logic.
    /// </summary>
    public interface IProductFilterService
    {
        /// <summary>
        /// Filters products asynchronously based on the provided criteria.
        /// </summary>
        /// <param name="products">The list of products to filter</param>
        /// <param name="criteria">The filtering criteria</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>Filtered list of products</returns>
        Task<List<ProductModel>> FilterProductsAsync(List<ProductModel> products, ProductFilterCriteria criteria, CancellationToken cancellationToken = default);
    }
}