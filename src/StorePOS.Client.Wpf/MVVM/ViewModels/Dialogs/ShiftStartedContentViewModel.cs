namespace StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel for the ShiftStartedView custom dialog content.
    /// </summary>
    public class ShiftStartedContentViewModel : ViewModelBase
    {
        private string _cashierName = string.Empty;
        private string _shiftNumber = string.Empty;
        private string _formattedStartingBalance = string.Empty;
        private string _startedAt = string.Empty;

        public string CashierName
        {
            get => _cashierName;
            set => SetProperty(ref _cashierName, value);
        }

        public string ShiftNumber
        {
            get => _shiftNumber;
            set => SetProperty(ref _shiftNumber, value);
        }

        public string FormattedStartingBalance
        {
            get => _formattedStartingBalance;
            set => SetProperty(ref _formattedStartingBalance, value);
        }

        public string StartedAt
        {
            get => _startedAt;
            set => SetProperty(ref _startedAt, value);
        }
    }
}
