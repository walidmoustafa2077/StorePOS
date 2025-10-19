using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Services.Implementations
{
    /// <summary>
    /// Default implementation of IProductFilterService.
    /// Provides efficient filtering logic without blocking the UI thread.
    /// </summary>
    public class ProductFilterService : IProductFilterService
    {
        /// <inheritdoc />
        public async Task<List<ProductModel>> FilterProductsAsync(List<ProductModel> products, ProductFilterCriteria criteria, CancellationToken cancellationToken = default)
        {
            // Perform filtering on a background thread to avoid UI blocking
            return await Task.Run(() =>
            {
                var filtered = products.AsEnumerable();

                // Text search - case insensitive (includes Name, Description, SKU, Barcode, Brand, Category)
                if (!string.IsNullOrWhiteSpace(criteria.SearchText))
                {
                    var searchLower = criteria.SearchText.ToLowerInvariant();
                    filtered = filtered.Where(p =>
                        p.Name.ToLowerInvariant().Contains(searchLower) ||
                        p.Description.ToLowerInvariant().Contains(searchLower) ||
                        p.SKU.ToLowerInvariant().Contains(searchLower) ||
                        p.Barcode.ToLowerInvariant().Contains(searchLower) ||
                        p.Brand.ToLowerInvariant().Contains(searchLower) ||
                        p.Category.ToLowerInvariant().Contains(searchLower));
                }

                // Category filter
                if (!string.IsNullOrWhiteSpace(criteria.Category) && criteria.Category != "All Categories")
                {
                    filtered = filtered.Where(p => string.Equals(p.Category, criteria.Category, StringComparison.OrdinalIgnoreCase));
                }

                // Price range filter
                filtered = ApplyPriceRangeFilter(filtered, criteria.PriceRange);

                // Stock status filter
                filtered = ApplyStockStatusFilter(filtered, criteria.StockStatus);

                // Active only filter
                if (criteria.ShowActiveOnly)
                {
                    filtered = filtered.Where(p => p.IsActive);
                }

                // Check for cancellation before returning
                cancellationToken.ThrowIfCancellationRequested();

                return filtered.OrderBy(p => p.Name).ToList();
            }, cancellationToken);
        }

        private static IEnumerable<ProductModel> ApplyPriceRangeFilter(IEnumerable<ProductModel> products, string priceRange)
        {
            return priceRange switch
            {
                "Under $10" => products.Where(p => p.Price < 10),
                "$10 - $50" => products.Where(p => p.Price >= 10 && p.Price <= 50),
                "$50 - $100" => products.Where(p => p.Price > 50 && p.Price <= 100),
                "Over $100" => products.Where(p => p.Price > 100),
                _ => products
            };
        }

        private static IEnumerable<ProductModel> ApplyStockStatusFilter(IEnumerable<ProductModel> products, string stockStatus)
        {
            return stockStatus switch
            {
                "In Stock" => products.Where(p => !p.IsLowStock && !p.IsOutOfStock),
                "Low Stock" => products.Where(p => p.IsLowStock),
                "Out of Stock" => products.Where(p => p.IsOutOfStock),
                _ => products
            };
        }
    }
}