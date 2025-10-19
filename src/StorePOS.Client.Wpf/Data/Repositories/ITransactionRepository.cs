using StorePOS.Client.Wpf.Data.Entities;
using StorePOS.Client.Wpf.Enums;

namespace StorePOS.Client.Wpf.Data.Repositories
{
    /// <summary>
    /// Repository interface for Transaction operations
    /// </summary>
    public interface ITransactionRepository : IRepository<TransactionEntity>
    {
        /// <summary>
        /// Gets all transactions for a specific shift
        /// </summary>
        Task<IEnumerable<TransactionEntity>> GetByShiftIdAsync(int shiftId);

        /// <summary>
        /// Gets all transactions for a specific wallet
        /// </summary>
        Task<IEnumerable<TransactionEntity>> GetByWalletIdAsync(int walletId);

        /// <summary>
        /// Gets transactions by type
        /// </summary>
        Task<IEnumerable<TransactionEntity>> GetByTypeAsync(TransactionType type);

        /// <summary>
        /// Gets transactions within a date range
        /// </summary>
        Task<IEnumerable<TransactionEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets the most recent transactions
        /// </summary>
        Task<IEnumerable<TransactionEntity>> GetRecentAsync(int count = 10);

        /// <summary>
        /// Gets transactions for the current day
        /// </summary>
        Task<IEnumerable<TransactionEntity>> GetTodayTransactionsAsync();
    }
}
