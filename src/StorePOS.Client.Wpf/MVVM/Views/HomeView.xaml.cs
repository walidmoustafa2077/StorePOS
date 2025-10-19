using StorePOS.Client.Wpf.MVVM.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace StorePOS.Client.Wpf.MVVM.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private bool _isMenuExpanded = true;

        public HomeView(HomeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Loaded += HomeView_Loaded;
        }

        private async void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize the HomeViewModel when the view is loaded
            if (DataContext is HomeViewModel homeViewModel)
            {
                await homeViewModel.InitializeAsync();
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenu();
        }

        private void ToggleMenu()
        {
            Storyboard storyboard;

            if (_isMenuExpanded)
            {
                // Collapse the menu
                storyboard = (Storyboard)FindResource("CollapseMenuStoryboard");
                _isMenuExpanded = false;
            }
            else
            {
                // Expand the menu
                storyboard = (Storyboard)FindResource("ExpandMenuStoryboard");
                _isMenuExpanded = true;
            }

            storyboard?.Begin();
        }
    }
}
