using StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels;
using System.Windows.Controls;

namespace StorePOS.Client.Wpf.MVVM.Views.HomeViews
{
    /// <summary>
    /// Interaction logic for SalesView.xaml
    /// </summary>
    public partial class SalesView : UserControl
    {
        public SalesView(SalesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
