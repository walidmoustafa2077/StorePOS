using StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels;
using System.Windows.Controls;

namespace StorePOS.Client.Wpf.MVVM.Views.HomeViews
{
    /// <summary>
    /// Interaction logic for InventoryView.xaml
    /// </summary>
    public partial class InventoryView : UserControl
    {
        public InventoryView(InventoryViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
