using StorePOS.Client.Wpf.Enums;
using StorePOS.Client.Wpf.Extensions;

namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Represents an individual cash flow item.
    /// </summary>
    public class CashFlowItemModel : ModelBase
    {
        private decimal _amount;
        private string _description = string.Empty;
        private string _category = string.Empty;

        public int Id { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public CashFlowType Type { get; set; }
        
        /// <summary>
        /// Indicates whether this item should count towards expenses calculation.
        /// Used to exclude non-cash wallet drops from expense totals.
        /// </summary>
        public bool CountsAsExpense { get; set; } = true;

        public decimal Amount
        {
            get => _amount;
            set
            {
                if (SetProperty(ref _amount, value))
                {
                    OnPropertyChanged(nameof(FormattedAmount));
                    OnPropertyChanged(nameof(AmountColor));
                }
            }
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        // Display Properties
        public string FormattedAmount => Amount.ToString("C");
        public string FormattedTime => Time.ToString("hh:mm tt");
        public string TypeDisplayName => Type.GetDisplayName();
        public string IconKind => Type.GetIconKind();
        public string Icon => IconKind; // Alias for XAML binding
        public string TypeColor => AmountColor; // Alias for XAML binding
        public DateTime Date => Time; // For date binding

        public string AmountColor
        {
            get
            {
                // Use type-based color if available, otherwise fall back to amount-based logic
                var typeColor = Type.GetTypeColor();
                if (typeColor != "#757575") // If not the default gray color
                    return typeColor;

                return Amount switch
                {
                    > 0 => "#4CAF50", // Green for cash in
                    < 0 => "#F44336", // Red for cash out
                    _ => "#757575" // Gray for zero
                };
            }
        }
    }
}