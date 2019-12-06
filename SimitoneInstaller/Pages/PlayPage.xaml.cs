using Simitone.Installer.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for PlayPage.xaml
    /// </summary>
    public partial class PlayPage : Page, INotifyPropertyChanged
    {
        private bool _3d;
        private bool _ide;
        private string _api;
        private bool _minimize;

        public bool MinimizeOnPlay
        {
            get => _minimize;
            set
            {
                _minimize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MinimizeOnPlay"));
            }
        }
        public bool Using3D
        {
            get => _3d;
            set
            {
                _3d = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Using3D"));
            }
        }
        public bool IDE
        {
            get => _ide;
            set
            {
                _ide = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IDE"));
            }
        }
        public string API
        {
            get => _api;
            set
            {
                _api = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("API"));
            }
        }

        public PlayPage()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Using3D = Properties.Settings.Default.Using3D;
            IDE = Properties.Settings.Default.IDEMode;
            API = Properties.Settings.Default.API;
            MinimizeOnPlay = Properties.Settings.Default.MinimizeOnPlay;

            Properties.Settings.Default.SimitoneInstalled = true;
            Properties.Settings.Default.Save();
            MainWindow.InstallerWindow.Title = "Simitone Launcher";
            (PlayPageFlyout.Content as PlayPageCore).OnPlay += PlayPage_OnPlay;
            (PlayPageFlyout.Content as PlayPageCore).OnForceUpdate += PlayPage_OnForceUpdate;
            (PlayPageFlyout.Content as PlayPageCore).OnTransfer += PlayPage_OnTransfer; ;
        }

        private void PlayPage_OnTransfer(object sender, EventArgs e)
        {
            PlayPageFlyout.Close((object s, EventArgs args) =>
            {
                NavigationService.Navigate(new TransferScreen());
            });
        }

        private void PlayPage_OnForceUpdate(object sender, EventArgs e)
        {
            PlayPageFlyout.Close((object s, EventArgs args) => {
                Properties.Settings.Default.SimitoneInstalled = false;
                NavigationService.Navigate(new Installation());
            });
        }

        private void PlayPage_OnPlay(object sender, EventArgs e)
        {
            void showErrorDiag()
            {
                Dialog.ShowDialog("Unable To Start Simitone!",
                    "It doesn't seem like you've installed Simitone using this installer " +
                    "before. Clicking Force Update will allow you to install Simitone again.");

            }
            var simitonePath = System.IO.Path.Combine(Properties.Settings.Default.SimitoneURI, "Simitone.Windows.exe");
            if (string.IsNullOrWhiteSpace(API))
                API = "DirectX";
            Properties.Settings.Default.Using3D = Using3D;
            Properties.Settings.Default.IDEMode = IDE;
            Properties.Settings.Default.API = API;
            Properties.Settings.Default.MinimizeOnPlay = MinimizeOnPlay;
            Properties.Settings.Default.Save();

            if (string.IsNullOrWhiteSpace(simitonePath))
            {
                showErrorDiag();
            }

            string verbs = (Using3D ? "-3d " : "") +
                (IDE ? "-ide " : "") +
                (API == "OpenGL" ? "-gl " : "");
            ProcessStartInfo info = new ProcessStartInfo(
            simitonePath, (verbs != "") ? verbs : null);
            info.WorkingDirectory = Properties.Settings.Default.SimitoneURI;
                
            var proc = Process.Start(info);
            if (proc != null && MinimizeOnPlay)
            {
                proc.Exited += Proc_Exited;
                MainWindow.InstallerWindow.WindowState = WindowState.Minimized;
            }
            else
                showErrorDiag();
        }

        private void Proc_Exited(object sender, EventArgs e)
        {
            var proc = sender as Process;
            proc.Dispose();
            MainWindow.InstallerWindow.WindowState = WindowState.Normal;
        }
    }
}
