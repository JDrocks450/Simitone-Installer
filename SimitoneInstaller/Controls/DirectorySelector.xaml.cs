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
    /// Interaction logic for DirectorySelector.xaml
    /// </summary>
    public partial class DirectorySelector : UserControl, INotifyPropertyChanged
    {
        private string _dir;

        public event PropertyChangedEventHandler PropertyChanged;

        public string SelectedDirectory
        {
            get => _dir;
            set
            {
                _dir = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedDirectory"));
            }
        }

        public DirectorySelector()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var folder = new System.Windows.Forms.FolderBrowserDialog();
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedDirectory = folder.SelectedPath;
            }            
        }
    }
}
