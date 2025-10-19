using StorePOS.Client.Wpf.Services;
using System.Windows;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels
{
    /// <summary>
    /// ViewModel for managing product data and operations.
    /// Inherits common product functionality from BaseProductViewModel and adds product-specific operations.
    /// </summary>
    public class ProductsViewModel : BaseProductViewModel
    {
        /// <summary>
        /// Initializes a new instance of ProductsViewModel with dependency injection.
        /// </summary>
        public ProductsViewModel(
            IProductFilterService filterService, 
            IProductService dataService,
            IDebounceService debounceService)
            : base(filterService, dataService, debounceService)
        {
            Title = "Products";
            
            // Load data asynchronously ensuring we're on UI thread
            Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                await InitializeAsync();
            });
        }

        /// <summary>
        /// Initializes product-specific commands in addition to base commands.
        /// </summary>
        protected override void InitializeBaseCommands()
        {
            base.InitializeBaseCommands();
        }
    }
}