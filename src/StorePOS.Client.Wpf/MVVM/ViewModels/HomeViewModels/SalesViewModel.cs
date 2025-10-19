using StorePOS.Client.Wpf.Commands;
using StorePOS.Client.Wpf.Extensions;
using StorePOS.Client.Wpf.MVVM.Models;
using StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs;
using StorePOS.Client.Wpf.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels
{
    /// <summary>
    /// ViewModel for managing sales data, viewing transactions, processing refunds, and printing receipts.
    /// </summary>
    public class SalesViewModel : ViewModelBase, INavigationAware, IRefreshable
    {
        private readonly ISaleService _saleService;
        private readonly IDialogService _dialogService;
        private readonly IDebounceService _debounceService;

        private string _searchText = string.Empty;
        private DateTime _startDate = DateTime.Today.AddDays(-30);
        private DateTime _endDate = DateTime.Now; // Use Now to include current time
        private SaleModel? _selectedSale;
        private ObservableCollection<SaleModel> _allSales = new();
        private ObservableCollection<ProductSalesModel> _topSellingProducts = new();
        private int _totalSalesCount;
        private decimal _totalRevenue;
        private decimal _averageSaleAmount;
        private int _totalItemsSold;
        private int _totalRefundsCount;
        private decimal _totalRefundAmount;
        private int _partialRefundsCount;
        private int _fullRefundsCount;

        // Constructor
        public SalesViewModel(
            ISaleService saleService,
            IDialogService dialogService,
            IDebounceService debounceService)
        {
            _saleService = saleService;
            _dialogService = dialogService;
            _debounceService = debounceService;

            Title = "Sales";

            // Initialize collections
            FilteredSales = new ObservableCollection<SaleModel>();
            TopSellingProducts = new ObservableCollection<ProductSalesModel>();

            // Initialize commands
            InitializeCommands();

            // Load data asynchronously
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await LoadSalesDataAsync();
                await LoadTopSellingProductsAsync();
            });
        }

        #region Properties

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    _debounceService.Debounce("SalesSearch", async () => await FilterSalesAsync(), TimeSpan.FromMilliseconds(500));
                }
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    // Validate that StartDate is not after EndDate
                    if (_startDate > _endDate)
                    {
                        // Adjust EndDate to match StartDate
                        EndDate = _startDate;
                        OnPropertyChanged(nameof(EndDate));
                    }
                    _ = LoadSalesDataAsync();
                    _ = LoadTopSellingProductsAsync();
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                // Validate that EndDate is not before StartDate
                if (value < _startDate)
                {
                    // Don't allow EndDate to be before StartDate
                    Application.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        await _dialogService.ShowErrorAsync(
                            "End date cannot be earlier than start date."
                            , "Invalid Date Range");
                    });

                    // Reset to StartDate
                    value = _startDate;
                }
                
                if (SetProperty(ref _endDate, value))
                {
                    _ = LoadSalesDataAsync();
                    _ = LoadTopSellingProductsAsync();
                }
            }
        }

        public SaleModel? SelectedSale
        {
            get => _selectedSale;
            set => SetProperty(ref _selectedSale, value);
        }

        public ObservableCollection<SaleModel> FilteredSales { get; private set; }

        public ObservableCollection<ProductSalesModel> TopSellingProducts
        {
            get => _topSellingProducts;
            private set => SetProperty(ref _topSellingProducts, value);
        }

        public int TotalSalesCount
        {
            get => _totalSalesCount;
            private set => SetProperty(ref _totalSalesCount, value);
        }

        public decimal TotalRevenue
        {
            get => _totalRevenue;
            private set => SetProperty(ref _totalRevenue, value);
        }

        public string FormattedTotalRevenue => TotalRevenue.ToString("C");

        public decimal AverageSaleAmount
        {
            get => _averageSaleAmount;
            private set => SetProperty(ref _averageSaleAmount, value);
        }

        public string FormattedAverageSaleAmount => AverageSaleAmount.ToString("C");

        public int TotalItemsSold
        {
            get => _totalItemsSold;
            private set => SetProperty(ref _totalItemsSold, value);
        }

        public int TotalRefundsCount
        {
            get => _totalRefundsCount;
            private set => SetProperty(ref _totalRefundsCount, value);
        }

        public decimal TotalRefundAmount
        {
            get => _totalRefundAmount;
            private set => SetProperty(ref _totalRefundAmount, value);
        }

        public string FormattedTotalRefundAmount => TotalRefundAmount.ToString("C");

        public int PartialRefundsCount
        {
            get => _partialRefundsCount;
            private set => SetProperty(ref _partialRefundsCount, value);
        }

        public int FullRefundsCount
        {
            get => _fullRefundsCount;
            private set => SetProperty(ref _fullRefundsCount, value);
        }

        public bool IsEmpty => FilteredSales.Count == 0;

        #endregion

        #region Commands

        public AsyncRelayCommand SearchCommand { get; private set; } = null!;
        public RelayCommand ClearSearchCommand { get; private set; } = null!;
        public AsyncRelayCommand<SaleModel> ViewSaleDetailsCommand { get; private set; } = null!;
        public AsyncRelayCommand<SaleModel> PrintReceiptCommand { get; private set; } = null!;
        public AsyncRelayCommand<SaleModel> ProcessRefundCommand { get; private set; } = null!;
        public AsyncRelayCommand ExportSalesCommand { get; private set; } = null!;
        public RelayCommand PrintSalesReportCommand { get; private set; } = null!;

        private void InitializeCommands()
        {
            SearchCommand = new AsyncRelayCommand(FilterSalesAsync);
            ClearSearchCommand = new RelayCommand(ExecuteClearSearch);
            ViewSaleDetailsCommand = new AsyncRelayCommand<SaleModel>(ExecuteViewSaleDetailsAsync);
            PrintReceiptCommand = new AsyncRelayCommand<SaleModel>(ExecutePrintReceiptAsync);
            ProcessRefundCommand = new AsyncRelayCommand<SaleModel>(ExecuteProcessRefundAsync);
            ExportSalesCommand = new AsyncRelayCommand(ExecuteExportSalesAsync);
            PrintSalesReportCommand = new RelayCommand(ExecutePrintSalesReport);
        }

        #endregion

        #region Methods

        private async Task LoadSalesDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Load sales within date range - ensure end date includes full day
                var endDate = EndDate.Date == DateTime.Today ? DateTime.Now : EndDate.Date.AddDays(1).AddSeconds(-1);
                var sales = await _saleService.GetSalesByDateRangeAsync(StartDate, endDate);
                _allSales = new ObservableCollection<SaleModel>(sales);

                // Get sales summary - use the adjusted endDate to ensure consistency
                var summary = await _saleService.GetSalesSummaryAsync(StartDate, endDate);
                TotalSalesCount = summary.TotalSales;
                TotalRevenue = summary.TotalRevenue;
                AverageSaleAmount = summary.AverageSaleAmount;
                TotalItemsSold = summary.ItemsSold;

                // Calculate refund statistics
                CalculateRefundStatistics();

                // Apply current filter
                await FilterSalesAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync(
                    "Load Error",
                    $"Failed to load sales data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Refreshes the sales data by reloading from the data source.
        /// Implements IRefreshable interface.
        /// </summary>
        public async Task RefreshAsync()
        {
            await LoadSalesDataAsync();
            await LoadTopSellingProductsAsync();
        }

        private void CalculateRefundStatistics()
        {
            // Calculate refund statistics from loaded sales
            var refundedSales = _allSales.Where(s => s.IsRefunded || s.IsPartiallyRefunded).ToList();
            
            TotalRefundsCount = refundedSales.Count;
            TotalRefundAmount = _allSales.Sum(s => s.TotalRefundAmount);
            FullRefundsCount = _allSales.Count(s => s.IsRefunded);
            PartialRefundsCount = _allSales.Count(s => s.IsPartiallyRefunded);
        }

        private async Task LoadTopSellingProductsAsync()
        {
            try
            {
                // Use the same date range as the sales list for consistency
                var endDate = EndDate.Date == DateTime.Today ? DateTime.Now : EndDate.Date.AddDays(1).AddSeconds(-1);
                var topProducts = await _saleService.GetBestSellingProductsAsync(
                    StartDate,
                    endDate,
                    10); // Top 10 products

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    TopSellingProducts.Clear();
                    int rank = 1;
                    foreach (var product in topProducts)
                    {
                        product.Rank = rank++;
                        TopSellingProducts.Add(product);
                    }
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync(
                    "Load Error",
                    $"Failed to load top selling products: {ex.Message}");
            }
        }

        private async Task FilterSalesAsync()
        {
            await Task.Run(async () =>
            {
                var filtered = _allSales.AsEnumerable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var searchLower = SearchText.ToLower();
                    filtered = filtered.Where(s =>
                        s.SaleNumber.ToLower().Contains(searchLower) ||
                        s.CustomerName.ToLower().Contains(searchLower) ||
                        s.CashierName.ToLower().Contains(searchLower) ||
                        s.Items.Any(item => item.ProductName.ToLower().Contains(searchLower)));
                }

                var filteredList = filtered.OrderByDescending(s => s.SaleDate).ToList();

                // Update UI on dispatcher thread
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    FilteredSales.Clear();
                    foreach (var sale in filteredList)
                    {
                        FilteredSales.Add(sale);
                    }
                    OnPropertyChanged(nameof(IsEmpty));
                });
            });
        }

        private void ExecuteClearSearch()
        {
            SearchText = string.Empty;
        }

        /// <summary>
        /// Refreshes a specific sale in the collections after it has been modified
        /// </summary>
        private async Task RefreshSaleAsync(int saleId)
        {
            try
            {
                // Get the updated sale from the database
                var updatedSale = await _saleService.GetSaleByIdAsync(saleId);
                if (updatedSale == null) return;

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // Update in _allSales collection
                    var existingInAll = _allSales.FirstOrDefault(s => s.Id == saleId);
                    if (existingInAll != null)
                    {
                        var index = _allSales.IndexOf(existingInAll);
                        _allSales[index] = updatedSale;
                    }

                    // Update in FilteredSales collection (the visible one)
                    var existingInFiltered = FilteredSales.FirstOrDefault(s => s.Id == saleId);
                    if (existingInFiltered != null)
                    {
                        var index = FilteredSales.IndexOf(existingInFiltered);
                        FilteredSales[index] = updatedSale;
                    }

                    // Update SelectedSale if it's the one we just refreshed
                    if (SelectedSale?.Id == saleId)
                    {
                        SelectedSale = updatedSale;
                    }
                });
            }
            catch (Exception ex)
            {
                // Log error but don't show to user as this is a background refresh
                System.Diagnostics.Debug.WriteLine($"Error refreshing sale {saleId}: {ex.Message}");
            }
        }

        private async Task ExecuteViewSaleDetailsAsync(SaleModel? sale)
        {
            if (sale == null) return;

            try
            {
                // Create the content ViewModel and populate with sale data
                var contentViewModel = new SaleDetailsContentViewModel
                {
                    SaleNumber = sale.SaleNumber,
                    SaleDate = sale.FormattedSaleDate,
                    Cashier = sale.CashierName,
                    Customer = sale.CustomerName,
                    PaymentMethod = sale.PaymentMethod.ToString(),
                    FormattedSubtotal = sale.FormattedSubtotal,
                    FormattedTax = sale.FormattedTotalTax,
                    FormattedTotal = sale.FormattedTotalAmount,
                    FormattedPaid = sale.FormattedAmountPaid,
                    FormattedChange = sale.FormattedChangeAmount,
                    Notes = sale.Notes ?? string.Empty
                };

                // Populate items collection
                foreach (var item in sale.Items)
                {
                    contentViewModel.Items.Add(new SaleItemViewModel
                    {
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        FormattedUnitPrice = item.FormattedUnitPrice,
                        FormattedLineTotal = item.FormattedLineTotal
                    });
                }

                // Create the view and set its DataContext
                var view = new Views.Dialogs.SaleDetailsView
                {
                    DataContext = contentViewModel
                };

                // Create dialog ViewModel with button command
                var dialogViewModel = new BaseDialogViewModel
                {
                    Title = "Sale Details",
                    Content = view,
                    PrimaryButtonText = "Close",
                    PrimaryButtonCommand = new RelayCommand(() => _dialogService.CloseDialog(true))
                };

                await _dialogService.ShowDialogAsync(dialogViewModel);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync(
                    "Error",
                    $"Failed to load sale details: {ex.Message}");
            }
        }

        private async Task ExecutePrintReceiptAsync(SaleModel? sale)
        {
            if (sale == null) return;

            try
            {
                IsBusy = true;

                // Generate receipt
                var receipt = await _saleService.GenerateReceiptAsync(sale.Id);

                // TODO: Implement actual printing functionality
                // Create custom content view model for the success dialog
                var contentViewModel = new ReceiptPrintedContentViewModel
                {
                    SaleNumber = sale.SaleNumber,
                    PrintTime = DateTime.Now.ToString("g")
                };

                // Create the custom view and set its DataContext
                var contentView = new Views.Dialogs.ReceiptPrintedView
                {
                    DataContext = contentViewModel
                };

                // Create a dialog with custom content
                var dialog = new BaseDialogViewModel
                {
                    Title = "Receipt Printed",
                    Content = contentView,
                    PrimaryButtonText = "OK",
                    PrimaryButtonCommand = new RelayCommand(() => _dialogService.CloseDialog(true))
                };

                await _dialogService.ShowDialogAsync(dialog);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync(
                    "Print Error",
                    $"Failed to print receipt: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecuteProcessRefundAsync(SaleModel? sale)
        {
            if (sale == null) return;

            try
            {
                // Check if sale is already fully refunded
                if (sale.IsRefunded)
                {
                    await _dialogService.ShowWarningAsync(
                        "Already Refunded",
                        $"Sale {sale.SaleNumber} has already been fully refunded.");
                    return;
                }

                IsBusy = true;

                // Get fresh sale data with items
                var fullSale = await _saleService.GetSaleByIdAsync(sale.Id);
                if (fullSale == null)
                {
                    await _dialogService.ShowErrorAsync("Error", "Sale not found.");
                    return;
                }

                // Create refund items from sale items
                var refundItems = fullSale.Items.Select(item => new RefundItemModel
                {
                    SaleItemId = item.Id, // Correct: Use the SaleItem ID, not Product ID
                    ProductName = item.ProductName,
                    OriginalQuantity = item.Quantity,
                    QuantityAlreadyRefunded = item.QuantityRefunded, // Use actual refunded quantity from database
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.LineTotal,
                    TaxAmount = item.TaxAmount,
                    IsSelected = false
                }).ToList();

                // Create and show refund dialog
                var refundDialog = new RefundDialogViewModel(_dialogService)
                {
                    SaleNumber = fullSale.SaleNumber,
                    SaleDate = fullSale.FormattedSaleDate,
                    TotalAmount = fullSale.FormattedTotalAmount
                };
                refundDialog.InitializeItems(refundItems);

                // Show the dialog
                var dialogResult = await _dialogService.ShowDialogAsync(refundDialog);

                // Check if user confirmed the refund
                if (dialogResult == true && refundDialog.IsConfirmed)
                {
                    var selectedItems = refundItems.Where(i => i.IsSelected && i.RefundQuantity > 0).ToList();
                    if (!selectedItems.Any())
                    {
                        return;
                    }

                    // Get current user (cashier name from sale or default)
                    var refundedBy = Environment.UserName; // You can replace this with actual logged-in user

                    // Process the refund
                    var result = await _saleService.ProcessRefundAsync(
                        fullSale.Id,
                        selectedItems,
                        refundDialog.RefundReason,
                        refundedBy);

                    if (result.IsSuccess)
                    {
                        // Create custom content view model for the success dialog
                        var refundContentViewModel = new RefundSuccessContentViewModel
                        {
                            SaleNumber = fullSale.SaleNumber,
                            FormattedRefundAmount = selectedItems.Sum(i => i.RefundAmount).ToString("C"),
                            ProcessedAt = DateTime.Now.ToString("g")
                        };

                        // Create the custom view and set its DataContext
                        var refundContentView = new Views.Dialogs.RefundSuccessView
                        {
                            DataContext = refundContentViewModel
                        };

                        // Create a dialog with custom content
                        var refundDialog2 = new BaseDialogViewModel
                        {
                            Title = "Refund Processed",
                            Content = refundContentView,
                            PrimaryButtonText = "Done",
                            PrimaryButtonCommand = new RelayCommand(() => _dialogService.CloseDialog(true))
                        };

                        await _dialogService.ShowDialogAsync(refundDialog2);

                        // Refresh only the refunded sale to update its status in the UI
                        await RefreshSaleAsync(fullSale.Id);

                        // Refresh analytics (summary stats and top selling products)
                        var summary = await _saleService.GetSalesSummaryAsync(StartDate, EndDate);
                        TotalSalesCount = summary.TotalSales;
                        TotalRevenue = summary.TotalRevenue;
                        AverageSaleAmount = summary.AverageSaleAmount;
                        TotalItemsSold = summary.ItemsSold;
                        
                        // Recalculate refund statistics
                        CalculateRefundStatistics();
                        
                        await LoadTopSellingProductsAsync();
                    }
                    else
                    {
                        await _dialogService.ShowErrorAsync(
                            "Refund Failed",
                            result.ErrorMessage ?? "Failed to process refund.");
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync(
                    "Error",
                    $"Failed to process refund: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecuteExportSalesAsync()
        {
            try
            {
                IsBusy = true;

                // TODO: Implement actual export functionality (CSV, Excel, PDF)
                await _dialogService.ShowSuccessAsync(
                    "Export Sales",
                    $"Sales data ({FilteredSales.Count} records) has been exported successfully.");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync(
                    "Export Error",
                    $"Failed to export sales: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ExecutePrintSalesReport()
        {
            try
            {
                // TODO: Implement print sales report functionality
                MessageBox.Show(
                    $"Printing sales report for {FilteredSales.Count} sales.\n" +
                    $"Date Range: {StartDate:d} to {EndDate:d}\n" +
                    $"Total Revenue: {FormattedTotalRevenue}",
                    "Print Sales Report",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error printing sales report: {ex.Message}",
                    "Print Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region INavigationAware Implementation

        /// <summary>
        /// Called when navigating to the Sales view - refreshes the sales data
        /// </summary>
        public async Task OnNavigatedToAsync()
        {
            await LoadSalesDataAsync();
        }

        /// <summary>
        /// Called when navigating away from the Sales view
        /// </summary>
        public Task OnNavigatedFromAsync()
        {
            // Nothing to do when leaving the view
            return Task.CompletedTask;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                FilteredSales.Clear();
                TopSellingProducts.Clear();
                _allSales.Clear();
            }
            base.Dispose(disposing);
        }
    }
}
