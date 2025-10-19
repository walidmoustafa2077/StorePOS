using StorePOS.Client.Wpf.Commands;
using StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels.Settings;
using StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels.SettingsViewModels;
using StorePOS.Client.Wpf.MVVM.Views.HomeViews.SettingsViews;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels
{
    /// <summary>
    /// ViewModel for the Settings view with dynamic content switching
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private int _selectedTabIndex;
        private object? _currentSettingsContent;

        // Sub ViewModels
        private readonly GeneralSettingsViewModel _generalSettingsViewModel;
        private readonly CashierSettingsViewModel _cashierSettingsViewModel;
        private readonly ReceiptPrintingSettingsViewModel _receiptPrintingSettingsViewModel;
        private readonly DatabaseSystemSettingsViewModel _databaseSystemSettingsViewModel;
        private readonly UsersSecuritySettingsViewModel _usersSecuritySettingsViewModel;

        /// <summary>
        /// Gets or sets the currently selected tab index
        /// </summary>
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (SetProperty(ref _selectedTabIndex, value))
                {
                    LoadSettingsContent(value);
                }
            }
        }

        /// <summary>
        /// Gets the current settings content view
        /// </summary>
        public object? CurrentSettingsContent
        {
            get => _currentSettingsContent;
            private set => SetProperty(ref _currentSettingsContent, value);
        }

        /// <summary>
        /// Gets the command to save all settings
        /// </summary>
        public AsyncRelayCommand SaveAllCommand { get; }

        /// <summary>
        /// Gets the command to reset settings
        /// </summary>
        public AsyncRelayCommand ResetCommand { get; }

        public SettingsViewModel()
        {
            Title = "Settings";

            // Initialize sub ViewModels
            _generalSettingsViewModel = new GeneralSettingsViewModel();
            _cashierSettingsViewModel = new CashierSettingsViewModel();
            _receiptPrintingSettingsViewModel = new ReceiptPrintingSettingsViewModel();
            _databaseSystemSettingsViewModel = new DatabaseSystemSettingsViewModel();
            _usersSecuritySettingsViewModel = new UsersSecuritySettingsViewModel();

            // Initialize commands
            SaveAllCommand = new AsyncRelayCommand(SaveAllSettingsAsync);
            ResetCommand = new AsyncRelayCommand(ResetSettingsAsync);

            // Load initial content (General Settings)
            LoadSettingsContent(0);
        }

        /// <summary>
        /// Loads the appropriate settings content based on selected tab
        /// </summary>
        private void LoadSettingsContent(int tabIndex)
        {
            CurrentSettingsContent = tabIndex switch
            {
                0 => new GeneralSettingsView { DataContext = _generalSettingsViewModel },
                1 => new CashierSettingsView { DataContext = _cashierSettingsViewModel },
                2 => new ReceiptPrintingSettingsView { DataContext = _receiptPrintingSettingsViewModel },
                3 => new DatabaseSystemSettingsView { DataContext = _databaseSystemSettingsViewModel },
                4 => new UsersSecuritySettingsView { DataContext = _usersSecuritySettingsViewModel },
                _ => null
            };
        }

        /// <summary>
        /// Saves all settings across all tabs
        /// </summary>
        private async Task SaveAllSettingsAsync()
        {
            IsBusy = true;
            try
            {
                // TODO: Implement save logic for all settings
                await Task.Delay(500); // Simulate async operation

                // Show success message
                // await _dialogService.ShowMessageAsync("Success", "All settings have been saved successfully.");
            }
            catch (Exception)
            {
                // Handle error
                // await _dialogService.ShowErrorAsync("Error", "Failed to save settings.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Resets settings to default values
        /// </summary>
        private async Task ResetSettingsAsync()
        {
            IsBusy = true;
            try
            {
                // TODO: Implement reset logic
                await Task.Delay(500); // Simulate async operation

                // Show confirmation
                // var result = await _dialogService.ShowConfirmationAsync("Reset Settings", 
                //     "Are you sure you want to reset all settings to default values?");
                
                // if (result)
                // {
                //     // Reset settings
                // }
            }
            catch (Exception)
            {
                // Handle error
                // await _dialogService.ShowErrorAsync("Error", "Failed to reset settings.");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
