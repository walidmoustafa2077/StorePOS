using StorePOS.Client.Wpf.Data.Entities;
using StorePOS.Client.Wpf.Data.Repositories;
using StorePOS.Client.Wpf.Enums;
using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Services.Implementations
{
    /// <summary>
    /// Service implementation for handling transaction operations
    /// Automatically updates database for non-cash wallet transactions
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IShiftService _shiftService;

        public event EventHandler<TransactionModel>? SaleProcessed;
        public event EventHandler<TransactionModel>? TransactionAdded;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IWalletRepository walletRepository,
            IShiftService shiftService)
        {
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            _shiftService = shiftService;
        }

        public async Task<bool> AddTransactionAsync(TransactionModel transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            try
            {
                // Get current shift
                var currentShift = await _shiftService.GetCurrentShiftAsync();

                // Get wallet by name or ID
                WalletEntity? wallet = null;
                if (transaction.Wallet?.Id > 0)
                {
                    wallet = await _walletRepository.GetByIdAsync(transaction.Wallet.Id);
                }
                else if (!string.IsNullOrWhiteSpace(transaction.Wallet?.Name))
                {
                    wallet = await _walletRepository.GetByNameAsync(transaction.Wallet.Name);
                }

                if (wallet == null)
                {
                    throw new InvalidOperationException($"Wallet not found: {transaction.Wallet?.Name ?? "Unknown"}");
                }

                // Create transaction entity
                var transactionEntity = new TransactionEntity
                {
                    Type = transaction.Type,
                    Amount = transaction.Amount,
                    Description = transaction.Description,
                    Reference = transaction.Reference,
                    Timestamp = transaction.Timestamp,
                    ShiftId = currentShift?.Id,
                    WalletId = wallet.Id
                };

                // Save transaction to database
                var savedTransaction = await _transactionRepository.AddAsync(transactionEntity);

                // Update transaction model with the generated ID
                transaction.Id = savedTransaction.Id;

                // Update wallet balance in database
                // Calculate the balance change based on transaction type
                decimal balanceChange = transaction.Type switch
                {
                    TransactionType.Sale => transaction.Amount,        // Money in
                    TransactionType.CashAdd => transaction.Amount,     // Money in
                    TransactionType.PayIn => transaction.Amount,       // Money in
                    TransactionType.Refund => -transaction.Amount,     // Money out
                    TransactionType.CashDrop => -transaction.Amount,   // Money out
                    TransactionType.PayOut => -transaction.Amount,     // Money out
                    _ => 0
                };

                // Update the wallet balance in the database
                await _walletRepository.UpdateBalanceAsync(wallet.Id, balanceChange);

                // Fire event if it's a sale transaction
                if (transaction.Type == TransactionType.Sale)
                {
                    SaleProcessed?.Invoke(this, transaction);
                }

                // Fire general transaction added event for all transaction types
                TransactionAdded?.Invoke(this, transaction);

                return true;
            }
            catch (Exception ex)
            {
                // Log the error (consider adding ILogger here)
                Console.WriteLine($"Error adding transaction: {ex.Message}");
                throw;
            }
        }

        public async Task<List<TransactionModel>> GetCurrentShiftTransactionsAsync()
        {
            var currentShift = await _shiftService.GetCurrentShiftAsync();
            if (currentShift == null)
                return new List<TransactionModel>();

            return await GetShiftTransactionsAsync(currentShift.Id);
        }

        public async Task<List<TransactionModel>> GetShiftTransactionsAsync(int shiftId)
        {
            var transactions = await _transactionRepository.GetByShiftIdAsync(shiftId);
            return transactions.Select(MapToModel).ToList();
        }

        public async Task<TransactionModel?> GetTransactionByIdAsync(int transactionId)
        {
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            return transaction != null ? MapToModel(transaction) : null;
        }

        public async Task<bool> UpdateTransactionAsync(TransactionModel transaction)
        {
            if (transaction == null || transaction.Id <= 0)
                return false;

            try
            {
                var existingTransaction = await _transactionRepository.GetByIdAsync(transaction.Id);
                if (existingTransaction == null)
                    return false;

                // Update properties
                existingTransaction.Description = transaction.Description;
                existingTransaction.Reference = transaction.Reference;

                await _transactionRepository.UpdateAsync(existingTransaction);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelTransactionAsync(int transactionId, string reason)
        {
            try
            {
                var transaction = await _transactionRepository.GetByIdAsync(transactionId);
                if (transaction == null)
                    return false;

                // Reverse the wallet balance if it's not a cash transaction
                var wallet = await _walletRepository.GetByIdAsync(transaction.WalletId);
                if (wallet != null && !wallet.Name.Equals("Cash Register", StringComparison.OrdinalIgnoreCase))
                {
                    // Reverse the balance change
                    var balanceChange = transaction.Type == TransactionType.Sale || transaction.Type == TransactionType.CashAdd
                        ? -transaction.Amount
                        : transaction.Amount;

                    await _walletRepository.UpdateBalanceAsync(wallet.Id, balanceChange);
                }

                // Delete or mark as cancelled (for now we'll delete)
                await _transactionRepository.DeleteAsync(transactionId);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Maps a transaction entity to a transaction model
        /// </summary>
        private TransactionModel MapToModel(TransactionEntity entity)
        {
            return new TransactionModel
            {
                Id = entity.Id,
                Type = entity.Type,
                Amount = entity.Amount,
                Description = entity.Description,
                Reference = entity.Reference,
                Timestamp = entity.Timestamp,
                Wallet = entity.Wallet != null ? new WalletModel
                {
                    Id = entity.Wallet.Id,
                    Name = entity.Wallet.Name,
                    Balance = entity.Wallet.Balance,
                    IsActive = entity.Wallet.IsActive
                } : null
            };
        }
    }
}
