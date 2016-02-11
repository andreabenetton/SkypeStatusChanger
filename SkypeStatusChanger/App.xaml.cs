using System;
using System.Collections.Generic;
using System.Windows;
using SkypeStatusChanger.SkypeApi;
using SkypeStatusChanger.Views;

namespace SkypeStatusChanger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string UNIQUE_NAME = "{5DB91739-6B2D-4F6C-A2DD-DDFBE3A691E7}_SkypeStatusChanger";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(UNIQUE_NAME))
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }

        #endregion

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            if (!SkypeHelper.IsSkypeInstalled())
            {
                MessageBox.Show("Skype not installed on computer! App will close.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            ShutdownMode = ShutdownMode.OnMainWindowClose;
            MainWindow = new MainWindow();
            MainWindow.Show();
        }
    }
}
