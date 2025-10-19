namespace StorePOS.Client.Wpf.MVVM.ViewModels
{
    /// <summary>
    /// Interface for ViewModels that need to be notified when they are navigated to or from
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// Called when the view is navigated to
        /// </summary>
        Task OnNavigatedToAsync();

        /// <summary>
        /// Called when the view is navigated away from
        /// </summary>
        Task OnNavigatedFromAsync();
    }
}
