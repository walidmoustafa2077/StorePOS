using StorePOS.Client.Wpf.Commands;
using StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs
{
    /// <summary>
    /// Wrapper ViewModel for custom dialog content
    /// </summary>
    public class CustomContentDialogViewModel : BaseDialogViewModel
    {
        private object? _dialogContent;
        private bool _dialogResult;

        public object? DialogContent
        {
            get => _dialogContent;
            set => SetProperty(ref _dialogContent, value);
        }

        public bool DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        public CustomContentDialogViewModel(object content, string title = "")
        {
            DialogContent = content;
            Title = string.Empty; // Clear title so BaseDialogView doesn't show it (custom content has its own)
            Content = content; // Set this for the ContentPresenter in BaseDialogView
            
            // Hide default buttons since custom content handles its own buttons
            PrimaryButtonText = string.Empty;
            SecondaryButtonText = string.Empty;
            ShowCloseButton = true;
        }
    }
}