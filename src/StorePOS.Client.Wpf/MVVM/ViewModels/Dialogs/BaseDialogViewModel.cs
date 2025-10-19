using System.Windows.Input;
using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs
{
    public class BaseDialogViewModel : ViewModelBase
    {
        private string _primaryButtonText = "OK";
        public string PrimaryButtonText
        {
            get => _primaryButtonText;
            set => SetProperty(ref _primaryButtonText, value);
        }

        private string? _secondaryButtonText;
        public string? SecondaryButtonText
        {
            get => _secondaryButtonText;
            set => SetProperty(ref _secondaryButtonText, value);
        }

        private object? _content;
        public object? Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        private ICommand? _primaryButtonCommand;
        public ICommand? PrimaryButtonCommand
        {
            get => _primaryButtonCommand;
            set => SetProperty(ref _primaryButtonCommand, value);
        }

        private ICommand? _secondaryButtonCommand;
        public ICommand? SecondaryButtonCommand
        {
            get => _secondaryButtonCommand;
            set => SetProperty(ref _secondaryButtonCommand, value);
        }

        // Predefined dialog properties
        private DialogType _dialogType = DialogType.Custom;
        public DialogType DialogType
        {
            get => _dialogType;
            set => SetProperty(ref _dialogType, value);
        }

        private string? _iconName;
        public string? IconName
        {
            get => _iconName;
            set => SetProperty(ref _iconName, value);
        }

        private string? _themeColor;
        public string? ThemeColor
        {
            get => _themeColor;
            set => SetProperty(ref _themeColor, value);
        }

        private bool _showCloseButton = true;
        public bool ShowCloseButton
        {
            get => _showCloseButton;
            set => SetProperty(ref _showCloseButton, value);
        }

        private int _autoCloseTimeoutMs = 0;
        public int AutoCloseTimeoutMs
        {
            get => _autoCloseTimeoutMs;
            set => SetProperty(ref _autoCloseTimeoutMs, value);
        }
    }
}