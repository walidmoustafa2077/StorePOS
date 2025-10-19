using StorePOS.Client.Wpf.Data.Entities;

namespace StorePOS.Client.Wpf.Data.Repositories
{
    /// <summary>
    /// Repository interface for Sale operations
    /// </summary>
    public interface ISaleRepository : IRepository<SaleEntity>
    {
        Task<IEnumerable<SaleEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<SaleEntity>> GetTodaysSalesAsync();
        Task<SaleEntity?> GetByNumberAsync(string saleNumber);
        Task<SaleEntity?> GetWithItemsAsync(int saleId);
        Task<decimal> GetTotalSalesAmountAsync(DateTime startDate, DateTime endDate);
        Task<int> GetTotalSalesCountAsync(DateTime startDate, DateTime endDate);
        Task<string> GenerateNextSaleNumberAsync();
        
        // Shift-related methods
        Task<IEnumerable<SaleEntity>> GetByShiftIdAsync(int shiftId);
        Task<decimal> GetTotalSalesAmountByShiftAsync(int shiftId);
        Task<int> GetTotalSalesCountByShiftAsync(int shiftId);

        // Refund-related methods
        Task<SaleEntity> UpdateSaleWithItemsAsync(SaleEntity sale);
    }
}