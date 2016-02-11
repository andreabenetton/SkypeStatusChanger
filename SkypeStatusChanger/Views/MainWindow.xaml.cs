using System.ComponentModel;
using System.Windows;
using SkypeStatusChanger.ViewModels;

namespace SkypeStatusChanger.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const double EXPECTED_TASKBAR_SIZE = 40;  // todo: calculate in runtime
        private const double POSITION_MARGIN = 5;
        private bool _closedFromTrayMenu;
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            InitLocation();

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }

        private void InitLocation()
        {
            // place window near tray
            Left = SystemParameters.PrimaryScreenWidth - Width - POSITION_MARGIN;
            Top = SystemParameters.PrimaryScreenHeight - Height - EXPECTED_TASKBAR_SIZE - POSITION_MARGIN;
        }

        private void MenuExitClick(object sender, RoutedEventArgs e)
        {
            _closedFromTrayMenu = true;
            Close();
        }

        private void MenuConfigureClick(object sender, RoutedEventArgs e)
        {
            Show();
            Activate();
        }

        private void MenuAboutClick(object sender, RoutedEventArgs e)
        {
            new About().Show();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (!_closedFromTrayMenu)
            {
                e.Cancel = true;
                Hide();
                return;
            }

            // unsubscribe from system events
            _viewModel.OsEventsWatcher.Dispose();
        }
    }
}