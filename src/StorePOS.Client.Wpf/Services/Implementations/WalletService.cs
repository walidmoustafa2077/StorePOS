using StorePOS.Client.Wpf.Data.Repositories;
using StorePOS.Client.Wpf.Data.Mapping;
using StorePOS.Client.Wpf.MVVM.Models;
using Microsoft.Extensions.Logging;

namespace StorePOS.Client.Wpf.Services.Implementations
{
    /// <summary>
    /// Implementation of IWalletService providing wallet business operations
    /// </summary>
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ILogger<WalletService> _logger;

        public WalletService(
            IWalletRepository walletRepository,
            ILogger<WalletService> logger)
        {
            _walletRepository = walletRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<WalletModel>> GetAllWalletsAsync()
        {
            try
            {
                var entities = await _walletRepository.GetAllAsync();
                return entities.Select(e => e.ToModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all wallets");
                throw;
            }
        }

        public async Task<IEnumerable<WalletModel>> GetActiveWalletsAsync()
        {
            try
            {
                var entities = await _walletRepository.GetActiveAsync();
                return entities.Select(e => e.ToModel());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting active wallets");
                throw;
            }
        }

        public async Task<WalletModel?> GetWalletByIdAsync(int id)
        {
            try
            {
                var entity = await _walletRepository.GetByIdAsync(id);
                return entity?.ToModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting wallet by ID: {WalletId}", id);
                throw;
            }
        }

        public async Task<WalletModel?> GetWalletByNameAsync(string name)
        {
            try
            {
                var entity = await _walletRepository.GetByNameAsync(name);
                return entity?.ToModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting wallet by name: {WalletName}", name);
                throw;
            }
        }

        public async Task<ServiceResult<WalletModel>> CreateWalletAsync(WalletModel wallet)
        {
            try
            {
                // Validate wallet data
                var validationErrors = ValidateWallet(wallet);
                if (validationErrors.Any())
                {
                    return ServiceResult<WalletModel>.ValidationFailure(validationErrors);
                }

                // Check if wallet with same name already exists
                var existingWallet = await _walletRepository.GetByNameAsync(wallet.Name);
                if (existingWallet != null)
                {
                    return ServiceResult<WalletModel>.Failure("A wallet with this name already exists");
                }

                var entity = wallet.ToEntity();
                var savedEntity = await _walletRepository.AddAsync(entity);
                var savedModel = savedEntity.ToModel();

                _logger.LogInformation("Created new wallet: {WalletName} (ID: {WalletId})", 
                    savedModel.Name, savedModel.Id);

                return ServiceResult<WalletModel>.Success(savedModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating wallet: {WalletName}", wallet.Name);
                return ServiceResult<WalletModel>.Failure($"Failed to create wallet: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WalletModel>> UpdateWalletAsync(WalletModel wallet)
        {
            try
            {
                // Validate wallet data
                var validationErrors = ValidateWallet(wallet);
                if (validationErrors.Any())
                {
                    return ServiceResult<WalletModel>.ValidationFailure(validationErrors);
                }

                var existingEntity = await _walletRepository.GetByIdAsync(wallet.Id);
                if (existingEntity == null)
                {
                    return ServiceResult<WalletModel>.Failure("Wallet not found");
                }

                // Check if wallet with same name already exists (excluding current wallet)
                var existingWallet = await _walletRepository.GetByNameAsync(wallet.Name);
                if (existingWallet != null && existingWallet.Id != wallet.Id)
                {
                    return ServiceResult<WalletModel>.Failure("A wallet with this name already exists");
                }

                existingEntity.UpdateFromModel(wallet);
                var updatedEntity = await _walletRepository.UpdateAsync(existingEntity);
                var updatedModel = updatedEntity.ToModel();

                _logger.LogInformation("Updated wallet: {WalletName} (ID: {WalletId})", 
                    updatedModel.Name, updatedModel.Id);

                return ServiceResult<WalletModel>.Success(updatedModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating wallet: {WalletId}", wallet.Id);
                return ServiceResult<WalletModel>.Failure($"Failed to update wallet: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteWalletAsync(int id)
        {
            try
            {
                var wallet = await _walletRepository.GetByIdAsync(id);
                if (wallet == null)
                {
                    return ServiceResult<bool>.Failure("Wallet not found");
                }

                // Check if wallet has balance
                if (wallet.Balance != 0)
                {
                    return ServiceResult<bool>.Failure("Cannot delete wallet with non-zero balance");
                }

                var deleted = await _walletRepository.DeleteAsync(id);
                if (!deleted)
                {
                    return ServiceResult<bool>.Failure("Failed to delete wallet");
                }

                _logger.LogInformation("Deleted wallet: {WalletName} (ID: {WalletId})", wallet.Name, id);
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting wallet: {WalletId}", id);
                return ServiceResult<bool>.Failure($"Failed to delete wallet: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WalletModel>> ToggleWalletStatusAsync(int id)
        {
            try
            {
                var entity = await _walletRepository.GetByIdAsync(id);
                if (entity == null)
                {
                    return ServiceResult<WalletModel>.Failure("Wallet not found");
                }

                entity.IsActive = !entity.IsActive;
                entity.LastUpdated = DateTime.Now;

                var updatedEntity = await _walletRepository.UpdateAsync(entity);
                var updatedModel = updatedEntity.ToModel();

                _logger.LogInformation("Toggled wallet status: {WalletName} (ID: {WalletId}) - Active: {IsActive}", 
                    updatedModel.Name, updatedModel.Id, updatedModel.IsActive);

                return ServiceResult<WalletModel>.Success(updatedModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling wallet status: {WalletId}", id);
                return ServiceResult<WalletModel>.Failure($"Failed to toggle wallet status: {ex.Message}");
            }
        }

        public async Task<ServiceResult<WalletModel>> UpdateWalletBalanceAsync(int walletId, decimal amount)
        {
            try
            {
                var entity = await _walletRepository.GetByIdAsync(walletId);
                if (entity == null)
                {
                    return ServiceResult<WalletModel>.Failure("Wallet not found");
                }

                if (!entity.IsActive)
                {
                    return ServiceResult<WalletModel>.Failure("Cannot update balance for inactive wallet");
                }

                var newBalance = entity.Balance + amount;
                if (newBalance < 0)
                {
                    return ServiceResult<WalletModel>.Failure("Insufficient balance");
                }

                var updated = await _walletRepository.UpdateBalanceAsync(walletId, amount);
                if (!updated)
                {
                    return ServiceResult<WalletModel>.Failure("Failed to update wallet balance");
                }

                // Refresh entity to get updated balance
                entity = await _walletRepository.GetByIdAsync(walletId);
                var updatedModel = entity!.ToModel();

                _logger.LogInformation("Updated wallet balance: {WalletName} (ID: {WalletId}) - Amount: {Amount}, New Balance: {Balance}", 
                    updatedModel.Name, walletId, amount, updatedModel.Balance);

                return ServiceResult<WalletModel>.Success(updatedModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating wallet balance: {WalletId}", walletId);
                return ServiceResult<WalletModel>.Failure($"Failed to update wallet balance: {ex.Message}");
            }
        }

        public async Task<decimal> GetTotalBalanceAsync()
        {
            try
            {
                return await _walletRepository.GetTotalBalanceAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting total balance");
                throw;
            }
        }

        public async Task<ServiceResult<bool>> TransferBetweenWalletsAsync(int fromWalletId, int toWalletId, decimal amount, string note = "")
        {
            try
            {
                if (amount <= 0)
                {
                    return ServiceResult<bool>.Failure("Transfer amount must be positive");
                }

                if (fromWalletId == toWalletId)
                {
                    return ServiceResult<bool>.Failure("Cannot transfer to the same wallet");
                }

                var fromWallet = await _walletRepository.GetByIdAsync(fromWalletId);
                var toWallet = await _walletRepository.GetByIdAsync(toWalletId);

                if (fromWallet == null || toWallet == null)
                {
                    return ServiceResult<bool>.Failure("One or both wallets not found");
                }

                if (!fromWallet.IsActive || !toWallet.IsActive)
                {
                    return ServiceResult<bool>.Failure("Both wallets must be active for transfer");
                }

                if (fromWallet.Balance < amount)
                {
                    return ServiceResult<bool>.Failure("Insufficient balance in source wallet");
                }

                // Perform the transfer
                await _walletRepository.UpdateBalanceAsync(fromWalletId, -amount);
                await _walletRepository.UpdateBalanceAsync(toWalletId, amount);

                _logger.LogInformation("Transferred {Amount} from wallet {FromWalletId} to wallet {ToWalletId}. Note: {Note}", 
                    amount, fromWalletId, toWalletId, note);

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while transferring between wallets: {FromWalletId} -> {ToWalletId}", 
                    fromWalletId, toWalletId);
                return ServiceResult<bool>.Failure($"Failed to transfer between wallets: {ex.Message}");
            }
        }

        private static List<string> ValidateWallet(WalletModel wallet)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(wallet.Name))
            {
                errors.Add("Wallet name is required");
            }
            else if (wallet.Name.Length > 100)
            {
                errors.Add("Wallet name must be 100 characters or less");
            }

            if (!string.IsNullOrEmpty(wallet.Description) && wallet.Description.Length > 500)
            {
                errors.Add("Wallet description must be 500 characters or less");
            }

            return errors;
        }
    }
}