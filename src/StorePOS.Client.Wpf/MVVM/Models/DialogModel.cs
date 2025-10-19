namespace StorePOS.Client.Wpf.MVVM.Models
{
    public class DialogModel
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public string PrimaryButtonText { get; set; } = "OK";
        public string? SecondaryButtonText { get; set; } = null;

        public Action? PrimaryButtonAction { get; set; } = null;
        public Action? SecondaryButtonAction { get; set; } = null;
    }
}
