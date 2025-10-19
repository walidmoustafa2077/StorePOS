namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Model for shift summary data
    /// </summary>
    public class ShiftSummaryModel
    {
        public int ShiftId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string CashierName { get; set; } = string.Empty;
        public decimal OpeningCash { get; set; }
        public decimal ClosingCash { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal CashVariance { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}