namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Criteria class for product filtering operations.
    /// Encapsulates all filter parameters to reduce parameter coupling.
    /// </summary>
    public class ProductFilterCriteria
    {
        public string? SearchText { get; set; }
        public string? Category { get; set; }
        public string PriceRange { get; set; } = "All";
        public string StockStatus { get; set; } = "All";
        public bool ShowActiveOnly { get; set; } = true;
    }
}