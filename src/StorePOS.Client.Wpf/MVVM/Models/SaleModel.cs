using StorePOS.Client.Wpf.Enums;

namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Represents a completed sale transaction.
    /// </summary>
    public class SaleModel : ModelBase
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; } = DateTime.Now;
        public string SaleNumber { get; set; } = string.Empty;
        public List<CartItemModel> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal ChangeAmount { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public string CustomerName { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string CashierName { get; set; } = string.Empty;

        // Refund tracking
        public bool IsRefunded { get; set; } = false;
        public bool IsPartiallyRefunded { get; set; } = false;
        public decimal TotalRefundAmount { get; set; } = 0;
        public DateTime? RefundDate { get; set; }
        public string RefundReason { get; set; } = string.Empty;
        public string RefundedBy { get; set; } = string.Empty;

        // Formatted Properties
        public string FormattedSaleDate => SaleDate.ToString("MM/dd/yyyy hh:mm tt");
        public string FormattedSubtotal => Subtotal.ToString("C");
        public string FormattedTotalTax => TotalTax.ToString("C");
        public string FormattedTotalAmount => TotalAmount.ToString("C");
        public string FormattedAmountPaid => AmountPaid.ToString("C");
        public string FormattedChangeAmount => ChangeAmount.ToString("C");
        public string FormattedRefundAmount => TotalRefundAmount.ToString("C");
        
        public string RefundStatus
        {
            get
            {
                if (IsRefunded) return "Fully Refunded";
                if (IsPartiallyRefunded) return "Partially Refunded";
                return "Active";
            }
        }
    }
}