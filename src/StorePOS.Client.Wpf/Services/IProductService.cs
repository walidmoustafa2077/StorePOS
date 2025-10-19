using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Services
{
    /// <summary>
    /// Service interface for product business operations
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Get all products
        /// </summary>
        Task<IEnumerable<ProductModel>> GetAllProductsAsync();

        /// <summary>
        /// Get only active products
        /// </summary>
        Task<IEnumerable<ProductModel>> GetActiveProductsAsync();

        /// <summary>
        /// Get product by ID
        /// </summary>
        Task<ProductModel?> GetProductByIdAsync(int id);

        /// <summary>
        /// Get product by SKU
        /// </summary>
        Task<ProductModel?> GetProductBySkuAsync(string sku);

        /// <summary>
        /// Get product by barcode
        /// </summary>
        Task<ProductModel?> GetProductByBarcodeAsync(string barcode);

        /// <summary>
        /// Get products by category
        /// </summary>
        Task<IEnumerable<ProductModel>> GetProductsByCategoryAsync(string category);

        /// <summary>
        /// Create a new product
        /// </summary>
        Task<ServiceResult<ProductModel>> CreateProductAsync(ProductModel product);

        /// <summary>
        /// Update an existing product
        /// </summary>
        Task<ServiceResult<ProductModel>> UpdateProductAsync(ProductModel product);

        /// <summary>
        /// Delete a product (soft delete by setting IsActive = false)
        /// </summary>
        Task<ServiceResult<bool>> DeleteProductAsync(int id);

        /// <summary>
        /// Search products by name, SKU, or barcode
        /// </summary>
        Task<IEnumerable<ProductModel>> SearchProductsAsync(string searchTerm);

        /// <summary>
        /// Check if product SKU already exists
        /// </summary>
        Task<bool> IsSkuExistsAsync(string sku, int? excludeId = null);

        /// <summary>
        /// Check if product barcode already exists
        /// </summary>
        Task<bool> IsBarcodeExistsAsync(string barcode, int? excludeId = null);

        /// <summary>
        /// Update product stock quantity
        /// </summary>
        Task<ServiceResult<bool>> UpdateStockAsync(int productId, int newQuantity);

        /// <summary>
        /// Check if product has sufficient stock
        /// </summary>
        Task<bool> HasSufficientStockAsync(int productId, int requiredQuantity);

        /// <summary>
        /// Get low stock products (below minimum threshold)
        /// </summary>
        Task<IEnumerable<ProductModel>> GetLowStockProductsAsync(int threshold = 10);

        /// <summary>
        /// Calculate product profit margin
        /// </summary>
        decimal CalculateProfitMargin(ProductModel product);

        /// <summary>
        /// Calculate discounted price
        /// </summary>
        decimal CalculateDiscountedPrice(ProductModel product);
    }
}