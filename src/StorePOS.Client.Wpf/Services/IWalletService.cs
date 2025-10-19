using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Services
{
    /// <summary>
    /// Service interface for wallet business operations
    /// </summary>
    public interface IWalletService
    {
        /// <summary>
        /// Get all wallets
        /// </summary>
        Task<IEnumerable<WalletModel>> GetAllWalletsAsync();

        /// <summary>
        /// Get only active wallets
        /// </summary>
        Task<IEnumerable<WalletModel>> GetActiveWalletsAsync();

        /// <summary>
        /// Get wallet by ID
        /// </summary>
        Task<WalletModel?> GetWalletByIdAsync(int id);

        /// <summary>
        /// Get wallet by name
        /// </summary>
        Task<WalletModel?> GetWalletByNameAsync(string name);

        /// <summary>
        /// Create a new wallet
        /// </summary>
        Task<ServiceResult<WalletModel>> CreateWalletAsync(WalletModel wallet);

        /// <summary>
        /// Update an existing wallet
        /// </summary>
        Task<ServiceResult<WalletModel>> UpdateWalletAsync(WalletModel wallet);

        /// <summary>
        /// Delete a wallet
        /// </summary>
        Task<ServiceResult<bool>> DeleteWalletAsync(int id);

        /// <summary>
        /// Toggle wallet active status
        /// </summary>
        Task<ServiceResult<WalletModel>> ToggleWalletStatusAsync(int id);

        /// <summary>
        /// Update wallet balance
        /// </summary>
        Task<ServiceResult<WalletModel>> UpdateWalletBalanceAsync(int walletId, decimal amount);

        /// <summary>
        /// Get total balance across all active wallets
        /// </summary>
        Task<decimal> GetTotalBalanceAsync();

        /// <summary>
        /// Transfer money between wallets
        /// </summary>
        Task<ServiceResult<bool>> TransferBetweenWalletsAsync(int fromWalletId, int toWalletId, decimal amount, string note = "");
    }
}