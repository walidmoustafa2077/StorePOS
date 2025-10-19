using StorePOS.Client.Wpf.Commands;
using StorePOS.Client.Wpf.Enums;
using StorePOS.Client.Wpf.Extensions;
using StorePOS.Client.Wpf.MVVM.Models;
using StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs;
using StorePOS.Client.Wpf.Services;
using System.Collections.ObjectModel;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels
{
    /// <summary>
    /// ViewModel for the Dashboard view, managing shift operations, cash flow, and business overview.
    /// Provides comprehensive shift management including transaction tracking and financial summaries.
    /// </summary>
    public class DashboardViewModel : ViewModelBase, IRefreshable
    {
        private ShiftModel? _currentShift;
        private decimal _totalBalance = 500m; // Demo starting balance
        private decimal _todayRevenue = 0m;

        // Services
        private readonly IShiftService _shiftService;
        private readonly IDialogService _dialogService;
        private readonly IWalletService _walletService;
        private readonly ITransactionService _transactionService;

        // Collections
        public ObservableCollection<WalletModel> Wallets { get; } = new();
        public ObservableCollection<TransactionModel> RecentTransactions { get; } = new();
        public ObservableCollection<CashFlowItemModel> CashFlowItems { get; } = new();

        // Commands
        public AsyncRelayCommand StartShiftCommand { get; private set; } = null!;
        public AsyncRelayCommand EndShiftCommand { get; private set; } = null!;
        public AsyncRelayCommand CashDropCommand { get; private set; } = null!;
        public AsyncRelayCommand CashAddCommand { get; private set; } = null!;
        public AsyncRelayCommand ViewTransactionDetailsCommand { get; private set; } = null!;

        // Properties
        public ShiftModel? CurrentShift
        {
            get => _currentShift;
            set
            {
                if (SetProperty(ref _currentShift, value))
                {
                    OnPropertyChanged(nameof(IsShiftActive));
                    StartShiftCommand.RaiseCanExecuteChanged();
                    EndShiftCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public decimal TotalBalance
        {
            get => _totalBalance;
            set
            {
                if (SetProperty(ref _totalBalance, value))
                {
                    OnPropertyChanged(nameof(FormattedTotalBalance));
                }
            }
        }

        public decimal TodayRevenue
        {
            get => _todayRevenue;
            set
            {
                if (SetProperty(ref _todayRevenue, value))
                {
                    OnPropertyChanged(nameof(FormattedTodayRevenue));
                }
            }
        }

        // Calculated Properties
        public bool IsShiftActive => CurrentShift?.IsActive ?? false;

        public CashFlowModel CashFlowSummary { get; } = new();

        // Formatted Properties
        public string FormattedTotalBalance => TotalBalance.ToString("C");
        public string FormattedTodayRevenue => TodayRevenue.ToString("C");
        public string FormattedCurrentTillBalance 
        {
            get
            {
                var cashWallet = Wallets?.FirstOrDefault(w => w.Name == "Cash Register");
                return cashWallet?.Balance.ToString("C") ?? "$0.00";
            }
        }

        // Additional calculated properties for XAML bindings
        public int ActiveWalletsCount => Wallets?.Count(w => w.IsActive) ?? 0;
        public int TodayTransactionsCount => RecentTransactions?.Count(t => t.Timestamp.Date == DateTime.Today && t.Type == TransactionType.Sale) ?? 0;

        /// <summary>
        /// Initializes a new instance of DashboardViewModel.
        /// </summary>
        public DashboardViewModel(
            IDialogService dialogService, 
            IWalletService walletService, 
            IShiftService shiftService,
            ITransactionService transactionService)
        {
            _dialogService = dialogService;
            _walletService = walletService;
            _shiftService = shiftService;
            _transactionService = transactionService;
            Title = "Dashboard";
            
            InitializeCommands();

            // Subscribe to sale processed events to auto-update dashboard
            _transactionService.SaleProcessed += OnSaleProcessed;
            
            // Subscribe to all transaction events (including refunds, cash operations, etc.)
            _transactionService.TransactionAdded += OnTransactionAdded;

            // Initialize the dashboard data asynchronously
            _ = InitializeAsync();
        }

        private void InitializeCommands()
        {
            StartShiftCommand = new AsyncRelayCommand(StartShiftAsync, CanStartShift);
            EndShiftCommand = new AsyncRelayCommand(EndShiftAsync, CanEndShift);
            CashDropCommand = new AsyncRelayCommand(OnCashDropAsync);
            CashAddCommand = new AsyncRelayCommand(OnCashAddAsync);
            ViewTransactionDetailsCommand = new AsyncRelayCommand(OnViewTransactionDetailsAsync);
        }


        /// <summary>
        /// Initialize the dashboard with data.
        /// </summary>
        public async Task InitializeAsync()
        {
            // Check for an active shift and load it if exists
            await LoadActiveShiftAsync();

            // Refresh cash flow summary
            RefreshCashFlowSummary();
        }

        /// <summary>
        /// Refreshes the dashboard data by reloading from the data source.
        /// Implements IRefreshable interface.
        /// </summary>
        public async Task RefreshAsync()
        {
            await InitializeAsync();
        }

        /// <summary>
        /// Loads the active shift from the database if one exists.
        /// Also loads associated transactions and sales for the active shift.
        /// Transactions include wallet information for proper balance calculations.
        /// </summary>
        private async Task LoadActiveShiftAsync()
        {
            try
            {
                // Load active wallets from database
                var wallets = await _walletService.GetActiveWalletsAsync();
               
                Wallets.Clear();
                foreach (var wallet in wallets)
                {
                    Wallets.Add(wallet);
                }

                // Load the current active shift from the database
                var activeShift = await _shiftService.GetCurrentShiftAsync();
                
                if (activeShift != null)
                {
                    // Load transactions for the active shift (with wallet information)
                    var transactions = await _transactionService.GetShiftTransactionsAsync(activeShift.Id);
                    
                    // Populate the shift's transactions list
                    activeShift.Transactions.Clear();
                    
                    // Set the current shift (after loading transactions)
                    CurrentShift = activeShift;

                    // Update the UI transactions list
                    RecentTransactions.Clear();
                    foreach (var transaction in transactions)
                    {
                        RecentTransactions.Add(transaction);
                        CurrentShift.AddTransaction(transaction);
                    }
                }
                else
                {
                    // No active shift found in database
                    CurrentShift = null;
                    RecentTransactions.Clear();
                }

                // Calculate today's revenue from transactions
                TodayRevenue = RecentTransactions
                    .Where(t => t.Type == TransactionType.Sale && t.Timestamp.Date == DateTime.Today)
                    .Sum(t => t.Amount);

                // Update total balance (sum of all wallet balances from DB + calculated cash balance)
                TotalBalance = Wallets.Sum(w => w.Balance);

                // Notify all calculated properties after loading data
                OnPropertyChanged(nameof(ActiveWalletsCount));
                OnPropertyChanged(nameof(TodayTransactionsCount));
                OnPropertyChanged(nameof(FormattedCurrentTillBalance));
            }
            catch
            {
                // Log the error but don't block the initialization
                CurrentShift = null;

                RecentTransactions.Clear();
            }
        }


        /// <summary>
        /// Handles the SaleProcessed event to automatically update the dashboard
        /// </summary>
        private void OnSaleProcessed(object? sender, TransactionModel transaction)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                // Add transaction to recent transactions at the top
                RecentTransactions.Insert(0, transaction);

                // Update the current shift's transaction list
                if (CurrentShift != null)
                {
                    CurrentShift.AddTransaction(transaction);
                }

                // Update today's revenue
                if (transaction.Timestamp.Date == DateTime.Today)
                {
                    TodayRevenue += transaction.Amount;
                }

                // Update wallet balance in UI
                var wallet = Wallets?.FirstOrDefault(w => w.Name == "Cash Register");
                if (wallet != null)
                {
                    wallet.Balance += transaction.BalanceChange;
                }

                // Update total balance
                TotalBalance = Wallets!.Sum(w => w.Balance);

                // Notify property changes
                OnPropertyChanged(nameof(TodayTransactionsCount));
                OnPropertyChanged(nameof(FormattedCurrentTillBalance));

                // Refresh cash flow summary
                RefreshCashFlowSummary();
            });
        }

        /// <summary>
        /// Handles the TransactionAdded event to automatically update the dashboard for all transaction types
        /// </summary>
        private void OnTransactionAdded(object? sender, TransactionModel transaction)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                // Add transaction to recent transactions at the top if not already added
                if (!RecentTransactions.Any(t => t.Id == transaction.Id))
                {
                    RecentTransactions.Insert(0, transaction);
                }

                // Update the current shift's transaction list
                if (CurrentShift != null && !CurrentShift.Transactions.Any(t => t.Id == transaction.Id))
                {
                    CurrentShift.AddTransaction(transaction);
                }

                // Update today's revenue based on transaction type
                if (transaction.Timestamp.Date == DateTime.Today)
                {
                    // For sales, add to revenue; for refunds, subtract from revenue
                    if (transaction.Type == TransactionType.Sale)
                    {
                        TodayRevenue += transaction.Amount;
                    }
                    else if (transaction.Type == TransactionType.Refund)
                    {
                        TodayRevenue -= transaction.Amount;
                    }
                }

                // Update wallet balance in UI based on the transaction's wallet
                WalletModel? targetWallet = null;
                if (transaction.Wallet?.Id > 0)
                {
                    targetWallet = Wallets?.FirstOrDefault(w => w.Id == transaction.Wallet.Id);
                }
                else if (!string.IsNullOrWhiteSpace(transaction.Wallet?.Name))
                {
                    targetWallet = Wallets?.FirstOrDefault(w => w.Name == transaction.Wallet.Name);
                }

                if (targetWallet != null)
                {
                    targetWallet.Balance += transaction.BalanceChange;
                }

                // Update total balance
                TotalBalance = Wallets!.Sum(w => w.Balance);

                // Notify property changes
                OnPropertyChanged(nameof(TodayTransactionsCount));
                OnPropertyChanged(nameof(FormattedCurrentTillBalance));

                // Refresh cash flow summary
                RefreshCashFlowSummary();
            });
        }

        // Command Handlers
        private bool CanStartShift() => CurrentShift?.IsActive != true;

        private async Task StartShiftAsync()
        {
            try
            {
                // Create and show the start shift dialog
                var startShiftDialog = new StartShiftDialogViewModel(_dialogService, _walletService);
                var dialogResult = await _dialogService.ShowDialogAsync(startShiftDialog);

                if (dialogResult == true && startShiftDialog.Result != null)
                {
                    var result = startShiftDialog.Result;
                    
                    // Get the cash register starting balance
                    var cashBalance = result.AccountBalances
                        .FirstOrDefault(ab => ab.WalletName == "Cash Register")?.StartingBalance ?? 0m;

                    // Start the shift using the shift service
                    var startResult = await _shiftService.StartShiftAsync(cashBalance, result.CashierName);

                    if (startResult.IsSuccess && startResult.Data != null)
                    {
                        // Set the current shift
                        CurrentShift = startResult.Data;

                        // Update wallet balances based on the account balances from the dialog
                        foreach (var accountBalance in result.AccountBalances)
                        {
                            var wallet = Wallets.FirstOrDefault(w => w.Name == accountBalance.WalletName);
                            if (wallet != null)
                            {
                                // Update balance in memory (UI)
                                wallet.Balance = accountBalance.StartingBalance;
                                
                                // Update balance in database
                                await _walletService.UpdateWalletAsync(wallet);
                            }
                        }

                        // Update total balance
                        TotalBalance = Wallets.Sum(w => w.Balance);

                        // Create custom content view model for the success dialog
                        var contentViewModel = new ShiftStartedContentViewModel
                        {
                            CashierName = result.CashierName,
                            ShiftNumber = CurrentShift.ShiftNumber,
                            FormattedStartingBalance = result.TotalStartingBalance.ToString("C"),
                            StartedAt = CurrentShift.FormattedStartTime
                        };

                        // Create the custom view and set its DataContext
                        var contentView = new Views.Dialogs.ShiftStartedView
                        {
                            DataContext = contentViewModel
                        };

                        // Create a dialog with custom content
                        var dialog = new BaseDialogViewModel
                        {
                            Title = "Shift Started",
                            Content = contentView,
                            PrimaryButtonText = "Continue",
                            PrimaryButtonCommand = new RelayCommand(() => _dialogService.CloseDialog(true))
                        };

                        await _dialogService.ShowDialogAsync(dialog);

                        // Refresh dashboard to ensure all data is synced
                        await InitializeAsync();
                    }
                    else
                    {
                        await _dialogService.ShowErrorAsync(
                            title: "Error starting shift",
                            message: startResult.ErrorMessage ?? "Failed to start shift");
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync(title: "Error starting shift", message: ex.Message);
            }
        }

        private bool CanEndShift() => CurrentShift?.IsActive == true;

        private async Task EndShiftAsync()
        {
            if (CurrentShift == null) return;

            // Create custom content view model for the confirmation dialog
            var contentViewModel = new ShiftSummaryContentViewModel
            {
                ShiftNumber = CurrentShift.ShiftNumber,
                CashierName = CurrentShift.CashierName,
                Duration = CurrentShift.FormattedDuration,
                TransactionCount = CurrentShift.TransactionCount,
                FormattedStartingBalance = CurrentShift.FormattedStartingBalance,
                FormattedCurrentBalance = CurrentShift.FormattedCurrentBalance,
                FormattedBalanceChange = CurrentShift.FormattedBalanceChange,
                IsPositiveChange = CurrentShift.BalanceChange >= 0
            };

            // Create the custom view and set its DataContext
            var contentView = new Views.Dialogs.ShiftSummaryView
            {
                DataContext = contentViewModel
            };

            // Create a dialog with custom content
            var dialog = new BaseDialogViewModel
            {
                Title = "End Shift",
                Content = contentView,
                PrimaryButtonText = "End Shift",
                SecondaryButtonText = "Cancel",
                PrimaryButtonCommand = new RelayCommand(() => _dialogService.CloseDialog(true)),
                SecondaryButtonCommand = new RelayCommand(() => _dialogService.CloseDialog(false))
            };

            var dialogResult = await _dialogService.ShowDialogAsync(dialog);


            if (dialogResult == true)
            {
                try
                {
                    var finalBalance = CurrentShift.CurrentBalance;
                    var finalShiftNumber = CurrentShift.ShiftNumber;
                    var finalDuration = CurrentShift.FormattedDuration;
                    var finalTransactionCount = CurrentShift.TransactionCount;

                    // Use the shift service to end the shift
                    var endShiftResult = await _shiftService.EndShiftAsync(finalBalance, "Shift ended from dashboard");
                    
                    if (!endShiftResult.IsSuccess)
                    {
                        await _dialogService.ShowErrorAsync(
                            title: "Error ending shift", 
                            message: endShiftResult.ErrorMessage ?? "Failed to end shift");
                        return;
                    }

                    // Clear till balance when shift ends
                    var cashWallet = Wallets.FirstOrDefault(w => w.Name == "Cash Register");
                    if (cashWallet != null)
                    {
                        cashWallet.Balance = 0m;

                        await _walletService.UpdateWalletAsync(cashWallet);
                        
                        // Notify till balance change
                        OnPropertyChanged(nameof(FormattedCurrentTillBalance));
                    }

                    // Clear current shift
                    CurrentShift = null;

                    // Create custom content view model for the success dialog
                    var endedContentViewModel = new ShiftEndedContentViewModel
                    {
                        ShiftNumber = finalShiftNumber,
                        Duration = finalDuration,
                        TransactionCount = finalTransactionCount,
                        FormattedFinalBalance = finalBalance.ToString("C")
                    };

                    // Create the custom view and set its DataContext
                    var endedContentView = new Views.Dialogs.ShiftEndedView
                    {
                        DataContext = endedContentViewModel
                    };

                    // Create a dialog with custom content
                    var endedDialog = new BaseDialogViewModel
                    {
                        Title = "Shift Ended",
                        Content = endedContentView,
                        PrimaryButtonText = "Close",
                        PrimaryButtonCommand = new RelayCommand(() => _dialogService.CloseDialog(true))
                    };

                    await _dialogService.ShowDialogAsync(endedDialog);
                    OnPropertyChanged(nameof(Wallets));

                    await InitializeAsync();
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync(title: "Error ending shift", message: ex.Message);
                }
            }
        }

        private async Task OnCashDropAsync(object? parameter)
        {
            if (!IsShiftActive)
            {
                await _dialogService.ShowErrorAsync(title: "No Active Shift", message: "Please start a shift before performing cash operations.");
                return;
            }

            try
            {
                // Get current cash balance
                var cashWallet = Wallets.FirstOrDefault(w => w.Name == "Cash Register");
                if (cashWallet == null)
                {
                    await _dialogService.ShowErrorAsync(title: "Error", message: "Cash register wallet not found.");
                    return;
                }

                // Create and configure the cash operation dialog
                var cashDropDialog = new CashOperationDialogViewModel(_dialogService);
                cashDropDialog.ConfigureForOperation("Drop", Wallets, cashWallet.Balance);

                var dialogResult = await _dialogService.ShowDialogAsync(cashDropDialog);

                if (dialogResult == true && cashDropDialog.Result != null)
                {
                    var result = cashDropDialog.Result;

                    // Create a cash drop transaction
                    var transaction = new TransactionModel
                    {
                        Type = TransactionType.CashDrop,
                        Amount = result.Amount,
                        Description = result.Reason,
                        Reference = $"DROP-{DateTime.Now:yyyyMMddHHmmss}",
                        Timestamp = DateTime.Now,
                        Wallet = result.Wallet
                    };

                    // Process the cash drop through the transaction service
                    var success = await _transactionService.AddTransactionAsync(transaction);

                    if (success)
                    {
                        // Update the wallet balance
                        result.Wallet.Balance -= result.Amount;

                        // Update current shift if active
                        if (CurrentShift != null)
                        {
                            CurrentShift.Transactions.Add(transaction);
                        }

                        // Add to recent transactions
                        RecentTransactions.Insert(0, transaction);
                        CurrentShift!.AddTransaction(transaction);

                        // Update total balance
                        TotalBalance = Wallets.Sum(w => w.Balance);

                        // Notify property changes
                        OnPropertyChanged(nameof(FormattedCurrentTillBalance));

                        // Refresh cash flow summary
                        RefreshCashFlowSummary();

                        // Create custom content view model for the success dialog
                        var contentViewModel = new CashOperationSuccessContentViewModel
                        {
                            OperationType = "Drop",
                            FormattedAmount = result.Amount.ToString("C"),
                            WalletName = result.Wallet.Name,
                            Reason = result.Reason,
                            FormattedNewBalance = result.Wallet.Balance.ToString("C"),
                            Time = transaction.FormattedTimestamp
                        };

                        // Create the custom view and set its DataContext
                        var contentView = new Views.Dialogs.CashOperationSuccessView
                        {
                            DataContext = contentViewModel
                        };

                        // Create a dialog with custom content
                        var dialog = new BaseDialogViewModel
                        {
                            Title = "Cash Drop Complete",
                            Content = contentView,
                            PrimaryButtonText = "Done",
                            PrimaryButtonCommand = new RelayCommand(() => _dialogService.CloseDialog(true))
                        };

                        await _dialogService.ShowDialogAsync(dialog);
                    }
                    else
                    {
                        await _dialogService.ShowErrorAsync(title: "Error", message: "Failed to process cash drop.");
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync(title: "Error performing cash drop", message: ex.Message);
            }
        }

        private async Task OnCashAddAsync(object? parameter)
        {
            if (!IsShiftActive)
            {
                await _dialogService.ShowErrorAsync(title: "No Active Shift", message: "Please start a shift before performing cash operations.");
                return;
            }

            try
            {
                // Get current cash balance
                var cashWallet = Wallets.FirstOrDefault(w => w.Name == "Cash Register");
                if (cashWallet == null)
                {
                    await _dialogService.ShowErrorAsync(title: "Error", message: "Cash register wallet not found.");
                    return;
                }

                // Create and configure the cash operation dialog
                var cashAddDialog = new CashOperationDialogViewModel(_dialogService);
                cashAddDialog.ConfigureForOperation("Add", Wallets, cashWallet.Balance);

                var dialogResult = await _dialogService.ShowDialogAsync(cashAddDialog);

                if (dialogResult == true && cashAddDialog.Result != null)
                {
                    var result = cashAddDialog.Result;

                    // Create a cash add transaction
                    var transaction = new TransactionModel
                    {
                        Type = TransactionType.CashAdd,
                        Amount = result.Amount,
                        Description = result.Reason,
                        Reference = $"ADD-{DateTime.Now:yyyyMMddHHmmss}",
                        Timestamp = DateTime.Now,
                        Wallet = result.Wallet
                    };

                    // Process the cash add through the transaction service
                    var success = await _transactionService.AddTransactionAsync(transaction);

                    if (success)
                    {
                        // Update the wallet balance
                        result.Wallet.Balance += result.Amount;

                        // Update current shift if active
                        if (CurrentShift != null)
                        {
                            CurrentShift.Transactions.Add(transaction);
                        }

                        // Add to recent transactions
                        RecentTransactions.Insert(0, transaction);
                        CurrentShift!.AddTransaction(transaction);
                        
                        // Update total balance
                        TotalBalance = Wallets.Sum(w => w.Balance);

                        // Notify property changes
                        OnPropertyChanged(nameof(FormattedCurrentTillBalance));

                        // Refresh cash flow summary
                        RefreshCashFlowSummary();

                        // Create custom content view model for the success dialog
                        var contentViewModel = new CashOperationSuccessContentViewModel
                        {
                            OperationType = "Add",
                            FormattedAmount = result.Amount.ToString("C"),
                            WalletName = result.Wallet.Name,
                            Reason = result.Reason,
                            FormattedNewBalance = result.Wallet.Balance.ToString("C"),
                            Time = transaction.FormattedTimestamp
                        };

                        // Create the custom view and set its DataContext
                        var contentView = new Views.Dialogs.CashOperationSuccessView
                        {
                            DataContext = contentViewModel
                        };

                        // Create a dialog with custom content
                        var dialog = new BaseDialogViewModel
                        {
                            Title = "Cash Add Complete",
                            Content = contentView,
                            PrimaryButtonText = "Done",
                            PrimaryButtonCommand = new RelayCommand(() => _dialogService.CloseDialog(true))
                        };

                        await _dialogService.ShowDialogAsync(dialog);
                    }
                    else
                    {
                        await _dialogService.ShowErrorAsync(title: "Error", message: "Failed to process cash add.");
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync(title: "Error adding cash", message: ex.Message);
            }
        }

        private async Task OnViewTransactionDetailsAsync(object? parameter)
        {
            if (parameter is TransactionModel transaction)
            {

                await _dialogService.ShowInformationAsync(title: "Transaction Details", message:
                    $"Type: {transaction.TypeDisplayName}\n" +
                    $"Amount: {transaction.FormattedAmount}\n" +
                    $"Balance Change: {transaction.FormattedBalanceChange}\n" +
                    $"Time: {transaction.FormattedTimestamp}\n" +
                    $"Description: {transaction.Description}\n" +
                    $"Reference: {transaction.Reference}");
            }
        }
             
        private void RefreshCashFlowSummary()
        {
            CashFlowSummary.Items.Clear();

            // Convert recent transactions to cash flow items
            foreach (var transaction in RecentTransactions)
            {
                var cashFlowType = transaction.Type switch
                {
                    TransactionType.Sale => CashFlowType.Sale,
                    TransactionType.Refund => CashFlowType.Refund,
                    TransactionType.CashDrop => CashFlowType.CashDrop,
                    TransactionType.CashAdd => CashFlowType.CashAdd,
                    _ => CashFlowType.Other
                };

                // Determine if this transaction should count as an expense
                // Only cash drops from "Cash Register" should count as expenses
                bool countsAsExpense = true;
                if (transaction.Type == TransactionType.CashDrop)
                {
                    // If the wallet is not "Cash Register", don't count it as expense
                    countsAsExpense = transaction.Wallet?.Name == "Cash Register";
                }

                var item = new CashFlowItemModel
                {
                    Id = transaction.Id,
                    Time = transaction.Timestamp,
                    Type = cashFlowType,
                    Amount = transaction.BalanceChange,
                    Description = transaction.Description,
                    Category = transaction.TypeDisplayName,
                    CountsAsExpense = countsAsExpense
                };

                CashFlowSummary.AddItem(item);
            }

            CashFlowSummary.RefreshCalculations();
        }

    }
}