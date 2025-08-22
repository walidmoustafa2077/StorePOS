using StorePOS.Domain.DTOs;

namespace StorePOS.Domain.Services
{
    /// <summary>
    /// Service interface for product management operations in the Point-of-Sale system.
    /// Provides comprehensive product lifecycle management including inventory control, barcode lookup, and stock management.
    /// </summary>
    /// <remarks>
    /// This service handles all product-related business logic including:
    /// - Product catalog management
    /// - Barcode-based product lookup for POS operations
    /// - Stock level tracking and updates
    /// - Product search and filtering
    /// - Category management integration
    /// 
    /// All operations return standardized ServiceResult objects for consistent error handling.
    /// Stock management operations are critical for inventory accuracy and sales processing.
    /// </remarks>
    public interface IProductService
    {
        /// <summary>
        /// Searches for products based on a query string, matching product name, barcode, or category.
        /// </summary>
        /// <param name="q">Search query string to match against product fields</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>List of products matching the search criteria</returns>
        /// <remarks>
        /// Search behavior:
        /// - Case-insensitive partial matching across product name and description
        /// - Exact matching for barcode lookups
        /// - Category-based filtering support
        /// - Results ordered by relevance and product name
        /// - Includes active products only
        /// - Optimized for POS quick product lookup scenarios
        /// </remarks>
        Task<List<ProductReadDto>> SearchAsync(string? q, CancellationToken ct);

        /// <summary>
        /// Retrieves a product by its unique barcode identifier for Point-of-Sale operations.
        /// </summary>
        /// <param name="barcode">The barcode to search for (UPC, EAN, or internal code)</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Product data if found, null otherwise</returns>
        /// <remarks>
        /// Barcode lookup features:
        /// - Exact match required for barcode field
        /// - Critical for POS scanning operations
        /// - Returns complete product information including pricing and stock
        /// - Validates product availability status
        /// - Optimized for high-frequency POS operations
        /// </remarks>
        Task<ProductReadDto?> GetByBarcodeAsync(string barcode, CancellationToken ct);

        /// <summary>
        /// Creates a new product in the catalog with the provided information.
        /// </summary>
        /// <param name="dto">Product creation data including name, barcode, pricing, and category</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result containing the created product data or validation errors</returns>
        /// <remarks>
        /// Creation validation:
        /// - Barcode uniqueness verification
        /// - Product name validation
        /// - Category existence verification
        /// - Pricing validation (positive values)
        /// - Stock level initialization
        /// - Required field validation
        /// </remarks>
        Task<ServiceResult<ProductReadDto>> CreateAsync(ProductCreateDto dto, CancellationToken ct);

        /// <summary>
        /// Updates an existing product's information excluding stock levels.
        /// </summary>
        /// <param name="id">The ID of the product to update</param>
        /// <param name="dto">Updated product information</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result containing the updated product data or validation errors</returns>
        /// <remarks>
        /// Update validation:
        /// - Product existence verification
        /// - Barcode uniqueness (excluding current product)
        /// - Category existence verification
        /// - Pricing validation
        /// - Preserves stock levels (use UpdateStockAsync for stock changes)
        /// - Maintains product history and audit trail
        /// </remarks>
        Task<ServiceResult<ProductReadDto>> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken ct);

        /// <summary>
        /// Updates a product's stock level with proper inventory tracking.
        /// </summary>
        /// <param name="id">The ID of the product to update stock for</param>
        /// <param name="dto">Stock update information including quantity and operation type</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result containing the updated product data or validation errors</returns>
        /// <remarks>
        /// Stock management features:
        /// - Supports both absolute and relative stock updates
        /// - Validates stock levels for negative values (configurable)
        /// - Creates inventory audit trail
        /// - Handles concurrent stock modifications
        /// - Integrates with sales processing for automatic deductions
        /// - Supports bulk stock operations
        /// </remarks>
        Task<ServiceResult<ProductReadDto>> UpdateStockAsync(int id, ProductStockUpdateDto dto, CancellationToken ct);

        /// <summary>
        /// Soft deletes a product by marking it as inactive while preserving historical data.
        /// </summary>
        /// <param name="id">The ID of the product to delete</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result indicating success or failure</returns>
        /// <remarks>
        /// Deletion behavior:
        /// - Soft deletion (marks product as inactive)
        /// - Preserves historical sales data integrity
        /// - Prevents new sales of the product
        /// - Maintains barcode for reference
        /// - May require special handling for products with active stock
        /// - Supports restoration if needed
        /// </remarks>
        Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken ct);
    }
}
