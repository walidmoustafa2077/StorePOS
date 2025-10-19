namespace StorePOS.Client.Wpf.MVVM.ViewModels
{
    /// <summary>
    /// Interface for ViewModels that support refresh functionality.
    /// Implementing ViewModels should reload their data when RefreshAsync is called.
    /// </summary>
    public interface IRefreshable
    {
        /// <summary>
        /// Refreshes the ViewModel's data by reloading from the data source.
        /// </summary>
        /// <returns>A task that represents the asynchronous refresh operation.</returns>
        Task RefreshAsync();
    }
}
