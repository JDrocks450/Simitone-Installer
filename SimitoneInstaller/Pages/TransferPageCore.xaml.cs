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
        public event EventHandler OnBack;
        Sayonara.SayonaraClient client;
        private InstallationContext _context;
        private bool isUIupdating = false;
        Task installationTask;

        public event PropertyChangedEventHandler PropertyChanged;

        public string URL
        {
            get => client?.ConnectedURL ?? "Not Hosting";
            set { }
        }
        public Driver.InstallationContext Context
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
            Sayonara.Out.DefaultOut.OnOutput += DefaultOut_OnOutput;
            client = new Sayonara.SayonaraClient("http://localhost:37575");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("URL"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TS1Path"));
            if (!Context.TS1Installed)
            {
                Dialog.ShowDialog("The Sims Is Not Installed!", "The Installation cannot be transferred " +
                    "if it is not present on your computer. Please install The Sims and revisit this screen.");
                OnBack?.Invoke(this, null);
                return;
            }
            DirectoryChooser.SelectedDirectory = TS1Path;            
        }

        private void DefaultOut_OnOutput(Sayonara.Out.MODE outMode, Sayonara.Out.OnOutputEventArgs args)
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

        private void BeginButton_Click(object sender, RoutedEventArgs e)
        {
            TS1Path = DirectoryChooser.SelectedDirectory;
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
                        Dispatcher.Invoke(() => (sender as Button).Content = "Begin");
                        
                    });
                }
                catch (TaskCanceledException exec)
                {
                    client.DisposeCancelToken();
                }
                catch (Exception exception)
                {
                    Dialog.ShowDialog("An Error Occurred...",
                    "The follow error occurred: " + exception);
                    (sender as Button).Content = "Begin";
                    installationTask = null;
                }
                (sender as Button).Content = "Stop";
            }
            else
            {
                RegisterSafeExit(() => installationTask = null);
                (sender as Button).Content = "Begin";
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
            if (installationTask != null)
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
            }
            else
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
