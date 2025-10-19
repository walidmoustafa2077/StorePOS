namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Model for selecting items and quantities to refund from a sale
    /// </summary>
    public class RefundItemModel : ModelBase
    {
        private bool _isSelected;
        private int _refundQuantity;

        public int SaleItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int OriginalQuantity { get; set; }
        public int QuantityAlreadyRefunded { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public decimal TaxAmount { get; set; }

        public int MaxRefundableQuantity => OriginalQuantity - QuantityAlreadyRefunded;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    // If unselected, reset quantity
                    if (!value)
                    {
                        RefundQuantity = 0;
                    }
                    else if (RefundQuantity == 0)
                    {
                        // If selected and quantity is 0, set to max refundable
                        RefundQuantity = MaxRefundableQuantity;
                    }
                }
            }
        }

        public int RefundQuantity
        {
            get => _refundQuantity;
            set
            {
                var clampedValue = Math.Max(0, Math.Min(value, MaxRefundableQuantity));
                if (SetProperty(ref _refundQuantity, clampedValue))
                {
                    OnPropertyChanged(nameof(RefundAmount));
                    OnPropertyChanged(nameof(FormattedRefundAmount));
                }
            }
        }

        public decimal RefundAmount => (LineTotal / OriginalQuantity) * RefundQuantity;
        public string FormattedRefundAmount => RefundAmount.ToString("C");
        public string FormattedUnitPrice => UnitPrice.ToString("C");
        public string FormattedLineTotal => LineTotal.ToString("C");
    }
}
