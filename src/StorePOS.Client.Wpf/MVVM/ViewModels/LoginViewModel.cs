using StorePOS.Client.Wpf.Commands;
using StorePOS.Client.Wpf.MVVM.Views;
using StorePOS.Client.Wpf.Services;

namespace StorePOS.Client.Wpf.MVVM.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IMainWindowService _mainWindowService;
        private readonly HomeView _homeView;

        public LoginViewModel(IDialogService dialogService, IMainWindowService mainWindowService, HomeView homeView)
        {
            _dialogService = dialogService;
            _mainWindowService = mainWindowService;
            _homeView = homeView;

            Title = "Login";
            LoginCommand = new AsyncRelayCommand(ExecuteLoginAsync, CanLogin);
        }

        private string _username = "";
        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                    LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                    LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public AsyncRelayCommand LoginCommand { get; } 

        private bool CanLogin() =>
            !IsBusy && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        private async Task ExecuteLoginAsync()
        {
            IsBusy = true;
            try
            {
                await Task.Delay(100); // Simulate async work

                // Navigate to home view after successful login
                _mainWindowService.NavigateToView(_homeView);

                // Initialize the HomeViewModel if needed
                if (_homeView.DataContext is HomeViewModel homeViewModel)
                {
                    await homeViewModel.InitializeAsync();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
