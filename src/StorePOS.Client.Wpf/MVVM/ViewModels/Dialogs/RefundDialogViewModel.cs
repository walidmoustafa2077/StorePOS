using StorePOS.Client.Wpf.Commands;
using StorePOS.Client.Wpf.MVVM.Models;
using System.Collections.ObjectModel;

using StorePOS.Client.Wpf.Services;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs
{
    /// <summary>
    /// ViewModel for the refund dialog to select items and quantities to refund
    /// </summary>
    public class RefundDialogViewModel : BaseDialogViewModel
    {
        private readonly IDialogService _dialogService;
        private string _refundReason = string.Empty;
        private ObservableCollection<RefundItemModel> _items = new();
        private bool? _dialogResult;

        public RefundDialogViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            Title = "Process Refund";
            
            SelectAllCommand = new RelayCommand(ExecuteSelectAll);
            DeselectAllCommand = new RelayCommand(ExecuteDeselectAll);
            ConfirmCommand = new RelayCommand(ExecuteConfirm, () => CanConfirm);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        public bool IsConfirmed => DialogResult == true;

        public string SaleNumber { get; set; } = string.Empty;
        public string SaleDate { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;

        public ObservableCollection<RefundItemModel> Items
        {
            get => _items;
            set
            {
                if (SetProperty(ref _items, value))
                {
                    OnPropertyChanged(nameof(TotalRefundAmount));
                    OnPropertyChanged(nameof(FormattedTotalRefundAmount));
                    OnPropertyChanged(nameof(CanConfirm));
                    ConfirmCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string RefundReason
        {
            get => _refundReason;
            set => SetProperty(ref _refundReason, value);
        }

        public decimal TotalRefundAmount => Items?.Where(i => i.IsSelected).Sum(i => i.RefundAmount) ?? 0;
        public string FormattedTotalRefundAmount => TotalRefundAmount.ToString("C");

        public bool CanConfirm => Items?.Any(i => i.IsSelected && i.RefundQuantity > 0) ?? false;

        public RelayCommand SelectAllCommand { get; }
        public RelayCommand DeselectAllCommand { get; }
        public RelayCommand ConfirmCommand { get; }
        public RelayCommand CancelCommand { get; }

        public void InitializeItems(IEnumerable<RefundItemModel> items)
        {
            Items = new ObservableCollection<RefundItemModel>(items);
            
            // Subscribe to property changes for each item
            foreach (var item in Items)
            {
                item.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(RefundItemModel.IsSelected) || 
                        e.PropertyName == nameof(RefundItemModel.RefundQuantity))
                    {
                        OnPropertyChanged(nameof(TotalRefundAmount));
                        OnPropertyChanged(nameof(FormattedTotalRefundAmount));
                        OnPropertyChanged(nameof(CanConfirm));
                        ConfirmCommand.RaiseCanExecuteChanged();
                    }
                };
            }
        }

        private void ExecuteSelectAll()
        {
            foreach (var item in Items.Where(i => i.MaxRefundableQuantity > 0))
            {
                item.IsSelected = true;
                item.RefundQuantity = item.MaxRefundableQuantity;
            }
        }

        private void ExecuteDeselectAll()
        {
            foreach (var item in Items)
            {
                item.IsSelected = false;
                item.RefundQuantity = 0;
            }
        }

        private void ExecuteConfirm()
        {
            DialogResult = true;
            CloseDialog(true);
        }

        private void ExecuteCancel()
        {
            DialogResult = false;
            CloseDialog(false);
        }

        private void CloseDialog(bool? result)
        {
            _dialogService.CloseDialog(result);
        }
    }
}
