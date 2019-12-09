using Simitone.Installer.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Simitone.Installer.UI.Pages
{
    /// <summary>
    /// Interaction logic for Installation.xaml
    /// </summary>
    public partial class Installation : Page
    {
        private Installer.Driver.InstallationManager InstallationManager;
        public Installation()
        {
            InitializeComponent();
            InstallationManager = new Installer.Driver.InstallationManager();
            Debug.WriteLine(string.Join(", ",Environment.GetCommandLineArgs()));
            var installDir = Environment.GetCommandLineArgs().Where(x => x.Contains("installTo")).Select(x => x.Substring(x.IndexOf(':') + 1)).FirstOrDefault();
            if (installDir != null)
                InstallationManager.InstallContext.SimitonePath = installDir;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.SimitoneInstalled)
            {
                NavigationService.Navigate(new PlayPage());
                return;
            }
            (InstallationPageFlyout.Content as SimitoneInstallationPage).Context = InstallationManager.InstallContext;
            (InstallationPageFlyout.Content as SimitoneInstallationPage).OnInstallationStart += Installation_OnInstallationStart;
            (InstallationPageFlyout.Content as SimitoneInstallationPage).OnSkip += Installation_OnSkip;
            while (InstallationManager.HasErrors)
            {
                var error = InstallationManager.InstallContext.PopWarning();
                Dialog.ShowDialog(error.Title, error.Text, DialogPage.CMsgBoxButtons.OK);
            }
        }

        private void Installation_OnSkip(object sender, EventArgs e)
        {
            SwitchToPlayScreen();
        }

        private void SwitchToPlayScreen()
        {
            InstallationPageFlyout.Close((object obj, EventArgs args) => NavigationService.Navigate(new PlayPage()));
        }

        private void Installation_OnInstallationStart(object sender, EventArgs e)
        {
            Task.Run(() => InstallationManager.BeginInstallation()).ContinueWith((Task<bool> previous) =>
            {
                try
                {
                    if (!previous.Result)
                        while (InstallationManager.HasErrors)
                        {
                            var error = InstallationManager.InstallContext.PopWarning();
                            Dialog.ShowDialog(error.Title, error.Text, DialogPage.CMsgBoxButtons.OK);
                        }
                    Dispatcher.Invoke(() =>
                        {
                            (InstallationPageFlyout.Content as Page).IsEnabled = true;
                            Properties.Settings.Default.SimitoneURI = InstallationManager.InstallContext.SimitonePath;
                            Properties.Settings.Default.Save();
                            if (previous.Result)
                            {
                                SwitchToPlayScreen();
                            }
                            MainWindow.HideDownload();
                        });
                }
                catch (Exception exception)
                {
                    Dispatcher.Invoke(() =>
                    {
                        (InstallationPageFlyout.Content as Page).IsEnabled = true;
                        exceptionDialog(exception);
                    });                    
                    return;
                }
            });                    
            MainWindow.ShowDownload(InstallationManager.CurrentStatus);
        }

        private void exceptionDialog(Exception e)
        {
            void showException(Exception exception)
            {
                if (exception is UnauthorizedAccessException)
                {
                    var diag = Dialog.ShowDialog("Cannot Install Simitone There!",
                        "The selected path to install Simitone to is not accessible. " +
                        "Would you like to restart the installer as an Administrator?", DialogPage.CMsgBoxButtons.YesNo);
                    diag.ResultYes += (DialogPage sender, bool? result) =>
                    {
                        var info = new ProcessStartInfo(Application.ResourceAssembly.Location, "-installTo:" + String.Format(@"""{0}""", InstallationManager.InstallContext.SimitonePath));
                        info.Verb = "runas";
                        Process.Start(info);
                        Application.Current.Shutdown();
                    };
                }
                else
                    throw exception;
            }
            if (e is AggregateException)
            {
                foreach (var exception in (e as AggregateException).InnerExceptions)
                    showException(exception);
            }
        }
    }
}
