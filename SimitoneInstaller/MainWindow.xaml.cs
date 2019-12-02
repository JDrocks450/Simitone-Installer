using Simitone.Installer.Driver;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Simitone.Installer.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double downloadOpenHeight = 20;

        public AsyncStatus CurrentDownload;
        public static MainWindow InstallerWindow;
        public static bool DialogOpened;
        public MainWindow()
        {
            InitializeComponent();
            InstallerWindow = this;
        }

        public static void ShowDialog(Dialog dialog)
        {
            InstallerWindow.DialogLayer.Children.Add(dialog);
            DialogOpened = true;
            Panel.SetZIndex(InstallerWindow.BackgroundBlocker, 1);
        }

        public static void HideDialog()
        {
            InstallerWindow.DialogLayer.Children.RemoveAt(InstallerWindow.DialogLayer.Children.Count-1);
            DialogOpened = false;
            Panel.SetZIndex(InstallerWindow.BackgroundBlocker, 0);
        }

        private void openDownloads()
        {
            Grid.SetRow(DownloadManager, 0);
            var anim = new DoubleAnimation(40, TimeSpan.FromSeconds(.5)) {
                AccelerationRatio = .5,
                DecelerationRatio = .5
            };
            anim.Completed += (object s, EventArgs e) => {  };
            DownloadManager.BeginAnimation(HeightProperty, anim);
            DownloadManager.ViewingStatus = CurrentDownload;
        }

        private void closeDownloads()
        {
            var anim = new DoubleAnimation(0, TimeSpan.FromSeconds(.5))
            {
                AccelerationRatio = .5,
                DecelerationRatio = .5
            };
            anim.Completed += (object s, EventArgs e) => { Grid.SetRow(DownloadManager, 1); };
            DownloadManager.BeginAnimation(HeightProperty, anim);
        }

        public static void HideDownload()
        {
            InstallerWindow.closeDownloads();
        }

        public static void ShowDownload(AsyncStatus installation)
        {
            MainWindow.InstallerWindow.CurrentDownload = installation;            
            MainWindow.InstallerWindow.openDownloads();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
