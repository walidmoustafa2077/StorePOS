using System.ComponentModel;

namespace StorePOS.Client.Wpf.Services
{
    public interface IMainWindowService
    {
        /// <summary>
        /// Gets the current view displayed in the main window
        /// </summary>
        object? CurrentView { get; }

        /// <summary>
        /// Gets the current dialog displayed in the main window
        /// </summary>
        object? CurrentDialog { get; }

        /// <summary>
        /// Gets a value indicating whether a dialog is currently open
        /// </summary>
        bool IsDialogOpen { get; }

        /// <summary>
        /// Gets a value indicating whether the main window is busy
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Event raised when a property changes
        /// </summary>
        event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Navigates to the specified view
        /// </summary>
        /// <param name="view">The view to navigate to</param>
        void NavigateToView(object view);

        /// <summary>
        /// Shows the specified dialog
        /// </summary>
        /// <param name="dialog">The dialog to show</param>
        void ShowDialog(object dialog);

        /// <summary>
        /// Closes the current dialog
        /// </summary>
        void CloseDialog();

        /// <summary>
        /// Sets the busy state of the main window
        /// </summary>
        /// <param name="isBusy">True to set busy state, false otherwise</param>
        void SetBusyState(bool isBusy);
    }
}