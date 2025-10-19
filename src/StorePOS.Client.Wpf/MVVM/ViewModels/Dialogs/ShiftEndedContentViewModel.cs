namespace StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel for the ShiftEndedView custom dialog content.
    /// </summary>
    public class ShiftEndedContentViewModel : ViewModelBase
    {
        private string _shiftNumber = string.Empty;
        private string _duration = string.Empty;
        private int _transactionCount;
        private string _formattedFinalBalance = string.Empty;

        public string ShiftNumber
        {
            get => _shiftNumber;
            set => SetProperty(ref _shiftNumber, value);
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

        public string FormattedFinalBalance
        {
            get => _formattedFinalBalance;
            set => SetProperty(ref _formattedFinalBalance, value);
        }
    }
}
