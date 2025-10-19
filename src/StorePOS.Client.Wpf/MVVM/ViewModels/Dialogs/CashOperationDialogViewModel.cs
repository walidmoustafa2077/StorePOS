using StorePOS.Client.Wpf.Commands;
using StorePOS.Client.Wpf.MVVM.Models;
using StorePOS.Client.Wpf.Services;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel for cash operation dialog (cash drop/add).
    /// Allows users to specify amount and select account/wallet.
    /// </summary>
    public class CashOperationDialogViewModel : BaseDialogViewModel
    {
        private decimal _amount;
        private WalletModel? _selectedWallet;
        private string _reason = string.Empty;
        private string _operationType = string.Empty;
        private bool _isValidAmount = true;
        private string _amountValidationMessage = string.Empty;

        /// <summary>
        /// The amount for the cash operation
        /// </summary>
        [Range(0.01, 9999.99, ErrorMessage = "Amount must be between $0.01 and $9,999.99")]
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (SetProperty(ref _amount, value))
                {
                    ValidateAmount();
                    ConfirmCommand?.RaiseCanExecuteChanged();
                    OnPropertyChanged(nameof(FormattedAmount));
                }
            }
        }

        /// <summary>
        /// The selected wallet/account for the operation
        /// </summary>
        public WalletModel? SelectedWallet
        {
            get => _selectedWallet;
            set
            {
                if (SetProperty(ref _selectedWallet, value))
                {
                    ValidateAmount();
                    ConfirmCommand?.RaiseCanExecuteChanged();
                    OnPropertyChanged(nameof(FormattedAmount));
                }
            }
        }

        /// <summary>
        /// Optional reason for the operation
        /// </summary>
        public string Reason
        {
            get => _reason;
            set => SetProperty(ref _reason, value);
        }

        /// <summary>
        /// The type of operation (Drop/Add)
        /// </summary>
        public string OperationType
        {
            get => _operationType;
            set => SetProperty(ref _operationType, value);
        }

        /// <summary>
        /// Available wallets for selection
        /// </summary>
        public ObservableCollection<WalletModel> AvailableWallets { get; } = new();

        /// <summary>
        /// Whether the amount is valid
        /// </summary>
        public bool IsValidAmount
        {
            get => _isValidAmount;
            set => SetProperty(ref _isValidAmount, value);
        }

        /// <summary>
        /// Amount validation message
        /// </summary>
        public string AmountValidationMessage
        {
            get => _amountValidationMessage;
            set => SetProperty(ref _amountValidationMessage, value);
        }

        /// <summary>
        /// Formatted amount for display
        /// </summary>
        public string FormattedAmount => Amount.ToString("C");

        /// <summary>
        /// Commands
        /// </summary>
        public RelayCommand ConfirmCommand { get; private set; } = null!;
        public RelayCommand CancelCommand { get; private set; } = null!;
        public RelayCommand ClearAmountCommand { get; private set; } = null!;
        public RelayCommand SetQuickAmountCommand { get; private set; } = null!;

        /// <summary>
        /// Result of the dialog operation
        /// </summary>
        public CashOperationResult? Result { get; private set; }

        /// <summary>
        /// Parent dialog service for closing
        /// </summary>
        private readonly IDialogService _dialogService;

        public CashOperationDialogViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            InitializeCommands();
            SetupValidation();
        }

        /// <summary>
        /// Configure the dialog for a specific operation type
        /// </summary>
        public void ConfigureForOperation(string operationType, ObservableCollection<WalletModel> wallets, decimal currentBalance = 0)
        {
            OperationType = operationType;
            Title = $"Cash {operationType}";

            // Set default wallet to cash register
            AvailableWallets.Clear();
            foreach (var wallet in wallets)
            {
                AvailableWallets.Add(wallet);
            }

            SelectedWallet = AvailableWallets.FirstOrDefault(w => w.Name == "Cash Register");

            // Set suggested amount based on operation type
            if (operationType == "Drop" && currentBalance > 100)
            {
                Amount = Math.Min(50m, currentBalance - 100m); // Suggest dropping excess above $100
            }
            else if (operationType == "Add")
            {
                Amount = 20m; // Suggest $20 for additions
            }

            // Configure dialog buttons
            PrimaryButtonText = $"Confirm {operationType}";
            SecondaryButtonText = "Cancel";
        }

        private void InitializeCommands()
        {
            ConfirmCommand = new RelayCommand(OnConfirm, CanConfirm);
            CancelCommand = new RelayCommand(OnCancel);
            ClearAmountCommand = new RelayCommand(OnClearAmount);
            SetQuickAmountCommand = new RelayCommand(OnSetQuickAmount);
        }

        private void SetupValidation()
        {
            // Amount starts at 0, so validate initially
            ValidateAmount();
        }

        private bool CanConfirm()
        {
            return IsValidAmount && Amount > 0 && SelectedWallet != null;
        }

        private void OnConfirm()
        {
            if (!CanConfirm()) return;

            Result = new CashOperationResult
            {
                Amount = Amount,
                Wallet = SelectedWallet!,
                Reason = $"Cash {OperationType.ToLower()}",
                OperationType = OperationType
            };

            // Close dialog with positive result
            CloseDialog(true);
        }

        private void OnCancel()
        {
            Result = null;
            CloseDialog(false);
        }

        private void OnClearAmount()
        {
            Amount = 0;
        }

        private void OnSetQuickAmount(object? parameter)
        {
            var amountString = parameter?.ToString();
            if (decimal.TryParse(amountString, out decimal amount))
            {
                Amount = amount;
            }
        }

        private void ValidateAmount()
        {
            IsValidAmount = true;
            AmountValidationMessage = string.Empty;

            if (Amount <= 0)
            {
                IsValidAmount = false;
                AmountValidationMessage = "Amount must be greater than $0.00";
                return;
            }

            if (Amount > 9999.99m)
            {
                IsValidAmount = false;
                AmountValidationMessage = "Amount cannot exceed $9,999.99";
                return;
            }

            // Additional validation for cash drops
            if (OperationType == "Drop" && SelectedWallet?.Name == "Cash Register")
            {
                var currentBalance = SelectedWallet.Balance;
                if (Amount > currentBalance)
                {
                    IsValidAmount = false;
                    AmountValidationMessage = $"Cannot drop more than current balance ({currentBalance:C})";
                    return;
                }
            }
        }

        private void CloseDialog(bool? result)
        {
            _dialogService.CloseDialog(result);
        }
    }
}