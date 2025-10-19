using StorePOS.Client.Wpf.MVVM.ViewModels;

namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Represents an account balance for shift start
    /// </summary>
    public class ShiftAccountBalance : ModelBase
    {
        private decimal _startingBalance;
        public string WalletName { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal StartingBalance
        {
            get => _startingBalance;
            set
            {
                if (SetProperty(ref _startingBalance, Math.Max(0, value)))
                {
                    OnPropertyChanged(nameof(FormattedBalance));
                }
            }
        }

        public string FormattedBalance => StartingBalance.ToString("C");
    }
}