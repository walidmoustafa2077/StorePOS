namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Model for receipt data
    /// </summary>
    public class ReceiptModel
    {
        public int SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public IEnumerable<CartItemModel> Items { get; set; } = new List<CartItemModel>();
        public decimal Subtotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal GrandTotal { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string CashierName { get; set; } = string.Empty;
    }
}