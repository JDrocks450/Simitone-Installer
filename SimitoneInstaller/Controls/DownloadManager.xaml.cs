using Simitone.Installer.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Simitone.Installer.UI.Controls
{
    /// <summary>
    /// Interaction logic for DownloadManager.xaml
    /// </summary>
    public partial class DownloadManager : UserControl, INotifyPropertyChanged
    {
        private AsyncStatus _status;

        private AsyncStatus viewingStatus
        {
            get => _status;
            set
            {
                _status = value;
                ViewingStatus_DataChanged(_status, null);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string StatusText
        {
            get => ViewingStatus?.Status ?? "No Active Downloads...";
        }

        public int Value
        {
            get => (int)(ViewingStatus?.Percent ?? 0 * 100);
        }
        public AsyncStatus ViewingStatus { get => viewingStatus; set { viewingStatus = value; viewingStatus.DataChanged += ViewingStatus_DataChanged; } }

        private void ViewingStatus_DataChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                StatusBlock.Text = ViewingStatus.Status;
                ProgressBar.Value = Value;
            });
        }

        public DownloadManager()
        {
            InitializeComponent();
        }
    }
}
