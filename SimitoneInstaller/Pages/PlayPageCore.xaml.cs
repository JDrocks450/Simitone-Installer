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
    /// Interaction logic for PlayPageCore.xaml
    /// </summary>
    public partial class PlayPageCore : Page
    {
        public event EventHandler OnPlay, OnForceUpdate, OnTransfer;
        public PlayPageCore()
        {
            InitializeComponent();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            OnPlay?.Invoke(this, null);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            OnForceUpdate?.Invoke(this, null);
        }

        private void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Driver.Properties.Constants.Default.GitRepoAddress);
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            OnTransfer?.Invoke(this, null);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
