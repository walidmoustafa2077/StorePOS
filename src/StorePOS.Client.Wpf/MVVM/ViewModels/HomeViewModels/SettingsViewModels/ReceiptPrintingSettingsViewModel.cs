using System.Collections.ObjectModel;
using StorePOS.Client.Wpf.Commands;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels.SettingsViewModels
{
    public class ReceiptPrintingSettingsViewModel : ViewModelBase
    {
        #region Private Fields

        private string _receiptHeader = "Thank you for shopping with us!";
        private string _receiptFooter = "Visit us again soon!";
        private string? _selectedPaperSize;
        private string? _selectedReceiptFontSize;
        private bool _showStoreLogo = true;
        private bool _showStoreAddress = true;
        private bool _showTaxBreakdown = true;
        private bool _showItemCodes = false;
        private bool _showBarcode = true;
        private bool _showQRCode = false;
        private string? _selectedPrinter;
        private int _selectedNumberOfCopies = 1;
        private bool _autoCutPaper = true;
        private bool _openCashDrawer = false;

        #endregion

        #region Properties

        // Receipt Format
        public string ReceiptHeader
        {
            get => _receiptHeader;
            set => SetProperty(ref _receiptHeader, value);
        }

        public string ReceiptFooter
        {
            get => _receiptFooter;
            set => SetProperty(ref _receiptFooter, value);
        }

        public ObservableCollection<string> PaperSizes { get; set; } = new();

        public string? SelectedPaperSize
        {
            get => _selectedPaperSize;
            set => SetProperty(ref _selectedPaperSize, value);
        }

        public ObservableCollection<string> ReceiptFontSizes { get; set; } = new();

        public string? SelectedReceiptFontSize
        {
            get => _selectedReceiptFontSize;
            set => SetProperty(ref _selectedReceiptFontSize, value);
        }

        // Receipt Content
        public bool ShowStoreLogo
        {
            get => _showStoreLogo;
            set => SetProperty(ref _showStoreLogo, value);
        }

        public bool ShowStoreAddress
        {
            get => _showStoreAddress;
            set => SetProperty(ref _showStoreAddress, value);
        }

        public bool ShowTaxBreakdown
        {
            get => _showTaxBreakdown;
            set => SetProperty(ref _showTaxBreakdown, value);
        }

        public bool ShowItemCodes
        {
            get => _showItemCodes;
            set => SetProperty(ref _showItemCodes, value);
        }

        public bool ShowBarcode
        {
            get => _showBarcode;
            set => SetProperty(ref _showBarcode, value);
        }

        public bool ShowQRCode
        {
            get => _showQRCode;
            set => SetProperty(ref _showQRCode, value);
        }

        // Printer Settings
        public ObservableCollection<string> AvailablePrinters { get; set; } = new();

        public string? SelectedPrinter
        {
            get => _selectedPrinter;
            set => SetProperty(ref _selectedPrinter, value);
        }

        public ObservableCollection<int> NumberOfCopies { get; set; } = new();

        public int SelectedNumberOfCopies
        {
            get => _selectedNumberOfCopies;
            set => SetProperty(ref _selectedNumberOfCopies, value);
        }

        public bool AutoCutPaper
        {
            get => _autoCutPaper;
            set => SetProperty(ref _autoCutPaper, value);
        }

        public bool OpenCashDrawer
        {
            get => _openCashDrawer;
            set => SetProperty(ref _openCashDrawer, value);
        }

        #endregion

        #region Commands

        public AsyncRelayCommand RefreshPrintersCommand { get; }
        public AsyncRelayCommand PreviewReceiptCommand { get; }
        public AsyncRelayCommand PrintTestReceiptCommand { get; }

        #endregion

        #region Constructor

        public ReceiptPrintingSettingsViewModel()
        {
            InitializeCollections();
            LoadDefaultValues();

            // Initialize commands
            RefreshPrintersCommand = new AsyncRelayCommand(RefreshPrintersAsync);
            PreviewReceiptCommand = new AsyncRelayCommand(PreviewReceiptAsync);
            PrintTestReceiptCommand = new AsyncRelayCommand(PrintTestReceiptAsync);
        }

        #endregion

        #region Private Methods

        private void InitializeCollections()
        {
            // Paper Sizes
            PaperSizes = new ObservableCollection<string>
            {
                "58mm (Thermal)",
                "80mm (Thermal)",
                "A4 (210mm x 297mm)",
                "Letter (8.5\" x 11\")",
                "Custom"
            };

            // Font Sizes
            ReceiptFontSizes = new ObservableCollection<string>
            {
                "Small (8pt)",
                "Normal (10pt)",
                "Medium (12pt)",
                "Large (14pt)"
            };

            // Available Printers (Mock data - should be populated from system)
            AvailablePrinters = new ObservableCollection<string>
            {
                "Epson TM-T88V Receipt Printer",
                "Star TSP100 Thermal Printer",
                "Microsoft Print to PDF",
                "No Printer Selected"
            };

            // Number of Copies
            NumberOfCopies = new ObservableCollection<int> { 1, 2, 3, 4, 5 };
        }

        private void LoadDefaultValues()
        {
            SelectedPaperSize = "80mm (Thermal)";
            SelectedReceiptFontSize = "Normal (10pt)";
            SelectedPrinter = "No Printer Selected";
            SelectedNumberOfCopies = 1;
        }

        private async Task RefreshPrintersAsync()
        {
            IsBusy = true;
            try
            {
                // TODO: Implement actual printer detection
                await Task.Delay(500); // Simulate refresh

                // For now, just keep mock data
                // In production, use System.Drawing.Printing.PrinterSettings.InstalledPrinters
            }
            catch (Exception)
            {
                // Handle error
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task PreviewReceiptAsync()
        {
            IsBusy = true;
            try
            {
                // TODO: Implement receipt preview dialog
                await Task.Delay(100);
            }
            catch (Exception)
            {
                // Handle error
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task PrintTestReceiptAsync()
        {
            IsBusy = true;
            try
            {
                // TODO: Implement test receipt printing
                await Task.Delay(500); // Simulate printing

                // Show success message
            }
            catch (Exception)
            {
                // Handle error
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
    }
}
