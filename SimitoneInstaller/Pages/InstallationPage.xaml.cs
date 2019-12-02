using Simitone.Installer.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.SimitoneInstalled)
            {
                NavigationService.Navigate(new PlayPage());
                return;
            }
            (InstallationPageFlyout.Content as SimitoneInstallationPage).Context = InstallationManager.Context;
            (InstallationPageFlyout.Content as SimitoneInstallationPage).OnInstallationStart += Installation_OnInstallationStart;
            (InstallationPageFlyout.Content as SimitoneInstallationPage).OnSkip += Installation_OnSkip;            
            while (InstallationManager.HasErrors)
            {
                var error = InstallationManager.Context.PopWarning();
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
                if (!previous.Result)
                    while (InstallationManager.HasErrors)
                    {
                        var error = InstallationManager.Context.PopWarning();
                        Dialog.ShowDialog(error.Title, error.Text, DialogPage.CMsgBoxButtons.OK);
                    }                
                Dispatcher.Invoke(() =>
                {
                    (InstallationPageFlyout.Content as Page).IsEnabled = true;
                    Properties.Settings.Default.SimitoneURI = InstallationManager.Context.SimitonePath;
                    Properties.Settings.Default.Save();
                    if (previous.Result)
                    {
                        SwitchToPlayScreen();
                    }
                    MainWindow.HideDownload();
                });
            });
            MainWindow.ShowDownload(InstallationManager.CurrentStatus);
        }
    }
}
