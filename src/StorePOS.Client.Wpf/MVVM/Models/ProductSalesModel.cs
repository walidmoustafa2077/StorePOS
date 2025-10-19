namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Model for product sales data
    /// </summary>
    public class ProductSalesModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int Rank { get; set; } // For display ranking (1-5)
    }
}