namespace StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel for the ReceiptPrintedView custom dialog content.
    /// </summary>
    public class ReceiptPrintedContentViewModel : ViewModelBase
    {
        private string _saleNumber = string.Empty;
        private string _printTime = string.Empty;

        public string SaleNumber
        {
            get => _saleNumber;
            set => SetProperty(ref _saleNumber, value);
        }

        public string PrintTime
        {
            get => _printTime;
            set => SetProperty(ref _printTime, value);
        }
    }
}
