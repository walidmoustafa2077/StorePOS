namespace StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel for the RefundSuccessView custom dialog content.
    /// </summary>
    public class RefundSuccessContentViewModel : ViewModelBase
    {
        private string _saleNumber = string.Empty;
        private string _formattedRefundAmount = string.Empty;
        private string _processedAt = string.Empty;

        public string SaleNumber
        {
            get => _saleNumber;
            set => SetProperty(ref _saleNumber, value);
        }

        public string FormattedRefundAmount
        {
            get => _formattedRefundAmount;
            set => SetProperty(ref _formattedRefundAmount, value);
        }

        public string ProcessedAt
        {
            get => _processedAt;
            set => SetProperty(ref _processedAt, value);
        }
    }
}
