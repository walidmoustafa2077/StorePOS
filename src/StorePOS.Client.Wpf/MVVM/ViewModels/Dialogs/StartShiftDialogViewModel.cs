using StorePOS.Client.Wpf.Commands;
using StorePOS.Client.Wpf.Enums;
using StorePOS.Client.Wpf.MVVM.Models;
using StorePOS.Client.Wpf.MVVM.Models.Results;
using StorePOS.Client.Wpf.Services;
using System.Collections.ObjectModel;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel for start shift dialog.
    /// Allows users to enter starting balances for different accounts when beginning a shift.
    /// </summary>
    public class StartShiftDialogViewModel : BaseDialogViewModel
    {
        private string _cashierName = string.Empty;
        private string _shiftNotes = string.Empty;
        private bool _isValidInput = true;
        private string _validationMessage = string.Empty;

        /// <summary>
        /// Name of the cashier starting the shift
        /// </summary>
        public string CashierName
        {
            get => _cashierName;
            set
            {
                if (SetProperty(ref _cashierName, value))
                {
                    ValidateInput();
                    StartShiftCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Optional notes for the shift
        /// </summary>
        public string ShiftNotes
        {
            get => _shiftNotes;
            set => SetProperty(ref _shiftNotes, value);
        }

        /// <summary>
        /// Whether the input is valid
        /// </summary>
        public bool IsValidInput
        {
            get => _isValidInput;
            set => SetProperty(ref _isValidInput, value);
        }

        /// <summary>
        /// Validation message
        /// </summary>
        public string ValidationMessage
        {
            get => _validationMessage;
            set => SetProperty(ref _validationMessage, value);
        }

        /// <summary>
        /// Account balances for the shift start
        /// </summary>
        public ObservableCollection<ShiftAccountBalance> AccountBalances { get; } = new();

        /// <summary>
        /// Commands
        /// </summary>
        public RelayCommand StartShiftCommand { get; private set; } = null!;
        public RelayCommand CancelCommand { get; private set; } = null!;
        public RelayCommand SetQuickCashAmountCommand { get; private set; } = null!;
        public RelayCommand ResetBalancesCommand { get; private set; } = null!;

        /// <summary>
        /// Result of the dialog operation
        /// </summary>
        public StartShiftResult? Result { get; private set; }

        /// <summary>
        /// Parent dialog service for closing
        /// </summary>
        private readonly IDialogService _dialogService;
        private readonly IWalletService _walletService;

        /// <summary>
        /// Total starting balance across all accounts
        /// </summary>
        public decimal TotalStartingBalance => AccountBalances.Sum(ab => ab.StartingBalance);

        /// <summary>
        /// Formatted total starting balance
        /// </summary>
        public string FormattedTotalBalance => TotalStartingBalance.ToString("C");

        public StartShiftDialogViewModel(IDialogService dialogService, IWalletService walletService)
        {
            _dialogService = dialogService;
            _walletService = walletService;
            Title = "Start New Shift";
            PrimaryButtonText = "Start Shift";
            SecondaryButtonText = "Cancel";

            InitializeCommands();
            _ = InitializeAccountBalancesAsync();
            ValidateInput();
        }

        private void InitializeCommands()
        {
            StartShiftCommand = new RelayCommand(OnStartShift, CanStartShift);
            CancelCommand = new RelayCommand(OnCancel);
            SetQuickCashAmountCommand = new RelayCommand(OnSetQuickCashAmount);
            ResetBalancesCommand = new RelayCommand(OnResetBalances);
        }

        private async Task InitializeAccountBalancesAsync()
        {
            try
            {
                // Load active wallets from database
                var wallets = await _walletService.GetActiveWalletsAsync();

                // Convert to ShiftAccountBalance objects
                foreach (var wallet in wallets)
                {
                    var accountBalance = new ShiftAccountBalance
                    {
                        WalletName = wallet.Name,
                        AccountName = wallet.Name,
                        Icon = wallet.IconKind,
                        Description = wallet.Description,
                        StartingBalance = wallet.Name == "Cash Register" ? 100m : wallet.Balance // Default cash to 100
                    };

                    AccountBalances.Add(accountBalance);

                    // Subscribe to property changes
                    accountBalance.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(ShiftAccountBalance.StartingBalance))
                        {
                            OnPropertyChanged(nameof(TotalStartingBalance));
                            OnPropertyChanged(nameof(FormattedTotalBalance));
                            ValidateInput();
                            StartShiftCommand?.RaiseCanExecuteChanged();
                        }
                    };
                }
            }
            catch (Exception)
            {
                // Log error or show notification
                // For now, add a default cash wallet as fallback
                var fallbackAccount = new ShiftAccountBalance
                {
                    WalletName = "Cash Register",
                    AccountName = "Cash Register",
                    Icon = "Cash",
                    Description = "Physical cash in register",
                    StartingBalance = 100m
                };
                AccountBalances.Add(fallbackAccount);
            }
        }

        private bool CanStartShift()
        {
            return IsValidInput &&
                   !string.IsNullOrWhiteSpace(CashierName) &&
                   AccountBalances.Any(ab => ab.StartingBalance > 0);
        }

        private void OnStartShift()
        {
            if (!CanStartShift()) return;

            Result = new StartShiftResult
            {
                CashierName = CashierName.Trim(),
                ShiftNotes = ShiftNotes?.Trim() ?? string.Empty,
                AccountBalances = AccountBalances.ToList(),
                TotalStartingBalance = TotalStartingBalance
            };

            CloseDialog(true);
        }

        private void OnCancel()
        {
            Result = null;
            CloseDialog(false);
        }

        private void OnSetQuickCashAmount(object? parameter)
        {
            var amountString = parameter?.ToString();
            if (decimal.TryParse(amountString, out decimal amount))
            {
                var cashAccount = AccountBalances.FirstOrDefault(ab => ab.WalletName == "Cash Register");
                if (cashAccount != null)
                {
                    cashAccount.StartingBalance = amount;
                }
            }
        }

        private void OnResetBalances()
        {
            foreach (var account in AccountBalances)
            {
                if (account.WalletName == "Cash Register")
                    account.StartingBalance = 100m; // Default cash amount
                else
                    account.StartingBalance = 0m;
            }
        }

        private void ValidateInput()
        {
            IsValidInput = true;
            ValidationMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(CashierName))
            {
                IsValidInput = false;
                ValidationMessage = "Cashier name is required";
                return;
            }

            if (CashierName.Length < 2)
            {
                IsValidInput = false;
                ValidationMessage = "Cashier name must be at least 2 characters";
                return;
            }

            if (!AccountBalances.Any(ab => ab.StartingBalance > 0))
            {
                IsValidInput = false;
                ValidationMessage = "At least one account must have a starting balance";
                return;
            }

            var totalBalance = TotalStartingBalance;
            if (totalBalance > 10000m)
            {
                IsValidInput = false;
                ValidationMessage = "Total starting balance cannot exceed $10,000";
                return;
            }
        }

        private void CloseDialog(bool? result)
        {
            _dialogService.CloseDialog(result);
        }
    }
}