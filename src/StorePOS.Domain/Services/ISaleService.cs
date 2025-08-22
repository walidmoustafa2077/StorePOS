using StorePOS.Domain.DTOs;

namespace StorePOS.Domain.Services
{
    /// <summary>
    /// Service interface for sales transaction management in the Point-of-Sale system.
    /// Provides comprehensive sales lifecycle management including transaction processing, inventory integration, and sales analytics.
    /// </summary>
    /// <remarks>
    /// This service orchestrates the complete sales process including:
    /// - Transaction creation and management
    /// - Cart composition and pricing calculation
    /// - Inventory deduction and stock management
    /// - Payment processing integration
    /// - Sales reporting and analytics
    /// - Transaction state management (pending, completed, cancelled)
    /// 
    /// All operations maintain ACID properties and integrate with inventory management.
    /// Critical for business operations and financial accuracy.
    /// </remarks>
    public interface ISaleService
    {
        /// <summary>
        /// Searches for sales transactions within an optional date range for reporting and analysis.
        /// </summary>
        /// <param name="from">Optional start date for filtering sales (inclusive)</param>
        /// <param name="to">Optional end date for filtering sales (inclusive)</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>List of sales matching the date criteria, ordered by sale date</returns>
        /// <remarks>
        /// Search capabilities:
        /// - Date range filtering for reporting periods
        /// - Returns complete sale information including cart lines
        /// - Supports open-ended date ranges (from only, to only, or both)
        /// - Optimized for reporting and analytics queries
        /// - Includes all sale statuses (completed, cancelled, pending)
        /// - Results ordered chronologically for analysis
        /// </remarks>
        Task<List<SaleReadDto>> SearchAsync(DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a complete sales transaction by its unique identifier.
        /// </summary>
        /// <param name="id">The unique ID of the sale to retrieve</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Complete sale data including cart lines and product details, or null if not found</returns>
        /// <remarks>
        /// Includes comprehensive sale information:
        /// - All cart line items with product details
        /// - Payment information and totals
        /// - Customer information if available
        /// - Sale status and timestamps
        /// - User/cashier information
        /// </remarks>
        Task<SaleReadDto?> GetByIdAsync(int id, CancellationToken ct = default);
        
        /// <summary>
        /// Creates a new sales transaction with the provided cart items and customer information.
        /// </summary>
        /// <param name="dto">Sale creation data including cart items, customer, and payment details</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result containing the created sale data or validation errors</returns>
        /// <remarks>
        /// Transaction creation process:
        /// - Validates all products exist and are available
        /// - Calculates totals and applies any discounts
        /// - Creates pending sale record
        /// - Does not affect inventory until completion
        /// - Validates payment amount and method
        /// - Creates audit trail for transaction
        /// </remarks>
        Task<ServiceResult<SaleReadDto>> CreateAsync(SaleCreateDto dto, CancellationToken ct = default);

        /// <summary>
        /// Updates a pending sales transaction with modified cart items or customer information.
        /// </summary>
        /// <param name="id">The ID of the sale to update</param>
        /// <param name="dto">Updated sale information</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result containing the updated sale data or validation errors</returns>
        /// <remarks>
        /// Update restrictions:
        /// - Only pending sales can be modified
        /// - Completed or cancelled sales cannot be updated
        /// - Recalculates totals when cart items change
        /// - Validates product availability for new/modified items
        /// - Maintains transaction integrity
        /// </remarks>
        Task<ServiceResult<SaleReadDto>> UpdateAsync(int id, SaleUpdateDto dto, CancellationToken ct = default);

        /// <summary>
        /// Soft deletes a sales transaction, marking it as cancelled while preserving data for audit purposes.
        /// </summary>
        /// <param name="id">The ID of the sale to delete</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result indicating success or failure</returns>
        /// <remarks>
        /// Deletion behavior:
        /// - Only pending sales can be deleted
        /// - Marks sale as cancelled rather than physical deletion
        /// - Preserves data for audit and reporting
        /// - Does not affect inventory (no stock restoration)
        /// - Maintains referential integrity
        /// </remarks>
        Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken ct = default);
        
        /// <summary>
        /// Completes a pending sales transaction by finalizing payment and reducing product inventory.
        /// </summary>
        /// <param name="saleId">The ID of the sale to complete</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result containing the completed sale data or failure information</returns>
        /// <remarks>
        /// Completion process:
        /// - Validates sale is in pending status
        /// - Verifies product availability and stock levels
        /// - Reduces inventory for all cart items atomically
        /// - Marks sale as completed with timestamp
        /// - Processes payment finalization
        /// - Creates inventory transaction records
        /// - Updates product stock levels
        /// - All operations are transactional (ACID compliant)
        /// </remarks>
        Task<ServiceResult<SaleReadDto>> CompleteSaleAsync(int saleId, CancellationToken ct = default);
        
        /// <summary>
        /// Cancels a sales transaction, marking it as cancelled and handling any necessary cleanup.
        /// </summary>
        /// <param name="saleId">The ID of the sale to cancel</param>
        /// <param name="ct">Cancellation token for async operation</param>
        /// <returns>Service result containing the cancelled sale data or failure information</returns>
        /// <remarks>
        /// Cancellation process:
        /// - Can cancel pending or even completed sales (with proper authorization)
        /// - For completed sales, may restore inventory depending on business rules
        /// - Marks sale as cancelled with timestamp and reason
        /// - Handles payment reversal if applicable
        /// - Creates audit trail for cancellation
        /// - May require manager authorization for completed sales
        /// </remarks>
        Task<ServiceResult<SaleReadDto>> CancelSaleAsync(int saleId, CancellationToken ct = default);
    }
}
