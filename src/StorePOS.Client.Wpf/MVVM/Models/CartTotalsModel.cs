namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Model for cart totals calculation
    /// </summary>
    public class CartTotalsModel
    {
        public decimal Subtotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal GrandTotal { get; set; }
        public int TotalItems { get; set; }
    }
}