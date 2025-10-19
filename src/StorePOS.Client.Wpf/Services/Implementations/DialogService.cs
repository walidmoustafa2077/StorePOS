using StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs;
using StorePOS.Client.Wpf.MVVM.Models;
using StorePOS.Client.Wpf.MVVM.Views.Dialogs;
using StorePOS.Client.Wpf.Commands;
using System.Windows.Controls;

namespace StorePOS.Client.Wpf.Services.Implementations
{
    public class DialogService : IDialogService
    {
        private readonly IMainWindowService _mainWindowService;
        private TaskCompletionSource<bool?>? _dialogTaskSource;

        public DialogService(IMainWindowService mainWindowService)
        {
            _mainWindowService = mainWindowService;
        }

        public bool IsDialogOpen => _mainWindowService.IsDialogOpen;

        public async Task<bool?> ShowDialogAsync(BaseDialogViewModel viewModel)
        {
            if (IsDialogOpen)
            {
                throw new InvalidOperationException("A dialog is already open.");
            }

            _dialogTaskSource = new TaskCompletionSource<bool?>();
            
            // Create the appropriate dialog view based on the ViewModel type
            UserControl dialogView = CreateDialogView(viewModel);
            dialogView.DataContext = viewModel;

            _mainWindowService.SetBusyState(false);
            _mainWindowService.ShowDialog(dialogView);
            
            return await _dialogTaskSource.Task;
        }

        /// <summary>
        /// Creates the appropriate dialog view based on the ViewModel type
        /// </summary>
        private UserControl CreateDialogView(BaseDialogViewModel viewModel)
        {
            return viewModel switch
            {
                StartShiftDialogViewModel => new StartShiftDialogView(),
                CashOperationDialogViewModel => new CashOperationDialogView(),
                RefundDialogViewModel => new RefundDialogView(),
                CustomContentDialogViewModel => new BaseDialogView(), // or create CustomContentDialogView if needed
                _ => new BaseDialogView() // Default to BaseDialogView for base ViewModels
            };
        }

        public async Task<bool?> ShowDialogAsync(DialogModel model)
        {
            var viewModel = CreateViewModelFromModel(model);
            return await ShowDialogAsync(viewModel);
        }

        public async Task<bool?> ShowDialogAsync(PredefinedDialogModel model)
        {
            var viewModel = CreateViewModelFromPredefinedModel(model);
            return await ShowDialogAsync(viewModel);
        }
        public void CloseDialog(bool? result = null)
        {
            if (IsDialogOpen)
            {
                _dialogTaskSource?.SetResult(result);
                _mainWindowService.CloseDialog();
                _dialogTaskSource = null;
            }
        }

        private BaseDialogViewModel CreateViewModelFromModel(DialogModel model)
        {
            var viewModel = new BaseDialogViewModel
            {
                Title = model.Title,
                Content = model.Message,
                PrimaryButtonText = model.PrimaryButtonText,
                SecondaryButtonText = model.SecondaryButtonText
            };

            // Setup custom actions if provided
            if (model.PrimaryButtonAction != null)
            {
                viewModel.PrimaryButtonCommand = new RelayCommand(() =>
                {
                    model.PrimaryButtonAction?.Invoke();
                    CloseDialog(true);
                });
            }
            else
            {
                // Default action for primary button
                viewModel.PrimaryButtonCommand = new RelayCommand(() => CloseDialog(true));
            }

            if (model.SecondaryButtonAction != null)
            {
                viewModel.SecondaryButtonCommand = new RelayCommand(() =>
                {
                    model.SecondaryButtonAction?.Invoke();
                    CloseDialog(false);
                });
            }
            else if (!string.IsNullOrEmpty(model.SecondaryButtonText))
            {
                // Default action for secondary button
                viewModel.SecondaryButtonCommand = new RelayCommand(() => CloseDialog(false));
            }

            return viewModel;
        }

        private BaseDialogViewModel CreateViewModelFromPredefinedModel(PredefinedDialogModel model)
        {
            var viewModel = new BaseDialogViewModel
            {
                Title = model.Title,
                Content = model.Message,
                PrimaryButtonText = model.PrimaryButtonText,
                SecondaryButtonText = model.SecondaryButtonText
            };

            // Add predefined dialog specific properties
            // These could be used by the view to style the dialog appropriately
            viewModel.DialogType = model.DialogType;
            viewModel.IconName = model.IconName;
            viewModel.ThemeColor = model.ThemeColor;
            viewModel.ShowCloseButton = model.ShowCloseButton;
            viewModel.AutoCloseTimeoutMs = model.AutoCloseTimeoutMs;

            // Setup custom actions if provided
            if (model.PrimaryButtonAction != null)
            {
                viewModel.PrimaryButtonCommand = new RelayCommand(() =>
                {
                    model.PrimaryButtonAction?.Invoke();
                    CloseDialog(true);
                });
            }
            else
            {
                // Default action for primary button
                viewModel.PrimaryButtonCommand = new RelayCommand(() => CloseDialog(true));
            }

            if (model.SecondaryButtonAction != null)
            {
                viewModel.SecondaryButtonCommand = new RelayCommand(() =>
                {
                    model.SecondaryButtonAction?.Invoke();
                    CloseDialog(false);
                });
            }
            else if (!string.IsNullOrEmpty(model.SecondaryButtonText))
            {
                // Default action for secondary button
                viewModel.SecondaryButtonCommand = new RelayCommand(() => CloseDialog(false));
            }

            return viewModel;
        }
    }

}