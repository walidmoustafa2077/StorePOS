using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using StorePOS.Client.Wpf.MVVM.ViewModels;

namespace StorePOS.Client.Wpf.Services.Implementations
{
    public class MainWindowService : IMainWindowService
    {
        private object? _currentView;
        private object? _currentDialog;
        private bool _isBusy;

        public object? CurrentView
        {
            get => _currentView;
            private set
            {
                if (SetProperty(ref _currentView, value))
                {
                    OnPropertyChanged(nameof(IsDialogOpen));
                    SubscribeToViewModelPropertyChanges();
                }
            }
        }

        public object? CurrentDialog
        {
            get => _currentDialog;
            private set
            {
                if (SetProperty(ref _currentDialog, value))
                    OnPropertyChanged(nameof(IsDialogOpen));
            }
        }

        public bool IsDialogOpen => CurrentDialog != null;

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void NavigateToView(object view)
        {
            CurrentView = view;
        }

        public void ShowDialog(object dialog)
        {
            CurrentDialog = dialog;
        }

        public void CloseDialog()
        {
            CurrentDialog = null;
        }

        public void SetBusyState(bool isBusy)
        {
            IsBusy = isBusy;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void SubscribeToViewModelPropertyChanges()
        {
            if (CurrentView is UserControl userControl && userControl.DataContext is ViewModelBase viewModel)
            {
                // Unsubscribe from previous view model if any
                viewModel.PropertyChanged -= OnCurrentViewModelPropertyChanged;
                
                // Subscribe to new view model
                viewModel.PropertyChanged += OnCurrentViewModelPropertyChanged;
                
                // Update initial busy state
                IsBusy = viewModel.IsBusy;
            }
        }

        private void OnCurrentViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModelBase.IsBusy) && sender is ViewModelBase vm)
            {
                IsBusy = vm.IsBusy;
            }
        }
    }
}