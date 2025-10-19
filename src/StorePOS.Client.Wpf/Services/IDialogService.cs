using StorePOS.Client.Wpf.MVVM.ViewModels.Dialogs;
using StorePOS.Client.Wpf.MVVM.Models;

namespace StorePOS.Client.Wpf.Services
{
    /// <summary>
    /// Interface for managing dialog operations in the application.
    /// Provides methods to show and manage modal dialogs with various content types.
    /// Follows Interface Segregation Principle by focusing only on dialog-related operations.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Gets whether a dialog is currently open.
        /// </summary>
        bool IsDialogOpen { get; }

        /// <summary>
        /// Shows a dialog with the specified ViewModel.
        /// Use this method when you have a custom ViewModel that inherits from BaseDialogViewModel.
        /// </summary>
        /// <param name="viewModel">The ViewModel to show in the dialog</param>
        /// <returns>Task that completes when the dialog is closed with the dialog result</returns>
        /// <exception cref="ArgumentNullException">Thrown when viewModel is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when a dialog is already open</exception>
        Task<bool?> ShowDialogAsync(BaseDialogViewModel viewModel);

        /// <summary>
        /// Shows a dialog with the specified Model (creates ViewModel internally).
        /// Use this method for simple dialogs where you just need to display content.
        /// </summary>
        /// <param name="model">The Model containing dialog data</param>
        /// <returns>Task that completes when the dialog is closed with the dialog result</returns>
        /// <exception cref="ArgumentNullException">Thrown when model is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when a dialog is already open</exception>
        Task<bool?> ShowDialogAsync(DialogModel model);

        /// <summary>
        /// Shows a predefined dialog with the specified Model.
        /// Use this method for standard dialogs like confirmation, error, or information dialogs.
        /// </summary>
        /// <param name="model">The predefined dialog model containing dialog data and type</param>
        /// <returns>Task that completes when the dialog is closed with the dialog result</returns>
        /// <exception cref="ArgumentNullException">Thrown when model is null</exception>
        /// <exception cref="InvalidOperationException">Thrown when a dialog is already open</exception>
        Task<bool?> ShowDialogAsync(PredefinedDialogModel model);

        /// <summary>
        /// Closes the currently open dialog with the specified result.
        /// If no dialog is open, this method does nothing.
        /// </summary>
        /// <param name="result">The dialog result (true for OK/Yes, false for Cancel/No, null for neutral)</param>
        void CloseDialog(bool? result = null);
    }
}