namespace StorePOS.Client.Wpf.MVVM.Models
{
    public class ProductModel : ModelBase
    {
        private int _id;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private decimal _price;
        private decimal _cost;
        private int _stockQuantity;
        private string _category = string.Empty;
        private string _sku = string.Empty;
        private string _barcode = string.Empty;
        private string _imageUrl = string.Empty;
        private DateTime _createdDate;
        private DateTime _lastUpdated;
        private string _brand = string.Empty;
        private string _supplier = string.Empty;
        private decimal _weight;
        private string _unit = string.Empty;
        private decimal _discount;
        private bool _isTaxable;
        private decimal _taxRate;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public decimal Cost
        {
            get => _cost;
            set => SetProperty(ref _cost, value);
        }

        public int StockQuantity
        {
            get => _stockQuantity;
            set => SetProperty(ref _stockQuantity, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public string SKU
        {
            get => _sku;
            set => SetProperty(ref _sku, value);
        }

        public string Barcode
        {
            get => _barcode;
            set => SetProperty(ref _barcode, value);
        }

        public string ImageUrl
        {
            get => _imageUrl;
            set => SetProperty(ref _imageUrl, value);
        }

        public bool IsActive
        {
            get { return _stockQuantity > 0; }
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set => SetProperty(ref _createdDate, value);
        }

        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set => SetProperty(ref _lastUpdated, value);
        }

        public string Brand
        {
            get => _brand;
            set => SetProperty(ref _brand, value);
        }

        public string Supplier
        {
            get => _supplier;
            set => SetProperty(ref _supplier, value);
        }

        public decimal Weight
        {
            get => _weight;
            set => SetProperty(ref _weight, value);
        }

        public string Unit
        {
            get => _unit; 
            set => SetProperty(ref _unit, value);
        }

        public decimal Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value);
        }

        public bool IsTaxable
        {
            get => _isTaxable;
            set => SetProperty(ref _isTaxable, value);
        }

        public decimal TaxRate
        {
            get => _taxRate;
            set => SetProperty(ref _taxRate, value);
        }

        // Calculated Properties
        public string FormattedPrice => $"{Price:C}";
        public string FormattedCost => $"{Cost:C}";
        public decimal ProfitMargin => Price > 0 ? ((Price - Cost) / Price) * 100 : 0;
        public string FormattedProfitMargin => $"{ProfitMargin:F1}%";
        public bool IsLowStock => StockQuantity <= 10;
        public bool IsOutOfStock => StockQuantity <= 0;
        public string StockStatus => IsOutOfStock ? "Out of Stock" : IsLowStock ? "Low Stock" : "In Stock";
        public string StockStatusColor => IsOutOfStock ? "#F44336" : IsLowStock ? "#FF9800" : "#4CAF50";
        public decimal DiscountedPrice => Price - (Price * Discount / 100);
        public bool HasDiscount => Discount > 0;
        public decimal TaxAmount => IsTaxable ? DiscountedPrice * (TaxRate / 100) : 0;
        public decimal FinalPrice => DiscountedPrice + TaxAmount;
        public string FormattedFinalPrice => $"{FinalPrice:C}";
        public string FormattedDiscountedPrice => $"{DiscountedPrice:C}";
        public decimal TotalValue => Price * StockQuantity;
        public string FormattedTotalValue => $"{TotalValue:C}";
        public string DisplayImageUrl => string.IsNullOrEmpty(ImageUrl) ? "/Assets/Images/no-image.png" : ImageUrl;
    }
}