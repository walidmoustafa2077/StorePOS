using StorePOS.Client.Wpf.Data.Entities;

namespace StorePOS.Client.Wpf.Data.Repositories
{
    /// <summary>
    /// Repository interface for Wallet operations
    /// </summary>
    public interface IWalletRepository : IRepository<WalletEntity>
    {
        Task<IEnumerable<WalletEntity>> GetActiveAsync();
        Task<WalletEntity?> GetByNameAsync(string name);
        Task<decimal> GetTotalBalanceAsync();
        Task<bool> UpdateBalanceAsync(int walletId, decimal amount);
    }
}