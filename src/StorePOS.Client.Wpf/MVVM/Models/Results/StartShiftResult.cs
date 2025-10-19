namespace StorePOS.Client.Wpf.MVVM.Models.Results
{
    /// <summary>
    /// Result of the start shift dialog
    /// </summary>
    public class StartShiftResult
    {
        public string CashierName { get; set; } = string.Empty;
        public string ShiftNotes { get; set; } = string.Empty;
        public List<ShiftAccountBalance> AccountBalances { get; set; } = new();
        public decimal TotalStartingBalance { get; set; }
    }
}