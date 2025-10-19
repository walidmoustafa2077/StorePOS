
using StorePOS.Client.Wpf.MVVM.Views.HomeViews;
using System.Windows.Controls;

namespace StorePOS.Client.Wpf.Services.Implementations
{
    /// <summary>
    /// Default implementation of INavigationService.
    /// Handles navigation between views while maintaining state and performance.
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Type> _viewRegistry = new();
        private readonly Dictionary<string, string> _viewTitles = new();
        private readonly Dictionary<string, object> _viewCache = new();
        
        private object? _currentView;

        public object? CurrentView
        {
            get => _currentView;
            private set => _currentView = value;
        }

        public event EventHandler<NavigationEventArgs>? NavigationOccurred;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            RegisterViews();
        }

        /// <summary>
        /// Registers available views and their titles.
        /// </summary>
        private void RegisterViews()
        {
            // Register view types for menu items
            _viewRegistry["Dashboard"] = typeof(DashboardView);
            _viewRegistry["Cashier"] = typeof(CashierView);
            _viewRegistry["Products"] = typeof(ProductsView);
            _viewRegistry["Inventory"] = typeof(InventoryView);
            _viewRegistry["Sales"] = typeof(SalesView);
            _viewRegistry["Settings"] = typeof(SettingsView);
            // Reports, Backoffice will use placeholders until implemented

            // Register view titles for menu items
            _viewTitles["Dashboard"] = "Dashboard";
            _viewTitles["Cashier"] = "Point of Sale";
            _viewTitles["Products"] = "Product Management";
            _viewTitles["Inventory"] = "Inventory Management";
            _viewTitles["Sales"] = "Sales Management";
            _viewTitles["Reports"] = "Reports & Analytics";
            _viewTitles["Backoffice"] = "Backoffice Management";
            _viewTitles["Settings"] = "Settings";
        }

        public bool CanNavigateTo(string viewName)
        {
            return _viewTitles.ContainsKey(viewName);
        }

        public string GetViewTitle(string viewName)
        {
            return _viewTitles.TryGetValue(viewName, out var title) ? title : viewName;
        }

        public async Task NavigateToAsync(string viewName, object? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(viewName))
            {
                throw new ArgumentException("View name cannot be null or empty.", nameof(viewName));
            }

            if (!CanNavigateTo(viewName))
            {
                throw new InvalidOperationException($"Cannot navigate to view: {viewName}");
            }

            // Call OnNavigatedFromAsync on the previous view's ViewModel if it implements INavigationAware
            if (CurrentView != null)
            {
                var previousViewModel = GetViewModelFromView(CurrentView);
                if (previousViewModel is MVVM.ViewModels.INavigationAware previousNavigationAware)
                {
                    await previousNavigationAware.OnNavigatedFromAsync();
                }
            }

            // Get or create the view
            var view = GetOrCreateView(viewName, parameters);
            
            // Update current view
            CurrentView = view;

            // Call OnNavigatedToAsync on the new view's ViewModel if it implements INavigationAware
            var viewModel = GetViewModelFromView(view);
            if (viewModel is MVVM.ViewModels.INavigationAware navigationAware)
            {
                await navigationAware.OnNavigatedToAsync();
            }

            // Raise navigation event
            NavigationOccurred?.Invoke(this, new NavigationEventArgs(viewName, view, parameters));
        }

        /// <summary>
        /// Gets or creates a view for the specified view name.
        /// </summary>
        private object GetOrCreateView(string viewName, object? parameters)
        {
            // Check if we have a registered type for this view
            if (_viewRegistry.TryGetValue(viewName, out var viewType))
            {
                // Try to get from cache
                if (_viewCache.TryGetValue(viewName, out var cachedView))
                {
                    return cachedView;
                }

                // Create new instance from service provider
                var view = _serviceProvider.GetService(viewType);
                if (view != null)
                {
                    _viewCache[viewName] = view;
                    return view;
                }
            }

            // Create a placeholder view for views that don't exist yet
            return CreatePlaceholderView(viewName);
        }

        /// <summary>
        /// Gets the ViewModel (DataContext) from a view object
        /// </summary>
        private object? GetViewModelFromView(object view)
        {
            if (view is System.Windows.FrameworkElement frameworkElement)
            {
                return frameworkElement.DataContext;
            }
            return null;
        }

        /// <summary>
        /// Creates a placeholder view for views that haven't been implemented yet.
        /// </summary>
        private object CreatePlaceholderView(string viewName)
        {
            var textBlock = new TextBlock
            {
                Text = $"{GetViewTitle(viewName)} - Coming Soon",
                FontSize = 24,
                FontWeight = System.Windows.FontWeights.Bold,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Margin = new System.Windows.Thickness(20)
            };

            var grid = new Grid();
            grid.Children.Add(textBlock);

            return grid;
        }
    }
}