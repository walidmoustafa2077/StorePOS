using StorePOS.Client.Wpf.MVVM.Models;
using StorePOS.Client.Wpf.Enums;

namespace StorePOS.Client.Wpf.Services
{
    /// <summary>
    /// Interface for handling transaction operations within shifts.
    /// Focuses specifically on transaction management and processing.
    /// </summary>
    public interface ITransactionService
    {
        /// <summary>
        /// Event fired when a sale transaction is successfully processed.
        /// </summary>
        event EventHandler<TransactionModel>? SaleProcessed;

        /// <summary>
        /// Event fired when any transaction is successfully added.
        /// </summary>
        event EventHandler<TransactionModel>? TransactionAdded;

        /// <summary>
        /// Adds a transaction to the current shift.
        /// </summary>
        /// <param name="transaction">Transaction to add</param>
        /// <returns>True if transaction was added successfully</returns>
        Task<bool> AddTransactionAsync(TransactionModel transaction);

        /// <summary>
        /// Gets transaction history for the current shift.
        /// </summary>
        /// <returns>List of transactions in current shift</returns>
        Task<List<TransactionModel>> GetCurrentShiftTransactionsAsync();

        /// <summary>
        /// Gets transaction history for a specific shift.
        /// </summary>
        /// <param name="shiftId">The ID of the shift to get transactions for</param>
        /// <returns>List of transactions for the specified shift</returns>
        Task<List<TransactionModel>> GetShiftTransactionsAsync(int shiftId);

        /// <summary>
        /// Gets transaction by its ID.
        /// </summary>
        /// <param name="transactionId">The transaction ID</param>
        /// <returns>The transaction if found, null otherwise</returns>
        Task<TransactionModel?> GetTransactionByIdAsync(int transactionId);

        /// <summary>
        /// Updates an existing transaction.
        /// </summary>
        /// <param name="transaction">The transaction to update</param>
        /// <returns>True if transaction was updated successfully</returns>
        Task<bool> UpdateTransactionAsync(TransactionModel transaction);

        /// <summary>
        /// Cancels a transaction by its ID.
        /// </summary>
        /// <param name="transactionId">The transaction ID to cancel</param>
        /// <param name="reason">Reason for cancellation</param>
        /// <returns>True if transaction was cancelled successfully</returns>
        Task<bool> CancelTransactionAsync(int transactionId, string reason);
    }
}