namespace StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel for the ShiftSummaryView custom dialog content.
    /// </summary>
    public class ShiftSummaryContentViewModel : ViewModelBase
    {
        private string _shiftNumber = string.Empty;
        private string _cashierName = string.Empty;
        private string _duration = string.Empty;
        private int _transactionCount;
        private string _formattedStartingBalance = string.Empty;
        private string _formattedCurrentBalance = string.Empty;
        private string _formattedBalanceChange = string.Empty;
        private bool _isPositiveChange;

        public string ShiftNumber
        {
            get => _shiftNumber;
            set => SetProperty(ref _shiftNumber, value);
        }

        public string CashierName
        {
            get => _cashierName;
            set => SetProperty(ref _cashierName, value);
        }

        public string Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        public int TransactionCount
        {
            get => _transactionCount;
            set => SetProperty(ref _transactionCount, value);
        }

        public string FormattedStartingBalance
        {
            get => _formattedStartingBalance;
            set => SetProperty(ref _formattedStartingBalance, value);
        }

        public string FormattedCurrentBalance
        {
            get => _formattedCurrentBalance;
            set => SetProperty(ref _formattedCurrentBalance, value);
        }

        public string FormattedBalanceChange
        {
            get => _formattedBalanceChange;
            set => SetProperty(ref _formattedBalanceChange, value);
        }

        public bool IsPositiveChange
        {
            get => _isPositiveChange;
            set => SetProperty(ref _isPositiveChange, value);
        }
    }
}
