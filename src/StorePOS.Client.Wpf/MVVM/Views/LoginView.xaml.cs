using System.Windows.Controls;
using StorePOS.Client.Wpf.MVVM.ViewModels;

namespace StorePOS.Client.Wpf.MVVM.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
