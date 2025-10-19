using System.Windows;
using StorePOS.Client.Wpf.MVVM.ViewModels;

namespace StorePOS.Client.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}