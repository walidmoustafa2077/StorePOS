using Microsoft.EntityFrameworkCore;
using StorePOS.Client.Wpf.Data.Entities;

namespace StorePOS.Client.Wpf.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Wallet operations
    /// </summary>
    public class WalletRepository : Repository<WalletEntity>, IWalletRepository
    {
        public WalletRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<WalletEntity>> GetActiveAsync()
        {
            return await _dbSet.Where(w => w.IsActive).ToListAsync();
        }

        public async Task<WalletEntity?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(w => w.Name == name);
        }

        public async Task<decimal> GetTotalBalanceAsync()
        {
            return await _dbSet.Where(w => w.IsActive).SumAsync(w => w.Balance);
        }

        public async Task<bool> UpdateBalanceAsync(int walletId, decimal amount)
        {
            var wallet = await _dbSet.FindAsync(walletId);
            if (wallet == null) return false;

            wallet.Balance += amount;
            wallet.LastUpdated = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}