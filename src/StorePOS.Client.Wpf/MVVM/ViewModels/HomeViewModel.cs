using StorePOS.Client.Wpf.Commands;
using StorePOS.Client.Wpf.MVVM.Models;
using StorePOS.Client.Wpf.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StorePOS.Client.Wpf.MVVM.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IMainWindowService _mainWindowService;
        private readonly INavigationService _navigationService;


        /// <summary>
        /// Gets the collection of menu items for navigation.
        /// </summary>
        public ObservableCollection<MenuItemModel> MenuItems { get; } = new();

        /// <summary>
        /// Gets the command for navigation operations.
        /// </summary>
        public AsyncRelayCommand<string>? NavigateCommand { get; }

        /// <summary>
        /// Gets the command for refreshing the current view.
        /// </summary>
        public AsyncRelayCommand RefreshCommand { get; }

        /// <summary>
        /// Gets the currently active view from the navigation service.
        /// </summary>
        public object? CurrentView => _navigationService.CurrentView;


        private string _selectedMenuItem = "Dashboard";

        /// <summary>
        /// Gets or sets the currently selected menu item.
        /// </summary>
        public string SelectedMenuItem
        {
            get => _selectedMenuItem;
            private set
            {
                if (SetProperty(ref _selectedMenuItem, value))
                {
                    UpdateMenuItemSelection();
                    Title = _navigationService.GetViewTitle(value);
                }
            }
        }


        public HomeViewModel(IDialogService dialogService, IMainWindowService mainWindowService, INavigationService navigationService)
        {
            _dialogService = dialogService;
            _mainWindowService = mainWindowService;
            _navigationService = navigationService;
            Title = "Home";

            // Initialize the navigate command
            NavigateCommand = new AsyncRelayCommand<string>(NavigateToViewAsync);
            
            // Initialize the refresh command
            RefreshCommand = new AsyncRelayCommand(RefreshCurrentViewAsync);

            InitializeMenuItems();
            SubscribeToNavigationEvents();
        }

        /// <summary>
        /// Navigates to the specified view.
        /// </summary>
        /// <param name="viewName">The name of the view to navigate to.</param>
        private async Task NavigateToViewAsync(string? viewName)
        {
            if (string.IsNullOrWhiteSpace(viewName))
                return;

            IsBusy = true;
            try
            {
                await _navigationService.NavigateToAsync(viewName);
                SelectedMenuItem = viewName;
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Refreshes the current view by calling RefreshAsync on ViewModels that implement IRefreshable.
        /// </summary>
        private async Task RefreshCurrentViewAsync()
        {
            if (CurrentView is not System.Windows.Controls.UserControl userControl)
                return;

            IsBusy = true;
            try
            {
                // Check if the DataContext implements IRefreshable
                if (userControl.DataContext is IRefreshable refreshableViewModel)
                {
                    await refreshableViewModel.RefreshAsync();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }


        /// <summary>
        /// Initializes the menu items for navigation.
        /// </summary>
        private void InitializeMenuItems()
        {
            var menuItems = new[]
            {
                new MenuItemModel("Dashboard", "ViewDashboard") { IsSelected = true },
                new MenuItemModel("Cashier", "CashRegister"),
                new MenuItemModel("Products", "Package"),
                new MenuItemModel("Inventory", "Warehouse"),
                new MenuItemModel("Sales", "Store"),
                new MenuItemModel("Reports", "ChartLine"),
                new MenuItemModel("Backoffice", "OfficeBuildingOutline"),
                new MenuItemModel("Settings", "Cog")
            };

            foreach (var item in menuItems)
            {
                MenuItems.Add(item);
            }
        }

        /// <summary>
        /// Initializes the view model with any async operations
        /// </summary>
        public async Task InitializeAsync()
        {
            IsBusy = true;
            try
            {
                await _navigationService.NavigateToAsync("Dashboard");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Subscribes to navigation service events to update the current view.
        /// </summary>
        private void SubscribeToNavigationEvents()
        {
            _navigationService.NavigationOccurred += OnNavigationOccurred;
        }

        /// <summary>
        /// Handles navigation events from the navigation service.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The navigation event arguments.</param>
        private void OnNavigationOccurred(object? sender, NavigationEventArgs e)
        {
            // Update the current view property
            OnPropertyChanged(nameof(CurrentView));

            // Update busy state from the ViewModel if applicable
            SubscribeToCurrentViewModelPropertyChanges();
        }

        /// <summary>
        /// Subscribes to property changes from the current ViewModel to update busy state.
        /// </summary>
        private void SubscribeToCurrentViewModelPropertyChanges()
        {
            if (CurrentView is System.Windows.Controls.UserControl userControl &&
                userControl.DataContext is ViewModelBase viewModel)
            {
                // Unsubscribe from previous view model if any
                viewModel.PropertyChanged -= OnCurrentViewModelPropertyChanged;

                // Subscribe to new view model
                viewModel.PropertyChanged += OnCurrentViewModelPropertyChanged;

                // Update initial busy state
                IsBusy = viewModel.IsBusy;
            }
        }

        /// <summary>
        /// Handles property changes from the current ViewModel.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The property changed event arguments.</param>
        private void OnCurrentViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModelBase.IsBusy) && sender is ViewModelBase viewModel)
            {
                IsBusy = viewModel.IsBusy;
            }
        }


        /// <summary>
        /// Updates the selection state of menu items based on the currently selected item.
        /// </summary>
        private void UpdateMenuItemSelection()
        {
            foreach (var item in MenuItems)
            {
                item.IsSelected = item.Name.Equals(SelectedMenuItem, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
