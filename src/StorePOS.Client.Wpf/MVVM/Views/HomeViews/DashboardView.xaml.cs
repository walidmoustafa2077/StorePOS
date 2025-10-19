using StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels;
using System.Windows.Controls;

namespace StorePOS.Client.Wpf.MVVM.Views.HomeViews
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView(DashboardViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}