namespace StorePOS.Client.Wpf.Services
{
    /// <summary>
    /// Event arguments for navigation events.
    /// </summary>
    public class NavigationEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the name of the view being navigated to.
        /// </summary>
        public string ViewName { get; }

        /// <summary>
        /// Gets the view instance.
        /// </summary>
        public object View { get; }

        /// <summary>
        /// Gets the optional parameters passed during navigation.
        /// </summary>
        public object? Parameters { get; }

        /// <summary>
        /// Initializes a new instance of the NavigationEventArgs class.
        /// </summary>
        /// <param name="viewName">The name of the view.</param>
        /// <param name="view">The view instance.</param>
        /// <param name="parameters">Optional parameters.</param>
        public NavigationEventArgs(string viewName, object view, object? parameters = null)
        {
            ViewName = viewName ?? throw new ArgumentNullException(nameof(viewName));
            View = view ?? throw new ArgumentNullException(nameof(view));
            Parameters = parameters;
        }
    }
}