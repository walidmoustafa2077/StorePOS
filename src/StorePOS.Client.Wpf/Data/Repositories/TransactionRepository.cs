using Microsoft.EntityFrameworkCore;
using StorePOS.Client.Wpf.Data.Entities;
using StorePOS.Client.Wpf.Enums;

namespace StorePOS.Client.Wpf.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Transaction operations
    /// </summary>
    public class TransactionRepository : Repository<TransactionEntity>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TransactionEntity>> GetByShiftIdAsync(int shiftId)
        {
            return await _dbSet
                .Include(t => t.Wallet)
                .Include(t => t.Shift)
                .Where(t => t.ShiftId == shiftId)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionEntity>> GetByWalletIdAsync(int walletId)
        {
            return await _dbSet
                .Include(t => t.Wallet)
                .Include(t => t.Shift)
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionEntity>> GetByTypeAsync(TransactionType type)
        {
            return await _dbSet
                .Include(t => t.Wallet)
                .Include(t => t.Shift)
                .Where(t => t.Type == type)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(t => t.Wallet)
                .Include(t => t.Shift)
                .Where(t => t.Timestamp >= startDate && t.Timestamp <= endDate)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionEntity>> GetRecentAsync(int count = 10)
        {
            return await _dbSet
                .Include(t => t.Wallet)
                .Include(t => t.Shift)
                .OrderByDescending(t => t.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionEntity>> GetTodayTransactionsAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await _dbSet
                .Include(t => t.Wallet)
                .Include(t => t.Shift)
                .Where(t => t.Timestamp >= today && t.Timestamp < tomorrow)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }
    }
}
