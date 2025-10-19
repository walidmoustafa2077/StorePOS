using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Services
{
    /// <summary>
    /// Service interface for sale business operations
    /// </summary>
    public interface ISaleService
    {
        /// <summary>
        /// Get all sales
        /// </summary>
        Task<IEnumerable<SaleModel>> GetAllSalesAsync();

        /// <summary>
        /// Get sale by ID
        /// </summary>
        Task<SaleModel?> GetSaleByIdAsync(int id);

        /// <summary>
        /// Get sales by date range
        /// </summary>
        Task<IEnumerable<SaleModel>> GetSalesByDateRangeAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Get sales for current shift
        /// </summary>
        Task<IEnumerable<SaleModel>> GetSalesForCurrentShiftAsync();

        /// <summary>
        /// Create a new sale transaction
        /// </summary>
        Task<ServiceResult<SaleModel>> CreateSaleAsync(SaleModel sale);

        /// <summary>
        /// Cancel/void a sale
        /// </summary>
        Task<ServiceResult<bool>> CancelSaleAsync(int saleId, string reason);

        /// <summary>
        /// Process partial or full refund for selected items
        /// </summary>
        Task<ServiceResult<bool>> ProcessRefundAsync(int saleId, IEnumerable<RefundItemModel> refundItems, string reason, string refundedBy);

        /// <summary>
        /// Get today's sales summary
        /// </summary>
        Task<SalesSummaryModel> GetTodaysSalesSummaryAsync();

        /// <summary>
        /// Get sales summary for date range
        /// </summary>
        Task<SalesSummaryModel> GetSalesSummaryAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Process cart checkout
        /// </summary>
        Task<ServiceResult<SaleModel>> ProcessCheckoutAsync(IEnumerable<CartItemModel> cartItems, decimal totalAmount, string paymentMethod);

        /// <summary>
        /// Validate cart items before checkout
        /// </summary>
        Task<ServiceResult<bool>> ValidateCartAsync(IEnumerable<CartItemModel> cartItems);

        /// <summary>
        /// Calculate cart totals
        /// </summary>
        Task<CartTotalsModel> CalculateCartTotalsAsync(IEnumerable<CartItemModel> cartItems);

        /// <summary>
        /// Get best selling products
        /// </summary>
        Task<IEnumerable<ProductSalesModel>> GetBestSellingProductsAsync(DateTime fromDate, DateTime toDate, int topCount = 10);

        /// <summary>
        /// Generate receipt for sale
        /// </summary>
        Task<ReceiptModel> GenerateReceiptAsync(int saleId);
    }
}