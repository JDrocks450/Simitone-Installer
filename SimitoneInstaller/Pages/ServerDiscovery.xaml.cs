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
    /// Interaction logic for ServerDiscovery.xaml
    /// </summary>
    public partial class ServerDiscovery : Page
    {
        public delegate void OptionSelectedHandler(object sender, string IP, bool IsServer);
        public event OptionSelectedHandler OptionSelectedEvent;
        public event EventHandler OnBack;
        private (string, string) targetAutoConnectServer = (null,null);

        private Sayonara.DiscoveryHandler discovery;

        public ServerDiscovery()
        {
            InitializeComponent();
            discovery = new Sayonara.DiscoveryHandler();
            discovery.BeginListening(ServerListUpdated);
        }

        private void ServerListUpdated(IEnumerable<(string Address, string Message)> servers)
        {
            Dispatcher.Invoke(() =>
            {
                if (servers.Count() > 0)
                {
                    AutoConnect.BorderBrush = Brushes.DeepSkyBlue;
                    AutoConnect.IsEnabled = true;
                    targetAutoConnectServer = servers.FirstOrDefault();
                }
                else
                {
                    AutoConnect.BorderBrush = Brushes.Gray;
                    AutoConnect.IsEnabled = false;
                }
            });
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            OptionSelectedEvent?.Invoke(this, null, true);
        }

        private void AutoConnect_Click(object sender, RoutedEventArgs e)
        {
            if (targetAutoConnectServer.Item1 != null)            
                OptionSelectedEvent?.Invoke(this, targetAutoConnectServer.Item1, false);            
        }

        private void DirectConnect_Click(object sender, RoutedEventArgs e)
        {
            OptionSelectedEvent?.Invoke(this, null, false);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            OnBack?.Invoke(this, null);
        }
    }
}
