using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Sayonara;
using Simitone.Installer.Driver;
using Simitone.Installer.UI.Controls;

namespace Simitone.Installer.UI.Pages
{
    /// <summary>
    /// Interaction logic for TransferPageCore.xaml
    /// </summary>
    public partial class TransferPageCore : Page, INotifyPropertyChanged
    {
        public class TransferContext
        {
            public string TS1InstallationPath;
            public bool TS1Installed;
            public string IP;
            public bool IsServer;
        }

        public event EventHandler OnBack;
        Sayonara.SayonaraClient client;
        Sayonara.SayonaraServer server;
        private TransferContext _context;
        private bool isUIupdating = false;
        Task installationTask;
        private string _ip = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public string URL
        {
            get => _ip ?? ((Context?.IsServer ?? false) ? "Not Hosting" : "Not Connected");
            set
            {
                _ip = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("URL"));
            }
        }
        public TransferContext Context
        {
            get => _context;
            set
            {
                _context = value;
                OnContextRecieved();
            }
        }
        public string TS1Path
        {
            get => Context?.TS1InstallationPath ?? "No Path";
            set
            {
                Context.TS1InstallationPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TS1Path"));
            }
        }

        public TransferPageCore()
        {
            InitializeComponent();
        }

        private void OnContextRecieved()
        {
            ButtonStackPanel.IsEnabled = false;
            Sayonara.Out.DefaultOut.OnOutput += DefaultOut_OnOutput;
            var ip = Context.IP ?? "localhost";
            if (!Context.IsServer)
            {
                client = new Sayonara.SayonaraClient(ip);
                URL = ip;
            }
            else
            {
                ButtonStackPanel.IsEnabled = true;
                PauseButton.Visibility = Visibility.Collapsed; // server cannot be paused.                
                BackButton.IsEnabled = true;                
            }            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TS1Path"));
            if (!Context.TS1Installed)
            {
                Dialog.ShowDialog("The Sims Is Not Installed!", "The Installation cannot be transferred " +
                    "if it is not present on your computer. Please install The Sims and revisit this screen.");
                OnBack?.Invoke(this, null);
                return;
            }
            DirectoryChooser.SelectedDirectory = TS1Path;
            ButtonStackPanel.IsEnabled = true;
        }

        private void DefaultOut_OnOutput(Sayonara.Out.MODE outMode, Sayonara.Out.OnOutputEventArgs args)
        {
            if (outMode == Out.MODE.WRITELINE)
            {
                if (!isUIupdating)
                    ConsoleOut.Dispatcher.InvokeAsync(() =>
                    {
                        isUIupdating = true;
                        if (ConsoleOut != null)
                        {
                            ConsoleOut.Text += args.Text + "\n";
                            ConsoleViewer.ScrollToBottom();
                        }
                        isUIupdating = false;
                    });
            }
            else
            {
                if (client != null)
                    client.IsPaused = true;
                var dialog = Dialog.ShowDialog("Transfer The Sims", args.Text, (DialogPage.CMsgBoxButtons)((int)args.Buttons));
                dialog.DialogClosed += (DialogPage sender, bool ? result) => {
                    if (client != null)
                        client.IsPaused = false;
                };
            }

        }

        private void BeginButton_Click(object sender, RoutedEventArgs e)
        {            
            TS1Path = DirectoryChooser.SelectedDirectory;
            if (Context.IsServer)
            {
                server = SayonaraServer.HostLocalServer(TS1Path);
                if (!server.Hosting)
                    return;
                URL = new Uri(server.Address).Host;
                BeginButton.Visibility = Visibility.Collapsed;
                BackButton.Content = "Stop Server and Go Back";
                return;
            }
            if (installationTask == null)
            {
                try
                {
                    Stopwatch w = new Stopwatch();
                    w.Start();
                    installationTask = Task.Run(() => client.DownloadAsync(TS1Path), client.CancellationToken.Token).ContinueWith((Task task) =>
                    {
                        w.Stop();
                        Out.PrintLine("Transfer completed in " + w.Elapsed.ToString());
                        Dispatcher.Invoke(() => (sender as Button).Content = "Start");
                        installationTask = null;
                    });                    
                }
                catch (TaskCanceledException exec)
                {
                    client.DisposeCancelToken();
                }
                catch (Exception exception)
                {
                    Dialog.ShowDialog("A Error Occurred...",
                    "Try again in a little while.");
                    installationTask = null;
                }
                (sender as Button).Content = "Start";
            }
            else
            {
                RegisterSafeExit(() => installationTask = null);
                (sender as Button).Content = "Start";
            }
        }

        private void RegisterSafeExit(Action callback)
        {
            if (installationTask != null)
            {
                installationTask.ContinueWith((Task task) =>
                {
                    if (task.Status == TaskStatus.Canceled)
                        callback?.Invoke();
                });
                client.CancellationToken.Cancel(true);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (installationTask != null && !Context.IsServer)
            {
                client.IsPaused = true;
                var dialog = Dialog.ShowDialog("Transfer In Progress!",
                    "The installer is still transferring The Sims. Are you sure you" +
                    " want to cancel the installation?", DialogPage.CMsgBoxButtons.YesNo);
                dialog.DialogClosed += (DialogPage page, bool? result) =>
                {
                    if (result != null)
                    {
                        if (result == true)
                            RegisterSafeExit(() => OnBack?.Invoke(this, null));
                        else
                            client.IsPaused = false;
                    }
                };
                return;
            }
            else if (Context.IsServer)
            {
                server.Shutdown();
            }
            Out.DefaultOut.OnOutput -= DefaultOut_OnOutput;
            OnBack?.Invoke(this, null);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            client.IsPaused = !client.IsPaused;
            if (client.IsPaused)
            {
                (sender as Button).Content = "Resume";
            }
            else
                (sender as Button).Content = "Pause";
        }
    }
}
