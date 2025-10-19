namespace StorePOS.Client.Wpf.Services
{
    /// <summary>
    /// Service responsible for navigation between different views in the application.
    /// Provides methods to navigate to views and manage navigation state.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Gets the currently active view.
        /// </summary>
        object? CurrentView { get; }

        /// <summary>
        /// Event raised when navigation occurs.
        /// </summary>
        event EventHandler<NavigationEventArgs>? NavigationOccurred;

        /// <summary>
        /// Navigates to the specified view by name.
        /// </summary>
        /// <param name="viewName">The name of the view to navigate to.</param>
        /// <param name="parameters">Optional parameters to pass to the view.</param>
        /// <returns>A task representing the navigation operation.</returns>
        Task NavigateToAsync(string viewName, object? parameters = null);

        /// <summary>
        /// Checks if navigation to the specified view is possible.
        /// </summary>
        /// <param name="viewName">The name of the view to check.</param>
        /// <returns>True if navigation is possible, false otherwise.</returns>
        bool CanNavigateTo(string viewName);

        /// <summary>
        /// Gets the title for the specified view.
        /// </summary>
        /// <param name="viewName">The name of the view.</param>
        /// <returns>The title for the view.</returns>
        string GetViewTitle(string viewName);
    }
}