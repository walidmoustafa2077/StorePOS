using StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels;
using System.Windows.Controls;

namespace StorePOS.Client.Wpf.MVVM.Views.HomeViews
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
