using StorePOS.Client.Wpf.MVVM.Models;
using StorePOS.Client.Wpf.Services;

namespace StorePOS.Client.Wpf.Extensions
{
    /// <summary>
    /// Extension methods for IDialogService to provide convenient predefined dialog methods
    /// </summary>
    public static class DialogServiceExtensions
    {
        /// <summary>
        /// Shows a success dialog with the specified message
        /// </summary>
        /// <param name="dialogService">The dialog service instance</param>
        /// <param name="message">The success message to display</param>
        /// <param name="title">Optional custom title (defaults to "Success")</param>
        /// <param name="autoCloseTimeoutMs">Auto-close timeout in milliseconds (0 = no auto-close)</param>
        /// <returns>Task that completes when the dialog is closed</returns>
        public static async Task<bool?> ShowSuccessAsync(this IDialogService dialogService, 
            string message, string? title = null, int autoCloseTimeoutMs = 0)
        {
            var model = PredefinedDialogModel.Create(DialogType.Success, message, title);
            model.AutoCloseTimeoutMs = autoCloseTimeoutMs;
            return await dialogService.ShowDialogAsync(model);
        }

        /// <summary>
        /// Shows a warning dialog with the specified message
        /// </summary>
        /// <param name="dialogService">The dialog service instance</param>
        /// <param name="message">The warning message to display</param>
        /// <param name="title">Optional custom title (defaults to "Warning")</param>
        /// <returns>Task that completes when the dialog is closed</returns>
        public static async Task<bool?> ShowWarningAsync(this IDialogService dialogService, 
            string message, string? title = null)
        {
            var model = PredefinedDialogModel.Create(DialogType.Warning, message, title);
            return await dialogService.ShowDialogAsync(model);
        }

        /// <summary>
        /// Shows an error dialog with the specified message
        /// </summary>
        /// <param name="dialogService">The dialog service instance</param>
        /// <param name="message">The error message to display</param>
        /// <param name="title">Optional custom title (defaults to "Error")</param>
        /// <returns>Task that completes when the dialog is closed</returns>
        public static async Task<bool?> ShowErrorAsync(this IDialogService dialogService, 
            string message, string? title = null)
        {
            var model = PredefinedDialogModel.Create(DialogType.Error, message, title);
            return await dialogService.ShowDialogAsync(model);
        }

        /// <summary>
        /// Shows an information dialog with the specified message
        /// </summary>
        /// <param name="dialogService">The dialog service instance</param>
        /// <param name="message">The information message to display</param>
        /// <param name="title">Optional custom title (defaults to "Information")</param>
        /// <returns>Task that completes when the dialog is closed</returns>
        public static async Task<bool?> ShowInformationAsync(this IDialogService dialogService, 
            string message, string? title = null)
        {
            var model = PredefinedDialogModel.Create(DialogType.Information, message, title);
            return await dialogService.ShowDialogAsync(model);
        }

        /// <summary>
        /// Shows a confirmation dialog with Yes/No buttons
        /// </summary>
        /// <param name="dialogService">The dialog service instance</param>
        /// <param name="message">The confirmation message to display</param>
        /// <param name="title">Optional custom title (defaults to "Confirm")</param>
        /// <returns>Task that completes with true if Yes was clicked, false if No was clicked</returns>
        public static async Task<bool?> ShowConfirmationAsync(this IDialogService dialogService, 
            string message, string? title = null)
        {
            var model = PredefinedDialogModel.Create(DialogType.Confirmation, message, title);
            return await dialogService.ShowDialogAsync(model);
        }

        /// <summary>
        /// Shows a retry dialog with Retry/Cancel buttons
        /// </summary>
        /// <param name="dialogService">The dialog service instance</param>
        /// <param name="message">The retry message to display</param>
        /// <param name="title">Optional custom title (defaults to "Retry Operation")</param>
        /// <returns>Task that completes with true if Retry was clicked, false if Cancel was clicked</returns>
        public static async Task<bool?> ShowRetryAsync(this IDialogService dialogService, 
            string message, string? title = null)
        {
            var model = PredefinedDialogModel.Create(DialogType.Retry, message, title);
            return await dialogService.ShowDialogAsync(model);
        }

        /// <summary>
        /// Shows a question dialog with OK/Cancel buttons
        /// </summary>
        /// <param name="dialogService">The dialog service instance</param>
        /// <param name="message">The question to display</param>
        /// <param name="title">Optional custom title (defaults to "Question")</param>
        /// <returns>Task that completes with true if OK was clicked, false if Cancel was clicked</returns>
        public static async Task<bool?> ShowQuestionAsync(this IDialogService dialogService, 
            string message, string? title = null)
        {
            var model = PredefinedDialogModel.Create(DialogType.Question, message, title);
            return await dialogService.ShowDialogAsync(model);
        }

        /// <summary>
        /// Shows a custom predefined dialog
        /// </summary>
        /// <param name="dialogService">The dialog service instance</param>
        /// <param name="dialogType">The type of dialog to show</param>
        /// <param name="message">The message to display</param>
        /// <param name="title">Optional custom title</param>
        /// <param name="primaryButtonText">Optional custom primary button text</param>
        /// <param name="secondaryButtonText">Optional custom secondary button text</param>
        /// <returns>Task that completes when the dialog is closed</returns>
        public static async Task<bool?> ShowPredefinedAsync(this IDialogService dialogService,
            DialogType dialogType, string message, string? title = null,
            string? primaryButtonText = null, string? secondaryButtonText = null)
        {
            var model = PredefinedDialogModel.Create(dialogType, message, title);
            
            if (!string.IsNullOrEmpty(primaryButtonText))
                model.PrimaryButtonText = primaryButtonText;
                
            if (!string.IsNullOrEmpty(secondaryButtonText))
                model.SecondaryButtonText = secondaryButtonText;

            return await dialogService.ShowDialogAsync(model);
        }
    }
}