namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Model for sales summary data
    /// </summary>
    public class SalesSummaryModel
    {
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageSaleAmount { get; set; }
        public int ItemsSold { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}