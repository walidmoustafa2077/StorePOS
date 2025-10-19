using System.Collections.ObjectModel;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels.SettingsViewModels
{
    public class CashierSettingsViewModel : ViewModelBase
    {
        #region Private Fields

        private string? _selectedLayoutMode;
        private string? _selectedProductDisplayMode;
        private int _gridColumns;
        private bool _enableQuickPayment;
        private bool _showDiscountButton;
        private bool _enableBarcodeScanner;
        private bool _autoFocusSearch;
        private bool _allowSplitPayment;
        private bool _showCalculator;
        private bool _askCustomerName;
        private string? _defaultPaymentMethod;
        private bool _soundOnTransaction;
        private bool _autoPrintReceipt;
        private bool _confirmBeforeDelete;

        #endregion

        #region Properties

        // POS Layout Settings
        public ObservableCollection<string> LayoutModes { get; set; } = new();
        
        public string? SelectedLayoutMode
        {
            get => _selectedLayoutMode;
            set => SetProperty(ref _selectedLayoutMode, value);
        }

        public ObservableCollection<string> ProductDisplayModes { get; set; } = new();
        
        public string? SelectedProductDisplayMode
        {
            get => _selectedProductDisplayMode;
            set => SetProperty(ref _selectedProductDisplayMode, value);
        }

        public int GridColumns
        {
            get => _gridColumns;
            set => SetProperty(ref _gridColumns, value);
        }

        // Quick Actions
        public bool EnableQuickPayment
        {
            get => _enableQuickPayment;
            set => SetProperty(ref _enableQuickPayment, value);
        }

        public bool ShowDiscountButton
        {
            get => _showDiscountButton;
            set => SetProperty(ref _showDiscountButton, value);
        }

        public bool EnableBarcodeScanner
        {
            get => _enableBarcodeScanner;
            set => SetProperty(ref _enableBarcodeScanner, value);
        }

        public bool AutoFocusSearch
        {
            get => _autoFocusSearch;
            set => SetProperty(ref _autoFocusSearch, value);
        }

        // Payment Settings
        public bool AllowSplitPayment
        {
            get => _allowSplitPayment;
            set => SetProperty(ref _allowSplitPayment, value);
        }

        public bool ShowCalculator
        {
            get => _showCalculator;
            set => SetProperty(ref _showCalculator, value);
        }

        public bool AskCustomerName
        {
            get => _askCustomerName;
            set => SetProperty(ref _askCustomerName, value);
        }

        public ObservableCollection<string> PaymentMethods { get; set; } = new();
        
        public string? DefaultPaymentMethod
        {
            get => _defaultPaymentMethod;
            set => SetProperty(ref _defaultPaymentMethod, value);
        }

        // Behavior Settings
        public bool SoundOnTransaction
        {
            get => _soundOnTransaction;
            set => SetProperty(ref _soundOnTransaction, value);
        }

        public bool AutoPrintReceipt
        {
            get => _autoPrintReceipt;
            set => SetProperty(ref _autoPrintReceipt, value);
        }

        public bool ConfirmBeforeDelete
        {
            get => _confirmBeforeDelete;
            set => SetProperty(ref _confirmBeforeDelete, value);
        }

        #endregion

        #region Constructor

        public CashierSettingsViewModel()
        {
            InitializeCollections();
            LoadDefaultValues();
        }

        #endregion

        #region Private Methods

        private void InitializeCollections()
        {
            // Layout Modes
            LayoutModes = new ObservableCollection<string>
            {
                "Compact View",
                "Standard View",
                "Wide View",
                "Split Screen",
                "Tablet Mode"
            };

            // Product Display Modes
            ProductDisplayModes = new ObservableCollection<string>
            {
                "Grid with Images",
                "Grid with Icons",
                "List View",
                "Compact List",
                "Categories First"
            };

            // Payment Methods
            PaymentMethods = new ObservableCollection<string>
            {
                "Cash",
                "Credit Card",
                "Debit Card",
                "Mobile Payment",
                "Bank Transfer"
            };
        }

        private void LoadDefaultValues()
        {
            // POS Layout
            SelectedLayoutMode = "Standard View";
            SelectedProductDisplayMode = "Grid with Images";
            GridColumns = 4;

            // Quick Actions
            EnableQuickPayment = true;
            ShowDiscountButton = true;
            EnableBarcodeScanner = true;
            AutoFocusSearch = true;

            // Payment Settings
            AllowSplitPayment = false;
            ShowCalculator = true;
            AskCustomerName = false;
            DefaultPaymentMethod = "Cash";

            // Behavior Settings
            SoundOnTransaction = true;
            AutoPrintReceipt = false;
            ConfirmBeforeDelete = true;
        }

        #endregion
    }
}
