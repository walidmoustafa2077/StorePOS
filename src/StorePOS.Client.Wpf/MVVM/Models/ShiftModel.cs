using StorePOS.Client.Wpf.Enums;

namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Represents a work shift session with transaction tracking and cash management.
    /// </summary>
    public class ShiftModel : ModelBase
    {
        private decimal _startingBalance = 0m; // Default starting cash
        private decimal _currentBalance = 0m;
        private bool _isActive = true;
        private string _shiftUser = "Current User";
        private string _notes = string.Empty;

        public int Id { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime? EndTime { get; set; }
        public string ShiftNumber { get; set; } = GenerateShiftNumber();

        public decimal StartingBalance
        {
            get => _startingBalance;
            set
            {
                if (SetProperty(ref _startingBalance, value))
                {
                    if (!IsActive) return; // Don't update current balance if shift ended
                    CurrentBalance = value + TransactionsTotalChange;
                }
            }
        }

        public decimal CurrentBalance
        {
            get => _currentBalance;
            set => SetProperty(ref _currentBalance, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public string ShiftUser
        {
            get => _shiftUser;
            set => SetProperty(ref _shiftUser, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        // Navigation properties
        public List<TransactionModel> Transactions { get; set; } = new();

        // Calculated Properties
        public TimeSpan Duration => (EndTime ?? DateTime.Now) - StartTime;
        public string FormattedDuration
        {
            get
            {
                var duration = Duration;
                if (duration.TotalDays >= 1)
                    return $"{(int)duration.TotalDays}d {duration.Hours}h {duration.Minutes}m";
                else if (duration.TotalHours >= 1)
                    return $"{duration.Hours}h {duration.Minutes}m";
                else
                    return $"{duration.Minutes}m";
            }
        }

        public int TransactionCount => Transactions?.Count ?? 0;
        public decimal TransactionsTotalChange => Transactions?.Sum(t => t.BalanceChange) ?? 0;
        public decimal BalanceChange => CurrentBalance - StartingBalance;
        
        /// <summary>
        /// Gets the total revenue from sales transactions in this shift.
        /// </summary>
        public decimal TotalRevenue => Transactions?.Where(t => t.Type == TransactionType.Sale).Sum(t => t.Amount) ?? 0;

        // Formatted Properties
        public string FormattedStartingBalance => StartingBalance.ToString("C");
        public string FormattedCurrentBalance => CurrentBalance.ToString("C");
        public string FormattedBalanceChange => BalanceChange.ToString("C");
        public string FormattedTotalRevenue => TotalRevenue.ToString("C");
        public string FormattedStartTime => StartTime.ToString("MM/dd/yyyy hh:mm tt");
        public string FormattedEndTime => EndTime?.ToString("MM/dd/yyyy hh:mm tt") ?? "Active";

        // Aliases for XAML binding compatibility
        public string CashierName => ShiftUser;
        public string ShiftDuration => FormattedDuration;

        /// <summary>
        /// Adds a transaction to the shift and updates the current balance.
        /// </summary>
        public void AddTransaction(TransactionModel transaction)
        {
            if (transaction == null) return;
            
            Transactions.Add(transaction); 
            CurrentBalance += transaction.BalanceChange;
            
            // Notify property changes
            OnPropertyChanged(nameof(TransactionCount));
            OnPropertyChanged(nameof(TransactionsTotalChange));
            OnPropertyChanged(nameof(BalanceChange));
            OnPropertyChanged(nameof(TotalRevenue));
            OnPropertyChanged(nameof(FormattedCurrentBalance));
            OnPropertyChanged(nameof(FormattedBalanceChange));
            OnPropertyChanged(nameof(FormattedTotalRevenue));
        }

        /// <summary>
        /// Ends the current shift.
        /// </summary>
        public void EndShift()
        {
            EndTime = DateTime.Now;
            IsActive = false;
            OnPropertyChanged(nameof(Duration));
            OnPropertyChanged(nameof(FormattedDuration));
            OnPropertyChanged(nameof(FormattedEndTime));
        }

        private static string GenerateShiftNumber()
        {
            return $"SH{DateTime.Now:yyyyMMdd}{DateTime.Now:HHmm}";
        }
    }
}