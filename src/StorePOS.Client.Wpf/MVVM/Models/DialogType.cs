namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Enumeration for predefined dialog types
    /// </summary>
    public enum DialogType
    {
        /// <summary>
        /// Success message dialog
        /// </summary>
        Success,

        /// <summary>
        /// Warning message dialog
        /// </summary>
        Warning,

        /// <summary>
        /// Error message dialog
        /// </summary>
        Error,

        /// <summary>
        /// Information message dialog
        /// </summary>
        Information,

        /// <summary>
        /// Confirmation dialog with Yes/No options
        /// </summary>
        Confirmation,

        /// <summary>
        /// Retry dialog with Retry/Cancel options
        /// </summary>
        Retry,

        /// <summary>
        /// Question dialog
        /// </summary>
        Question,

        /// <summary>
        /// Custom dialog (default behavior)
        /// </summary>
        Custom
    }
}