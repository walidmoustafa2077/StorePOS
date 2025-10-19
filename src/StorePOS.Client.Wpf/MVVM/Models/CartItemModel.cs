using System;
using System.ComponentModel;

namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Represents an item in the shopping cart with quantity and calculated totals.
    /// </summary>
    public class CartItemModel : ModelBase
    {
        private int _quantity = 1;
        private decimal _discount = 0;
        private string _notes = string.Empty;

        // Sale Item ID (for existing sale items, 0 for new cart items)
        public int Id { get; set; }

        // Refund tracking properties
        public int QuantityRefunded { get; set; }
        public decimal RefundAmount { get; set; }
        public bool IsFullyRefunded { get; set; }

        public ProductModel Product { get; set; } = null!;

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (SetProperty(ref _quantity, Math.Max(1, value)))
                {
                    NotifyCalculatedPropertiesChanged();
                }
            }
        }

        public decimal Discount
        {
            get => _discount;
            set
            {
                if (SetProperty(ref _discount, Math.Max(0, Math.Min(value, Product?.Price ?? 0))))
                {
                    NotifyCalculatedPropertiesChanged();
                }
            }
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        // Calculated Properties
        public decimal UnitPrice => Product?.Price ?? 0;
        public decimal LineTotal => (UnitPrice - Discount) * Quantity;
        public decimal TaxAmount => LineTotal * (Product?.TaxRate ?? 0) / 100;
        public decimal LineTotalWithTax => LineTotal + TaxAmount;
        public string FormattedLineTotal => LineTotal.ToString("C");
        public string FormattedLineTotalWithTax => LineTotalWithTax.ToString("C");
        public string DisplayName => Product?.Name ?? "Unknown Product";
        public string ProductName => Product?.Name ?? "Unknown Product";
        public string DisplayPrice => UnitPrice.ToString("C");
        public string FormattedUnitPrice => UnitPrice.ToString("C");

        private void NotifyCalculatedPropertiesChanged()
        {
            OnPropertyChanged(nameof(LineTotal));
            OnPropertyChanged(nameof(TaxAmount));
            OnPropertyChanged(nameof(LineTotalWithTax));
            OnPropertyChanged(nameof(FormattedLineTotal));
            OnPropertyChanged(nameof(FormattedLineTotalWithTax));
            OnPropertyChanged(nameof(FormattedUnitPrice));
        }
    }
}