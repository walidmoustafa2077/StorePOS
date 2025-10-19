using StorePOS.Client.Wpf.Commands;
using StorePOS.Client.Wpf.Services;
using System.Windows;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels
{
    /// <summary>
    /// ViewModel for managing inventory data with grid view, printing, and exporting capabilities.
    /// Inherits common product functionality from BaseProductViewModel and adds inventory-specific operations.
    /// </summary>
    public class InventoryViewModel : BaseProductViewModel
    {
        // Additional commands for inventory operations
        public RelayCommand PrintInventoryCommand { get; private set; }
        public RelayCommand ExportInventoryCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of InventoryViewModel with dependency injection.
        /// </summary>
        public InventoryViewModel(
            IProductFilterService filterService, 
            IProductService dataService,
            IDebounceService debounceService)
            : base(filterService, dataService, debounceService)
        {
            Title = "Inventory";
            
            // Initialize inventory-specific commands
            PrintInventoryCommand = new RelayCommand(ExecutePrintInventory);
            ExportInventoryCommand = new RelayCommand(ExecuteExportInventory);
            
            // Load data asynchronously ensuring we're on UI thread
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await InitializeAsync();
            });
        }

        /// <summary>
        /// Initializes inventory-specific commands in addition to base commands.
        /// </summary>
        protected override void InitializeBaseCommands()
        {
            base.InitializeBaseCommands();
        }

        /// <summary>
        /// Override to load all filtered products at once for grid view
        /// Grid views work better with all data visible rather than paginated
        /// </summary>
        protected override async Task LoadInitialPageAsync()
        {
            // Clear the collection
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                FilteredProducts.Clear();
            });

            // Load ALL filtered products at once for the grid
            await PopulateProductsIncrementallyAsync(_currentFilteredProducts);

            OnPropertyChanged(nameof(IsEmpty));
        }

        /// <summary>
        /// Override LoadMoreAsync to do nothing since we load all products initially
        /// </summary>
        protected override async Task LoadMoreAsync()
        {
            // Do nothing - we load all products at once for grid view
            await Task.CompletedTask;
        }

        /// <summary>
        /// Executes the print inventory operation.
        /// </summary>
        private void ExecutePrintInventory()
        {
            try
            {
                // TODO: Implement print functionality
                // This could involve generating a print-friendly report of the filtered inventory
                MessageBox.Show(
                    $"Print functionality will generate a report of {FilteredProductsCount} items.",
                    "Print Inventory",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error printing inventory: {ex.Message}",
                    "Print Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Executes the export inventory operation.
        /// </summary>
        private void ExecuteExportInventory()
        {
            try
            {
                // TODO: Implement export functionality (CSV, Excel, PDF)
                // This could involve exporting the filtered inventory to various formats
                MessageBox.Show(
                    $"Export functionality will export {FilteredProductsCount} items to a file.",
                    "Export Inventory",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error exporting inventory: {ex.Message}",
                    "Export Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
