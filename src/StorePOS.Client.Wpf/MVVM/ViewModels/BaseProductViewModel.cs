using StorePOS.Client.Wpf.Commands;
using StorePOS.Client.Wpf.Extensions;
using StorePOS.Client.Wpf.MVVM.Models;
using StorePOS.Client.Wpf.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace StorePOS.Client.Wpf.MVVM.ViewModels
{
    /// <summary>
    /// Base abstract ViewModel for managing product data and operations.
    /// Provides common functionality for product filtering, searching, and category management.
    /// Follows MVVM pattern with proper separation of concerns and async operations.
    /// </summary>
    public abstract class BaseProductViewModel : ViewModelBase, IRefreshable
    {
        protected readonly IProductFilterService _filterService;
        protected readonly IProductService _dataService;
        protected readonly IDebounceService _debounceService;
        protected CancellationTokenSource _loadCancellationTokenSource = new();

        private string _searchText = string.Empty;
        private CategoryModel? _selectedCategory;
        private ProductModel? _selectedProduct;
        private string _selectedPriceRange = "All";
        private string _selectedStockStatus = "All";
        private bool _showActiveOnly = true;
        private bool _isLoadingMore = false;
        private DateTime _lastLoadMoreTime = DateTime.MinValue;

        // Pagination properties
        protected const int PageSize = 10;
        protected int _currentPage = 0;
        protected List<ProductModel> _currentFilteredProducts = new();
        private bool _hasMoreItems = true;

        public string[] PriceRanges { get; } = { "All", "Under $10", "$10 - $50", "$50 - $100", "Over $100" };
        public string[] StockStatuses { get; } = { "All", "In Stock", "Low Stock", "Out of Stock" };

        public ObservableCollection<ProductModel> AllProducts { get; } = new();
        public ObservableCollection<ProductModel> FilteredProducts { get; } = new();
        public ObservableCollection<CategoryModel> Categories { get; } = new();

        // Common Commands
        public AsyncRelayCommand LoadMoreCommand { get; protected set; } = null!;
        public RelayCommand ClearSearchCommand { get; protected set; } = null!;
        public RelayCommand SelectCategoryCommand { get; protected set; } = null!;
        public RelayCommand ClearFiltersCommand { get; protected set; } = null!;

        // Properties
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    // Use debouncing to prevent excessive filtering during typing
                    ApplyFiltersAsync();
                }
            }
        }

        public CategoryModel? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    ApplyFiltersAsync();
                }
            }
        }

        public ProductModel? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public string SelectedPriceRange
        {
            get => _selectedPriceRange;
            set
            {
                if (SetProperty(ref _selectedPriceRange, value))
                {
                    ApplyFiltersAsync();
                }
            }
        }

        public string SelectedStockStatus
        {
            get => _selectedStockStatus;
            set
            {
                if (SetProperty(ref _selectedStockStatus, value))
                {
                    ApplyFiltersAsync();
                }
            }
        }

        public bool ShowActiveOnly
        {
            get => _showActiveOnly;
            set
            {
                if (SetProperty(ref _showActiveOnly, value))
                {
                    ApplyFiltersAsync();
                }
            }
        }

        public bool IsLoadingMore
        {
            get => _isLoadingMore;
            set => SetProperty(ref _isLoadingMore, value);
        }

        public bool HasMoreItems
        {
            get => _hasMoreItems;
            set => SetProperty(ref _hasMoreItems, value);
        }

        // Summary Properties
        public int TotalProducts => AllProducts.Count;
        public int FilteredProductsCount => FilteredProducts.Count;
        public bool IsEmpty => FilteredProducts.Count <= 0;
        public decimal TotalInventoryValue => AllProducts.Sum(p => p.Cost * p.StockQuantity);
        public string FormattedInventoryValue => $"{TotalInventoryValue:C}";
        public int LowStockCount => AllProducts.Count(p => p.IsLowStock);
        public int OutOfStockCount => AllProducts.Count(p => p.IsOutOfStock);

        /// <summary>
        /// Initializes a new instance of BaseProductViewModel with dependency injection.
        /// </summary>
        protected BaseProductViewModel(
            IProductFilterService filterService,
            IProductService dataService,
            IDebounceService debounceService)
        {
            // Use dependency injection or create default implementations
            _filterService = filterService;
            _dataService = dataService;
            _debounceService = debounceService;

            InitializeBaseCommands();
        }


        /// <summary>
        /// Initializes the common commands. Can be overridden by derived classes to add more commands.
        /// </summary>
        protected virtual void InitializeBaseCommands()
        {
            LoadMoreCommand = new AsyncRelayCommand(LoadMoreAsync, () => !IsLoadingMore && HasMoreItems);
            ClearSearchCommand = new RelayCommand(OnClearSearch);
            SelectCategoryCommand = new RelayCommand(OnSelectCategory);
            ClearFiltersCommand = new RelayCommand(OnClearFilters);
        }

        /// <summary>
        /// Loads initial data. Should be called after constructor.
        /// </summary>
        public virtual async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        protected virtual async Task LoadDataAsync()
        {
            // Cancel any existing load operation
            _loadCancellationTokenSource.Cancel();
            _loadCancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _loadCancellationTokenSource.Token;

            try
            {
                IsBusy = true;

                // Load products from data service
                var products = await _dataService.GetAllProductsAsync();
                if (cancellationToken.IsCancellationRequested) return;

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    AllProducts.Clear();
                    AllProducts.AddRange(products);
                    LoadCategories();
                    
                    var uniqueIds = products.Select(p => p.Id).Distinct().Count();
                    if (products.Count() != uniqueIds)
                    {
                        var duplicates = products.GroupBy(p => p.Id).Where(g => g.Count() > 1);
                    }
                });

                ApplyFiltersAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected virtual void LoadCategories()
        {
            Categories.Clear();

            // Always start with "All Categories"
            Categories.Add(new CategoryModel
            {
                Name = "All Categories",
                ProductCount = AllProducts.Count,
                IsSelected = true // Set as initially selected
            });

            // Group products by category and create category entries
            var grouped = AllProducts
                .GroupBy(p => p.Category)
                .OrderBy(g => g.Key);

            foreach (var group in grouped)
            {
                Categories.Add(new CategoryModel
                {
                    Name = group.Key,
                    ProductCount = group.Count(),
                    IsSelected = false
                });
            }

            // Set default selection
            SelectedCategory = Categories.FirstOrDefault();
        }

        protected virtual void ApplyFiltersAsync()
        {
            _debounceService.Debounce("product-filter", async () =>
            {
                try
                {
                    var criteria = new ProductFilterCriteria
                    {
                        SearchText = SearchText,
                        Category = SelectedCategory?.Name == "All Categories" ? null : SelectedCategory?.Name,
                        PriceRange = SelectedPriceRange,
                        StockStatus = SelectedStockStatus,
                        ShowActiveOnly = SelectedStockStatus == "Out of Stock" ? false : ShowActiveOnly
                    };

                    var filteredResults = await _filterService.FilterProductsAsync(AllProducts.ToList(), criteria);
                    _currentFilteredProducts = filteredResults.ToList();
                    _currentPage = 0;
                    HasMoreItems = _currentFilteredProducts.Count > PageSize;

                    var uniqueFilteredIds = _currentFilteredProducts.Select(p => p.Id).Distinct().Count();

                    await LoadInitialPageAsync();
                }
                catch (Exception)
                {
                    // Handle filtering errors
                }
            }, TimeSpan.FromMilliseconds(250));
        }

        protected virtual async Task LoadInitialPageAsync()
        {
            // Ensure collection is cleared before loading new data
            await Application.Current.Dispatcher.InvokeAsync(() => 
            {
                FilteredProducts.Clear();
            });

            var initialPage = _currentFilteredProducts.Take(PageSize).ToList();
            await PopulateProductsIncrementallyAsync(initialPage);

            _currentPage = 1;
            OnPropertyChanged(nameof(IsEmpty));
        }

        protected virtual async Task LoadMoreAsync()
        {
            if (IsLoadingMore || !HasMoreItems)
                return;

            // Prevent rapid consecutive calls (debounce for 500ms)
            var now = DateTime.Now;
            if ((now - _lastLoadMoreTime).TotalMilliseconds < 500)
            {
                return;
            }
            _lastLoadMoreTime = now;

            try
            {
                IsLoadingMore = true;

                var skip = _currentPage * PageSize;
                var nextBatch = _currentFilteredProducts.Skip(skip).Take(PageSize).ToList();

                if (nextBatch.Any())
                {
                    await PopulateProductsIncrementallyAsync(nextBatch, append: true);
                    _currentPage++;
                }

                HasMoreItems = (_currentPage * PageSize) < _currentFilteredProducts.Count;
            }
            finally
            {
                IsLoadingMore = false;
                LoadMoreCommand.RaiseCanExecuteChanged();
            }
        }

        protected virtual async Task PopulateProductsIncrementallyAsync(IEnumerable<ProductModel> products, bool append = false)
        {
            // Clear existing products if not appending
            if (!append)
            {
                await Application.Current.Dispatcher.InvokeAsync(() => FilteredProducts.Clear());
            }

            // Add products with a small delay for smooth UI experience
            foreach (var product in products)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // Always check for duplicates before adding to prevent duplicate entries
                    if (!FilteredProducts.Any(p => p.Id == product.Id))
                    {
                        FilteredProducts.Add(product);
                    }
                });

                // Small delay to allow UI to update smoothly
                await Task.Delay(1);
            }

            if (!append)
            {
                NotifyInventoryPropertiesChanged();
            }
        }

        protected virtual void NotifyInventoryPropertiesChanged()
        {
            OnPropertyChanged(nameof(TotalProducts));
            OnPropertyChanged(nameof(FilteredProductsCount));
            OnPropertyChanged(nameof(TotalInventoryValue));
            OnPropertyChanged(nameof(FormattedInventoryValue));
            OnPropertyChanged(nameof(LowStockCount));
            OnPropertyChanged(nameof(OutOfStockCount));
        }

        // Event handlers
        protected virtual void OnClearSearch(object? parameter)
        {
            SearchText = string.Empty;
        }

        protected virtual void OnSelectCategory(object? parameter)
        {
            if (parameter is CategoryModel category)
            {
                // Clear previous selection
                foreach (var cat in Categories)
                {
                    cat.IsSelected = false;
                }

                // If clicking the same category, deselect it (go to "All Categories")
                if (SelectedCategory == category && category.Name != "All Categories")
                {
                    SelectedCategory = Categories.FirstOrDefault(c => c.Name == "All Categories");
                }
                else
                {
                    SelectedCategory = category;
                }

                // Set the selected category's IsSelected property
                if (SelectedCategory != null)
                {
                    SelectedCategory.IsSelected = true;
                }
            }
        }

        protected virtual void OnClearFilters(object? parameter)
        {
            SearchText = string.Empty;
            
            // Clear all category selections
            foreach (var cat in Categories)
            {
                cat.IsSelected = false;
            }
            
            // Set "All Categories" as selected
            var allCategory = Categories.FirstOrDefault(c => c.Name == "All Categories");
            if (allCategory != null)
            {
                allCategory.IsSelected = true;
            }
            SelectedCategory = allCategory;
            
            SelectedPriceRange = "All";
            SelectedStockStatus = "All";
            ShowActiveOnly = true;

            // Reset pagination
            _currentPage = 0;
            HasMoreItems = true;
        }

        /// <summary>
        /// Refreshes the product data by reloading from the data source.
        /// Implements IRefreshable interface.
        /// </summary>
        public virtual async Task RefreshAsync()
        {
            await LoadDataAsync();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _loadCancellationTokenSource?.Cancel();
                _loadCancellationTokenSource?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}