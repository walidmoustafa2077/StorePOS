using StorePOS.Client.Wpf.Services;
using StorePOS.Client.Wpf.MVVM.Views;
using System.ComponentModel;

namespace StorePOS.Client.Wpf.MVVM.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IMainWindowService _mainWindowService;

        public object? CurrentView => _mainWindowService.CurrentView;
        public object? CurrentDialog => _mainWindowService.CurrentDialog;
        public bool IsDialogOpen => _mainWindowService.IsDialogOpen;

        public MainWindowViewModel(IMainWindowService mainWindowService, LoginView loginView)
        {
            _mainWindowService = mainWindowService;
            
            // Subscribe to main window service property changes
            _mainWindowService.PropertyChanged += OnMainWindowServicePropertyChanged;
            
            // Start with login screen
            _mainWindowService.NavigateToView(loginView);
        }

        private void OnMainWindowServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IMainWindowService.CurrentView):
                    OnPropertyChanged(nameof(CurrentView));
                    break;
                case nameof(IMainWindowService.CurrentDialog):
                    OnPropertyChanged(nameof(CurrentDialog));
                    break;
                case nameof(IMainWindowService.IsDialogOpen):
                    OnPropertyChanged(nameof(IsDialogOpen));
                    break;
                case nameof(IMainWindowService.IsBusy):
                    IsBusy = _mainWindowService.IsBusy;
                    break;
            }
        }
    }
}
