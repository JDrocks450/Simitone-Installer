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
    /// Interaction logic for TransferScreen.xaml
    /// </summary>
    public partial class TransferScreen : Page
    {
        private TransferPageCore.TransferContext context;
        public TransferScreen()
        {
            InitializeComponent();
            var manager = new Driver.InstallationManager();
            context = new TransferPageCore.TransferContext()
            {
                TS1InstallationPath = manager.Context.TS1InstallationPath,
                TS1Installed = manager.Context.TS1Installed,
            };
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            HostGrid.Children.Remove(TransferPageFlyout);            
            (TransferPageFlyout.Content as TransferPageCore).OnBack += TransferScreen_OnBack;

            (DiscoveryFlyout.Content as ServerDiscovery).OnBack += TransferScreen_OnBack;
            (DiscoveryFlyout.Content as ServerDiscovery).OptionSelectedEvent += TransferScreen_OptionSelectedEvent;            
        }

        private void TransferScreen_OptionSelectedEvent(object sender, string IP, bool IsServer)
        {
            context.IP = IP;
            context.IsServer = IsServer;
            DiscoveryFlyout.Close((object s, EventArgs e) =>
            {
                HostGrid.Children.Remove(DiscoveryFlyout);
                HostGrid.Children.Add(TransferPageFlyout);
                (TransferPageFlyout.Content as TransferPageCore).Context = context;
            });
        }

        private void TransferScreen_OnBack(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                NavigationService.Navigate(new PlayPage());
            });
        }
    }
}
