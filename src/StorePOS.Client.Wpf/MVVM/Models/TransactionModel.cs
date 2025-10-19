using StorePOS.Client.Wpf.Enums;
using StorePOS.Client.Wpf.Extensions;

namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Represents a transaction that affects the cash register balance.
    /// </summary>
    public class TransactionModel : ModelBase
    {
        private decimal _amount;
        private string _description = string.Empty;
        private string _reference = string.Empty;

        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public TransactionType Type { get; set; }
        
        /// <summary>
        /// The wallet/account that was affected by this transaction.
        /// If null, defaults to cash wallet for backward compatibility.
        /// </summary>
        public WalletModel? Wallet { get; set; }

        public decimal Amount
        {
            get => _amount;
            set
            {
                if (SetProperty(ref _amount, value))
                {
                    OnPropertyChanged(nameof(FormattedAmount));
                    OnPropertyChanged(nameof(BalanceChange));
                    OnPropertyChanged(nameof(FormattedBalanceChange));
                }
            }
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Reference
        {
            get => _reference;
            set => SetProperty(ref _reference, value);
        }

        // Calculated Properties
        public decimal BalanceChange
        {
            get
            {
                return Type switch
                {
                    TransactionType.Sale => Amount, // Positive - money in
                    TransactionType.Refund => -Amount, // Negative - money out
                    TransactionType.CashDrop => -Amount, // Negative - money removed
                    TransactionType.CashAdd => Amount, // Positive - money added
                    TransactionType.PayIn => Amount, // Positive - money in
                    TransactionType.PayOut => -Amount, // Negative - money out
                    _ => 0
                };
            }
        }

        // Formatted Properties
        public string FormattedAmount => Amount.ToString("C");
        public string FormattedBalanceChange => BalanceChange.ToString("C");
        public string FormattedTimestamp => Timestamp.ToString("MM/dd/yyyy hh:mm tt");
        public string TypeDisplayName => Type.GetDisplayName();
        public string TypeIcon => Type.GetIconKind(); // Icon for transaction type
        public string StatusColor => BalanceChangeColor; // Alias for XAML binding
        
        public string BalanceChangeColor
        {
            get
            {
                return BalanceChange switch
                {
                    > 0 => "#4CAF50", // Green for positive
                    < 0 => "#F44336", // Red for negative
                    _ => "#757575" // Gray for zero
                };
            }
        }

        // Additional properties expected by XAML
        public string CustomerName => "Walk-in Customer"; // Default customer name
        public string PaymentMethod => "Cash"; // Default to cash for now
        public string FormattedTime => Timestamp.ToString("HH:mm");
    }
}