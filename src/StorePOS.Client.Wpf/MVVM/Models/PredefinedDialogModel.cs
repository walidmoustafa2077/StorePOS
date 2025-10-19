namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Model for predefined dialogs with standardized configurations
    /// </summary>
    public class PredefinedDialogModel : DialogModel
    {
        /// <summary>
        /// The type of dialog which determines the default appearance and behavior
        /// </summary>
        public DialogType DialogType { get; set; } = DialogType.Custom;

        /// <summary>
        /// Icon to display in the dialog (if supported by the view)
        /// </summary>
        public string? IconName { get; set; }

        /// <summary>
        /// Color theme for the dialog (if supported by the view)
        /// </summary>
        public string? ThemeColor { get; set; }

        /// <summary>
        /// Whether to show the close button (X) in the dialog
        /// </summary>
        public bool ShowCloseButton { get; set; } = true;

        /// <summary>
        /// Auto-close the dialog after specified milliseconds (0 = no auto-close)
        /// </summary>
        public int AutoCloseTimeoutMs { get; set; } = 0; 

        /// <summary>
        /// Creates a new predefined dialog model with default settings for the specified type
        /// </summary>
        /// <param name="dialogType">The type of dialog to create</param>
        /// <param name="message">The message to display</param>
        /// <param name="title">Optional custom title (will use default if not provided)</param>
        /// <returns>Configured PredefinedDialogModel</returns>
        public static PredefinedDialogModel Create(DialogType dialogType, string message, string? title = null)
        {
            var model = new PredefinedDialogModel
            {
                DialogType = dialogType,
                Message = message
            };

            // Set defaults based on dialog type
            switch (dialogType)
            {
                case DialogType.Success:
                    model.Title = title ?? "Success";
                    model.IconName = "CheckCircle";
                    model.ThemeColor = "Green";
                    model.PrimaryButtonText = "OK";
                    break;

                case DialogType.Warning:
                    model.Title = title ?? "Warning";
                    model.IconName = "Warning";
                    model.ThemeColor = "Orange";
                    model.PrimaryButtonText = "OK";
                    break;

                case DialogType.Error:
                    model.Title = title ?? "Error";
                    model.IconName = "Error";
                    model.ThemeColor = "Red";
                    model.PrimaryButtonText = "OK";
                    break;

                case DialogType.Information:
                    model.Title = title ?? "Information";
                    model.IconName = "Info";
                    model.ThemeColor = "Blue";
                    model.PrimaryButtonText = "OK";
                    break;

                case DialogType.Confirmation:
                    model.Title = title ?? "Confirm";
                    model.IconName = "Question";
                    model.ThemeColor = "Blue";
                    model.PrimaryButtonText = "Yes";
                    model.SecondaryButtonText = "No";
                    break;

                case DialogType.Retry:
                    model.Title = title ?? "Retry Operation";
                    model.IconName = "Refresh";
                    model.ThemeColor = "Orange";
                    model.PrimaryButtonText = "Retry";
                    model.SecondaryButtonText = "Cancel";
                    break;

                case DialogType.Question:
                    model.Title = title ?? "Question";
                    model.IconName = "Question";
                    model.ThemeColor = "Blue";
                    model.PrimaryButtonText = "OK";
                    model.SecondaryButtonText = "Cancel";
                    break;

                case DialogType.Custom:
                default:
                    model.Title = title ?? "Dialog";
                    model.PrimaryButtonText = "OK";
                    break;
            }

            return model;
        }
    }
}