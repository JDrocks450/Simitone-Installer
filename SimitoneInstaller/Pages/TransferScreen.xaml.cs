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
        Installer.Driver.InstallationManager Manager;
        Sayonara.SayonaraClient Client;
        public TransferScreen()
        {
            InitializeComponent();
            Manager = new Driver.InstallationManager();            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (TransferPageFlyout.Content as TransferPageCore).Context = Manager.Context;
            (TransferPageFlyout.Content as TransferPageCore).OnBack += TransferScreen_OnBack; ;
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
