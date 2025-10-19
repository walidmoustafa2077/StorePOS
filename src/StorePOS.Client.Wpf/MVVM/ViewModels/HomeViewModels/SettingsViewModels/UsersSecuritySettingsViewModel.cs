using StorePOS.Client.Wpf.Commands;
using StorePOS.Client.Wpf.MVVM.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels.SettingsViewModels
{
    public class UsersSecuritySettingsViewModel : ViewModelBase
    {
        #region User Management Properties
        private ObservableCollection<UserModel> _users = new();
        public ObservableCollection<UserModel> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Security Settings Properties
        private bool _requireLogin = true;
        public bool RequireLogin
        {
            get => _requireLogin;
            set
            {
                _requireLogin = value;
                OnPropertyChanged();
            }
        }

        private string _selectedSessionTimeout = "30";
        public string SelectedSessionTimeout
        {
            get => _selectedSessionTimeout;
            set
            {
                _selectedSessionTimeout = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _sessionTimeouts = new()
        {
            "15",
            "30",
            "60",
            "120",
            "Never"
        };
        public ObservableCollection<string> SessionTimeouts
        {
            get => _sessionTimeouts;
            set
            {
                _sessionTimeouts = value;
                OnPropertyChanged();
            }
        }

        // Password Requirements
        private bool _requireMinLength = true;
        public bool RequireMinLength
        {
            get => _requireMinLength;
            set
            {
                _requireMinLength = value;
                OnPropertyChanged();
            }
        }

        private bool _requireUppercase = true;
        public bool RequireUppercase
        {
            get => _requireUppercase;
            set
            {
                _requireUppercase = value;
                OnPropertyChanged();
            }
        }

        private bool _requireLowercase = true;
        public bool RequireLowercase
        {
            get => _requireLowercase;
            set
            {
                _requireLowercase = value;
                OnPropertyChanged();
            }
        }

        private bool _requireNumbers = true;
        public bool RequireNumbers
        {
            get => _requireNumbers;
            set
            {
                _requireNumbers = value;
                OnPropertyChanged();
            }
        }

        private bool _requireSpecialChars = false;
        public bool RequireSpecialChars
        {
            get => _requireSpecialChars;
            set
            {
                _requireSpecialChars = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Access Control Properties
        private bool _enableRBAC = true;
        public bool EnableRBAC
        {
            get => _enableRBAC;
            set
            {
                _enableRBAC = value;
                OnPropertyChanged();
            }
        }

        private bool _requireManagerForRefunds = true;
        public bool RequireManagerForRefunds
        {
            get => _requireManagerForRefunds;
            set
            {
                _requireManagerForRefunds = value;
                OnPropertyChanged();
            }
        }

        private bool _requireManagerForDiscounts = true;
        public bool RequireManagerForDiscounts
        {
            get => _requireManagerForDiscounts;
            set
            {
                _requireManagerForDiscounts = value;
                OnPropertyChanged();
            }
        }

        private bool _logUserActions = true;
        public bool LogUserActions
        {
            get => _logUserActions;
            set
            {
                _logUserActions = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Audit & Activity Log Properties
        private string _selectedLogRetentionPeriod = "1 Year";
        public string SelectedLogRetentionPeriod
        {
            get => _selectedLogRetentionPeriod;
            set
            {
                _selectedLogRetentionPeriod = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _logRetentionPeriods = new()
        {
            "3 Months",
            "6 Months",
            "1 Year",
            "2 Years",
            "5 Years",
            "Forever"
        };
        public ObservableCollection<string> LogRetentionPeriods
        {
            get => _logRetentionPeriods;
            set
            {
                _logRetentionPeriods = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand ViewActivityLogCommand { get; }
        public ICommand ExportActivityLogCommand { get; }
        #endregion

        public UsersSecuritySettingsViewModel()
        {
            AddUserCommand = new AsyncRelayCommand(AddUserAsync);
            EditUserCommand = new RelayCommand(param => EditUser(param as UserModel));
            DeleteUserCommand = new AsyncRelayCommand<UserModel>(DeleteUserAsync);
            ViewActivityLogCommand = new AsyncRelayCommand(ViewActivityLogAsync);
            ExportActivityLogCommand = new AsyncRelayCommand(ExportActivityLogAsync);

            // Load sample users
            LoadSampleUsers();
        }

        #region Command Methods
        private async Task AddUserAsync()
        {
            try
            {
                // TODO: Show add user dialog
                await Task.Delay(500);

                // Sample: Add new user
                var newUser = new UserModel
                {
                    FullName = "New User",
                    Email = "newuser@example.com",
                    Role = "Cashier",
                    Status = "Active",
                    StatusColor = "#4CAF50"
                };

                Users.Add(newUser);

                System.Windows.MessageBox.Show(
                    "User added successfully!",
                    "Success",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to add user: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private void EditUser(UserModel? user)
        {
            if (user == null) return;

            try
            {
                // TODO: Show edit user dialog
                System.Windows.MessageBox.Show(
                    $"Edit user: {user.FullName}",
                    "Edit User",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to edit user: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task DeleteUserAsync(UserModel? user)
        {
            if (user == null) return;

            try
            {
                var result = System.Windows.MessageBox.Show(
                    $"Are you sure you want to delete user '{user.FullName}'?",
                    "Confirm Delete",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);

                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    // TODO: Implement actual delete logic
                    await Task.Delay(500);
                    Users.Remove(user);

                    System.Windows.MessageBox.Show(
                        "User deleted successfully!",
                        "Success",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to delete user: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task ViewActivityLogAsync()
        {
            try
            {
                // TODO: Open activity log viewer window/dialog
                await Task.Delay(500);

                System.Windows.MessageBox.Show(
                    "Activity log viewer will be displayed here.",
                    "Activity Log",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to load activity log: {ex.Message}",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private async Task ExportActivityLogAsync()
        {
            try
            {
                // TODO: Show save file dialog and export log
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv|Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
                    FileName = $"ActivityLog_{DateTime.Now:yyyyMMdd_HHmmss}",
                    Title = "Export Activity Log"
                };

                if (dialog.ShowDialog() == true)
                {
                    // TODO: Implement export logic
                    await Task.Delay(1000);

                    System.Windows.MessageBox.Show(
                        $"Activity log exported successfully to:\n{dialog.FileName}",
                        "Export Complete",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to export activity log: {ex.Message}",
                    "Export Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private void LoadSampleUsers()
        {
            // Load sample users for demonstration
            Users = new ObservableCollection<UserModel>
            {
                new UserModel
                {
                    FullName = "Admin User",
                    Email = "admin@storepos.com",
                    Role = "Administrator",
                    Status = "Active",
                    StatusColor = "#4CAF50"
                },
                new UserModel
                {
                    FullName = "John Smith",
                    Email = "john.smith@storepos.com",
                    Role = "Manager",
                    Status = "Active",
                    StatusColor = "#4CAF50"
                },
                new UserModel
                {
                    FullName = "Sarah Johnson",
                    Email = "sarah.j@storepos.com",
                    Role = "Cashier",
                    Status = "Active",
                    StatusColor = "#4CAF50"
                },
                new UserModel
                {
                    FullName = "Mike Davis",
                    Email = "mike.d@storepos.com",
                    Role = "Cashier",
                    Status = "Inactive",
                    StatusColor = "#757575"
                },
                new UserModel
                {
                    FullName = "Emma Wilson",
                    Email = "emma.w@storepos.com",
                    Role = "Cashier",
                    Status = "Active",
                    StatusColor = "#4CAF50"
                }
            };
        }
        #endregion
    }

    /// <summary>
    /// Model representing a user in the system
    /// </summary>
    public class UserModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
    }
}
