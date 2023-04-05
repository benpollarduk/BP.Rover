using System.Windows;
using BP.Rover.UI.ViewModels;
using BP.Rover.UI.Views;

namespace BP.Rover.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainView { ViewModel = new MainViewModel() };
            MainWindow.Show();
        }
    }
}
