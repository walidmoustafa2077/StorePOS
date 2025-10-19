using System.Collections.ObjectModel;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel for the SaleDetailsView custom dialog content.
    /// </summary>
    public class SaleDetailsContentViewModel : ViewModelBase
    {
        private string _saleNumber = string.Empty;
        private string _saleDate = string.Empty;
        private string _cashier = string.Empty;
        private string _customer = string.Empty;
        private string _paymentMethod = string.Empty;
        private string _formattedSubtotal = string.Empty;
        private string _formattedTax = string.Empty;
        private string _formattedTotal = string.Empty;
        private string _formattedPaid = string.Empty;
        private string _formattedChange = string.Empty;
        private string _notes = string.Empty;

        public string SaleNumber
        {
            get => _saleNumber;
            set => SetProperty(ref _saleNumber, value);
        }

        public string SaleDate
        {
            get => _saleDate;
            set => SetProperty(ref _saleDate, value);
        }

        public string Cashier
        {
            get => _cashier;
            set => SetProperty(ref _cashier, value);
        }

        public string Customer
        {
            get => _customer;
            set => SetProperty(ref _customer, value);
        }

        public string PaymentMethod
        {
            get => _paymentMethod;
            set => SetProperty(ref _paymentMethod, value);
        }

        public ObservableCollection<SaleItemViewModel> Items { get; } = new();

        public string FormattedSubtotal
        {
            get => _formattedSubtotal;
            set => SetProperty(ref _formattedSubtotal, value);
        }

        public string FormattedTax
        {
            get => _formattedTax;
            set => SetProperty(ref _formattedTax, value);
        }

        public string FormattedTotal
        {
            get => _formattedTotal;
            set => SetProperty(ref _formattedTotal, value);
        }

        public string FormattedPaid
        {
            get => _formattedPaid;
            set => SetProperty(ref _formattedPaid, value);
        }

        public string FormattedChange
        {
            get => _formattedChange;
            set => SetProperty(ref _formattedChange, value);
        }

        public string Notes
        {
            get => _notes; 
            set => SetProperty(ref _notes, value);
        }
    }

    /// <summary>
    /// Represents a sale item for display in the sale details dialog.
    /// </summary>
    public class SaleItemViewModel : ViewModelBase
    {
        private string _productName = string.Empty;
        private int _quantity;
        private string _formattedUnitPrice = string.Empty;
        private string _formattedLineTotal = string.Empty;

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        public string FormattedUnitPrice
        {
            get => _formattedUnitPrice;
            set => SetProperty(ref _formattedUnitPrice, value);
        }

        public string FormattedLineTotal
        {
            get => _formattedLineTotal;
            set => SetProperty(ref _formattedLineTotal, value);
        }
    }
}
