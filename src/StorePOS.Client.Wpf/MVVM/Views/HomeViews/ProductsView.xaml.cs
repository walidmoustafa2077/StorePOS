using StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels;
using System.Windows.Controls;

namespace StorePOS.Client.Wpf.MVVM.Views.HomeViews
{
    /// <summary>
    /// Interaction logic for ProductsView.xaml
    /// </summary>
    public partial class ProductsView : UserControl
    {
        public ProductsView(ProductsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}