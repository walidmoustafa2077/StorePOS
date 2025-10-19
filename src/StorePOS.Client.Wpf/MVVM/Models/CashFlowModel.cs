namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Represents cash flow summary information for dashboard display.
    /// </summary>
    public class CashFlowModel : ModelBase
    {
        public DateTime Date { get; set; } = DateTime.Today;
        public List<CashFlowItemModel> Items { get; set; } = new();

        // Calculated Properties
        public decimal TotalCashIn => Items.Where(i => i.Amount > 0).Sum(i => i.Amount);
        public decimal TotalCashOut => Items.Where(i => i.Amount < 0 && i.CountsAsExpense).Sum(i => Math.Abs(i.Amount));
        public decimal NetCashFlow => TotalCashIn - TotalCashOut;

        // Formatted Properties
        public string FormattedTotalCashIn => TotalCashIn.ToString("C");
        public string FormattedTotalCashOut => TotalCashOut.ToString("C");
        public string FormattedNetCashFlow => NetCashFlow.ToString("C");
        public string FormattedDate => Date.ToString("MM/dd/yyyy");

        public string NetCashFlowColor
        {
            get
            {
                return NetCashFlow switch
                {
                    > 0 => "#4CAF50", // Green for positive
                    < 0 => "#F44336", // Red for negative
                    _ => "#757575" // Gray for zero
                };
            }
        }

        public string NetCashFlowIcon
        {
            get
            {
                return NetCashFlow switch
                {
                    > 0 => "TrendingUp", // Up arrow for positive
                    < 0 => "TrendingDown", // Down arrow for negative
                    _ => "TrendingNeutral" // Neutral for zero
                };
            }
        }

        // Additional properties for dashboard display
        public decimal TotalIncome => TotalCashIn;
        public decimal TotalExpenses => TotalCashOut;
        public string FormattedTotalIncome => TotalIncome.ToString("C");
        public string FormattedTotalExpenses => TotalExpenses.ToString("C");

        /// <summary>
        /// Adds a cash flow item and updates calculated properties.
        /// </summary>
        public void AddItem(CashFlowItemModel item)
        {
            if (item == null) return;
            
            Items.Add(item);
            NotifyCalculatedPropertiesChanged();
        }

        /// <summary>
        /// Refreshes all calculated properties.
        /// </summary>
        public void RefreshCalculations()
        {
            NotifyCalculatedPropertiesChanged();
        }

        private void NotifyCalculatedPropertiesChanged()
        {
            OnPropertyChanged(nameof(TotalCashIn));
            OnPropertyChanged(nameof(TotalCashOut));
            OnPropertyChanged(nameof(NetCashFlow));
            OnPropertyChanged(nameof(FormattedTotalCashIn));
            OnPropertyChanged(nameof(FormattedTotalCashOut));
            OnPropertyChanged(nameof(FormattedNetCashFlow));
            OnPropertyChanged(nameof(NetCashFlowColor));
            OnPropertyChanged(nameof(NetCashFlowIcon));
            OnPropertyChanged(nameof(TotalIncome));
            OnPropertyChanged(nameof(TotalExpenses));
            OnPropertyChanged(nameof(FormattedTotalIncome));
            OnPropertyChanged(nameof(FormattedTotalExpenses));
        }
    }
}