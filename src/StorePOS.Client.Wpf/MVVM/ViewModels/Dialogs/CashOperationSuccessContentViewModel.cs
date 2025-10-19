namespace StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel for the CashOperationSuccessView custom dialog content.
    /// </summary>
    public class CashOperationSuccessContentViewModel : ViewModelBase
    {
        private string _operationType = string.Empty;
        private string _formattedAmount = string.Empty;
        private string _walletName = string.Empty;
        private string _walletLabel = string.Empty;
        private string _reason = string.Empty;
        private string _formattedNewBalance = string.Empty;
        private string _time = string.Empty;

        public string OperationType
        {
            get => _operationType;
            set
            {
                if (SetProperty(ref _operationType, value))
                {
                    // Update wallet label based on operation type
                    WalletLabel = value == "Drop" ? "From" : "To";
                }
            }
        }

        public string FormattedAmount
        {
            get => _formattedAmount;
            set => SetProperty(ref _formattedAmount, value);
        }

        public string WalletName
        {
            get => _walletName;
            set => SetProperty(ref _walletName, value);
        }

        public string WalletLabel
        {
            get => _walletLabel;
            set => SetProperty(ref _walletLabel, value);
        }

        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        public string FormattedNewBalance
        {
            get => _formattedNewBalance;
            set => SetProperty(ref _formattedNewBalance, value);
        }

        public string Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }
    }
}
