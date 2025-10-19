namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Result of a cash operation dialog
    /// </summary>
    public class CashOperationResult
    {
        public decimal Amount { get; set; }
        public WalletModel Wallet { get; set; } = null!;
        public string Reason { get; set; } = string.Empty;
        public string OperationType { get; set; } = string.Empty;
    }
}