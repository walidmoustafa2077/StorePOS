using StorePOS.Client.Wpf.Commands;
using StorePOS.Client.Wpf.Enums;
using StorePOS.Client.Wpf.MVVM.Models;
using StorePOS.Client.Wpf.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels
{
    /// <summary>
    /// ViewModel for the Sales/POS interface, managing cart operations and checkout process.
    /// Inherits product management functionality from BaseProductViewModel.
    /// </summary>
    public class CashierViewModel : BaseProductViewModel
    {
        // Override PageSize for sales view (4 columns * 5 rows = 20 is good)
        private new const int PageSize = 20;
        
        private CartItemModel? _selectedCartItem;
        private PaymentMethod _selectedPaymentMethod = PaymentMethod.Cash;
        private decimal _amountPaid = 0;
        private string _customerName = string.Empty;
        private string _notes = string.Empty;
        private bool _isProcessingSale = false;

        // Services
        private readonly IShiftService? _shiftService;
        private readonly ISaleService? _saleService;
        private readonly IDialogService _dialogService;
        private ShiftModel? _currentShift;

        // Sales Summary
        private SalesSummaryModel? _todaysSummary;
        
        // Cart Management
        public ObservableCollection<CartItemModel> CartItems { get; } = new();

        // Additional Commands for Sales
        public RelayCommand AddToCartCommand { get; private set; } = null!;
        public AsyncRelayCommand ClearCartCommand { get; private set; } = null!;
        public AsyncRelayCommand ProcessSaleCommand { get; private set; } = null!;
        public RelayCommand ApplyDiscountCommand { get; private set; } = null!;
        public RelayCommand IncreaseQuantityCommand { get; private set; } = null!;
        public RelayCommand DecreaseQuantityCommand { get; private set; } = null!;
        public RelayCommand RemoveFromCartCommand { get; private set; } = null!;

        // Properties
        public CartItemModel? SelectedCartItem
        {
            get => _selectedCartItem;
            set => SetProperty(ref _selectedCartItem, value);
        }

        public PaymentMethod SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set
            {
                if (SetProperty(ref _selectedPaymentMethod, value))
                {
                    OnPropertyChanged(nameof(FormattedChangeAmount));
                }
            }
        }

        public decimal AmountPaid
        {
            get => _amountPaid;
            set
            {
                var newValue = Math.Max(0, value);
                if (SetProperty(ref _amountPaid, newValue))
                {
                    OnPropertyChanged(nameof(ChangeAmount));
                    OnPropertyChanged(nameof(FormattedChangeAmount));
                    OnPropertyChanged(nameof(CanCompleteSale));
                    ProcessSaleCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public bool IsProcessingSale
        {
            get => _isProcessingSale;
            set => SetProperty(ref _isProcessingSale, value);
        }

        // Calculated Properties
        public int ItemCount => CartItems.Sum(item => item.Quantity);
        public decimal Subtotal => CartItems.Sum(item => item.LineTotal);
        public decimal TotalTax => CartItems.Sum(item => item.TaxAmount);
        public decimal TotalAmount => Subtotal + TotalTax;
        public decimal ChangeAmount 
        { 
            get 
            { 
                var change = Math.Max(0, AmountPaid - TotalAmount);
                return change;
            }
        }

        // Formatted Properties
        public string FormattedSubtotal => Subtotal.ToString("C");
        public string FormattedTotalTax => TotalTax.ToString("C");
        public string FormattedTotalAmount => TotalAmount.ToString("C");
        public string FormattedChangeAmount => ChangeAmount.ToString("C");

        // Business Logic Properties
        public bool HasItems => CartItems.Count > 0;
        public bool IsShiftActive => _currentShift?.IsActive == true;
        public bool CanCompleteSale => HasItems && AmountPaid >= TotalAmount && !IsProcessingSale && IsShiftActive;
        public string SaleCompletionStatus
        {
            get
            {
                if (!HasItems) return "Add items to cart";
                if (!IsShiftActive) return "Start a shift to process sales";
                if (AmountPaid < TotalAmount) return $"Payment required: {(TotalAmount):C}";
                if (IsProcessingSale) return "Processing sale...";
                return "Ready to complete sale";
            }
        }

        // Payment Methods
        public PaymentMethod[] PaymentMethods { get; } = Enum.GetValues<PaymentMethod>();

        // Sales Summary Properties
        public SalesSummaryModel? TodaysSummary
        {
            get => _todaysSummary;
            set
            {
                if (SetProperty(ref _todaysSummary, value))
                {
                    OnPropertyChanged(nameof(TodaysSummaryDisplay));
                }
            }
        }

        public string TodaysSummaryDisplay
        {
            get
            {
                if (TodaysSummary == null)
                    return "No sales today";

                return $"Today: {TodaysSummary.TotalSales} sales | {TodaysSummary.TotalRevenue:C} revenue | Avg: {TodaysSummary.AverageSaleAmount:C}";
            }
        }


        /// <summary>
        /// Initializes a new instance of SalesViewModel.
        /// </summary>
        public CashierViewModel(
            IDialogService dialogService,
            IProductFilterService filterService,
            IProductService dataService,
            IDebounceService debounceService,
            IShiftService? shiftService = null,
            ISaleService? saleService = null)
            : base(filterService, dataService, debounceService)
        {
            _shiftService = shiftService;
            _saleService = saleService;
            _dialogService = dialogService;
            Title = "Sales";

            InitializeSalesCommands();

            // Subscribe to cart changes to update calculated properties
            CartItems.CollectionChanged += (s, e) => NotifyCartPropertiesChanged();

            // Subscribe to shift changes to update shift status
            if (_shiftService != null)
            {
                _shiftService.ShiftChanged += OnShiftChanged;
            }

            // Load data asynchronously ensuring we're on UI thread
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                ShowActiveOnly = true; // Default to showing active products only
                await InitializeAsync();
            });
        }

        /// <summary>
        /// Handles the ShiftChanged event to automatically update shift status
        /// </summary>
        private void OnShiftChanged(object? sender, ShiftModel shift)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _currentShift = shift;
                OnPropertyChanged(nameof(IsShiftActive));
                OnPropertyChanged(nameof(CanCompleteSale));
                OnPropertyChanged(nameof(SaleCompletionStatus));
                ProcessSaleCommand?.RaiseCanExecuteChanged();
            });
        }


        /// <summary>
        /// Initializes sales-specific commands in addition to base commands.
        /// </summary>
        protected override void InitializeBaseCommands()
        {
            base.InitializeBaseCommands();
            InitializeSalesCommands();
        }

        private void InitializeSalesCommands()
        {
            AddToCartCommand = new RelayCommand(OnAddToCart);
            RemoveFromCartCommand = new RelayCommand(OnRemoveFromCart);
            ClearCartCommand = new AsyncRelayCommand(OnClearCart);
            ProcessSaleCommand = new AsyncRelayCommand(ProcessSaleAsync, () => CanCompleteSale);
            ApplyDiscountCommand = new RelayCommand(OnApplyDiscount);
            IncreaseQuantityCommand = new RelayCommand(OnIncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(OnDecreaseQuantity);
        }

        // Command Handlers
        private void OnAddToCart(object? parameter)
        {
            if (parameter is ProductModel product)
            {
                // Check if product is already in cart
                var existingItem = CartItems.FirstOrDefault(item => item.Product.Id == product.Id);
                
                if (existingItem != null)
                {
                    // Increase quantity of existing item
                    existingItem.Quantity++;
                }
                else
                {
                    // Add new item to cart
                    var cartItem = new CartItemModel
                    {
                        Product = product,
                        Quantity = 1
                    };
                    
                    CartItems.Add(cartItem);
                }
                
                NotifyCartPropertiesChanged();
            }
        }

        private void OnRemoveFromCart(object? parameter)
        {
            if (parameter is CartItemModel cartItem)
            {
                CartItems.Remove(cartItem);
                NotifyCartPropertiesChanged();
            }
        }

        private async Task OnClearCart(object? parameter)
        {
            var result = await _dialogService.ShowDialogAsync(new DialogModel
            {
                Title = "Clear Cart",
                Message = "Are you sure you want to clear the cart? This action cannot be undone.",
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "Cancel"
            });

            if (result == true)
            {
                CartItems.Clear();
                NotifyCartPropertiesChanged();
                ResetCheckoutForm();
            }
        }

        private async Task ProcessSaleAsync()
        {
            if (!CanCompleteSale)
                return;

            try
            {
                IsProcessingSale = true;

                if (_saleService == null)
                {
                    await _dialogService.ShowDialogAsync(new DialogModel
                    {
                        Title = "Error",
                        Message = "Sale service is not available.",
                        PrimaryButtonText = "OK"
                    });
                    return;
                }

                // Step 1: Validate cart before processing
                var validationResult = await _saleService.ValidateCartAsync(CartItems);
                if (!validationResult.IsSuccess)
                {
                    await _dialogService.ShowDialogAsync(new DialogModel
                    {
                        Title = "Validation Error",
                        Message = validationResult.ErrorMessage ?? "Cart validation failed",
                        PrimaryButtonText = "OK"
                    });
                    return;
                }

                // Step 2: Calculate accurate cart totals
                var cartTotals = await _saleService.CalculateCartTotalsAsync(CartItems);

                // Add confirmation dialog before processing
                var confirmResult = await _dialogService.ShowDialogAsync(new DialogModel
                {
                    Title = "Confirm Sale",
                    Message = $"Process sale of {cartTotals.GrandTotal:C}?\n\n" +
                             $"Subtotal: {cartTotals.Subtotal:C}\n" +
                             $"Tax: {cartTotals.TotalTax:C}\n" +
                             $"Discount: {cartTotals.TotalDiscount:C}\n" +
                             $"Total: {cartTotals.GrandTotal:C}\n\n" +
                             $"Change: {FormattedChangeAmount}",
                    PrimaryButtonText = "Process Sale",
                    SecondaryButtonText = "Cancel"
                });

                if (confirmResult != true)
                    return; // User cancelled

                // Step 3: Process the sale using the sale service
                var saleResult = await _saleService.ProcessCheckoutAsync(
                    CartItems, 
                    cartTotals.GrandTotal, 
                    SelectedPaymentMethod.ToString());

                if (saleResult.IsSuccess && saleResult.Data != null)
                {
                    var sale = saleResult.Data;
                    
                    // Show success message
                    var successMessage = $"✅ SALE COMPLETED SUCCESSFULLY\n" +
                                       $"═══════════════════════════════\n" +
                                       $"Sale #: {sale.SaleNumber}\n" +
                                       $"Sale ID: {sale.Id}\n" +
                                       $"Subtotal: {cartTotals.Subtotal:C}\n" +
                                       $"Tax: {cartTotals.TotalTax:C}\n" +
                                       $"Total Amount: {cartTotals.GrandTotal:C}\n" +
                                       $"Items Sold: {cartTotals.TotalItems}\n" +
                                       $"Payment Method: {SelectedPaymentMethod}\n" +
                                       $"Amount Paid: {AmountPaid:C}\n" +
                                       $"Change: {ChangeAmount:C}\n" +
                                       $"Time: {DateTime.Now:hh:mm tt}";

                    await _dialogService.ShowDialogAsync(new DialogModel
                    {
                        Title = "Sale Completed",
                        Message = successMessage,
                        PrimaryButtonText = "OK"
                    });

                    // Clear cart and reset form
                    CartItems.Clear();
                    ResetCheckoutForm();
                    NotifyCartPropertiesChanged();
                    
                    // Reload products to reflect updated stock quantities
                    await LoadDataAsync();
                }
                else
                {
                    await _dialogService.ShowDialogAsync(new DialogModel
                    {
                        Title = "Sale Error",
                        Message = saleResult.ErrorMessage ?? "Failed to process sale",
                        PrimaryButtonText = "OK"
                    });
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowDialogAsync(new DialogModel
                {
                    Title = "Sale Error",
                    Message = $"An error occurred while processing the sale:\n{ex.Message}",
                    PrimaryButtonText = "OK"
                });
            }
            finally
            {
                IsProcessingSale = false;
                ProcessSaleCommand.RaiseCanExecuteChanged();
            }
        }

        private void OnApplyDiscount(object? parameter)
        {
            if (parameter is CartItemModel cartItem)
            {
                // This could open a discount dialog or apply a percentage discount
                // For now, we'll apply a simple 10% discount
                var discountAmount = cartItem.UnitPrice * 0.10m;
                cartItem.Discount = discountAmount;
                NotifyCartPropertiesChanged();
            }
        }

        private void OnIncreaseQuantity(object? parameter)
        {
            if (parameter is CartItemModel cartItem)
            {
                cartItem.Quantity++;
                NotifyCartPropertiesChanged();
            }
        }

        private void OnDecreaseQuantity(object? parameter)
        {
            if (parameter is CartItemModel cartItem)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    NotifyCartPropertiesChanged();
                }
                else
                {
                    // Remove item if quantity would become 0
                    CartItems.Remove(cartItem);
                    NotifyCartPropertiesChanged();
                }
            }
        }

        // Helper Methods
        private void NotifyCartPropertiesChanged()
        {
            OnPropertyChanged(nameof(ItemCount));
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(TotalTax));
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(ChangeAmount));
            OnPropertyChanged(nameof(FormattedSubtotal));
            OnPropertyChanged(nameof(FormattedTotalTax));
            OnPropertyChanged(nameof(FormattedTotalAmount));
            OnPropertyChanged(nameof(FormattedChangeAmount));
            OnPropertyChanged(nameof(HasItems));
            OnPropertyChanged(nameof(IsShiftActive));
            OnPropertyChanged(nameof(CanCompleteSale));
            OnPropertyChanged(nameof(SaleCompletionStatus));
            ProcessSaleCommand?.RaiseCanExecuteChanged();
        }

        private void ResetCheckoutForm()
        {
            AmountPaid = 0;
            Notes = string.Empty;
            SelectedPaymentMethod = PaymentMethod.Cash;
        }

        /// <summary>
        /// Initialize the sales view with data.
        /// This is called when the view is first accessed to prevent UI blocking.
        /// </summary>
        public override async Task InitializeAsync()
        {
            try
            {
                await base.InitializeAsync();
                
                // Load the current active shift
                await LoadCurrentShiftAsync();
                
                // Set default amount paid when cart changes
                if (HasItems && AmountPaid == 0)
                {
                    AmountPaid = TotalAmount;
                }
                
            }
            catch (Exception ex)
            {
                await _dialogService.ShowDialogAsync(new DialogModel
                {
                    Title = "Error",
                    Message = $"Error loading sales data: {ex.Message}",
                    PrimaryButtonText = "OK"
                });
            }
        }

        /// <summary>
        /// Override ApplyFiltersAsync to add automatic barcode detection for POS functionality.
        /// If an exact barcode match is found, automatically add the product to cart and clear search.
        /// </summary>
        protected override void ApplyFiltersAsync()
        {
            // Check for exact barcode match before applying filters
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchTerm = SearchText.Trim();
                
                // Check if search term matches a barcode exactly
                var productByBarcode = AllProducts.FirstOrDefault(p => 
                    !string.IsNullOrWhiteSpace(p.Barcode) && 
                    p.Barcode.Equals(searchTerm, StringComparison.OrdinalIgnoreCase));
                
                if (productByBarcode != null)
                {
                    // Auto-add to cart
                    OnAddToCart(productByBarcode);
                    
                    // Clear search text to prepare for next scan
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        SearchText = string.Empty;
                    });
                    
                    // Don't continue with regular filtering since we found exact match
                    return;
                }
            }
            
            // If no exact barcode match, proceed with normal filtering
            base.ApplyFiltersAsync();
        }


        /// <summary>
        /// Loads the current active shift from the database.
        /// </summary>
        public async Task LoadCurrentShiftAsync()
        {
            try
            {
                if (_shiftService != null)
                {
                    _currentShift = await _shiftService.GetCurrentShiftAsync();
                    OnPropertyChanged(nameof(IsShiftActive));
                    OnPropertyChanged(nameof(CanCompleteSale));
                    OnPropertyChanged(nameof(SaleCompletionStatus));
                    ProcessSaleCommand?.RaiseCanExecuteChanged();
                }
            }
            catch
            {
                _currentShift = null;
            }
        }
    
    }
}