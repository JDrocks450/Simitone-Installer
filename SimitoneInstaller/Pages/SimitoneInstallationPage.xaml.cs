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
using Simitone.Installer.Driver;

namespace Simitone.Installer.UI.Pages
{
    /// <summary>
    /// Interaction logic for SimitoneInstallationPage.xaml
    /// </summary>
    public partial class SimitoneInstallationPage : Page, INotifyPropertyChanged
    {
        private InstallationContext _context;

        public event EventHandler OnInstallationStart, OnSkip;
        public event PropertyChangedEventHandler PropertyChanged;

        public Installer.Driver.InstallationContext Context
        {
            get => _context;
            set
            {
                _context = value;
                if (IsLoaded)
                    updateUI();
            }
        }

        public string TS1Path
        {
            get => Context?.TS1InstallationPath ?? "Doesn't Exist...";
            set { if (Context != null) Context.TS1InstallationPath = value; }
        }
        public string SimitonePath
        {
            get => Context?.SimitonePath ?? "C:/Program Files/Simitone";
            set { if (Context != null) Context.SimitonePath = value; }
        }

        public SimitoneInstallationPage()
        {
            InitializeComponent();            
        }

        private void updateUI()
        {            
            if (Context == null)
                return;
            TS1PathBox.SelectedDirectory = TS1Path;
            SimitonePathBox.SelectedDirectory = SimitonePath;
            return;
            ExpansionDisplay.Children.Clear();
            for (int i = 0; i < Context.InstalledComponents.Length; i++)
                if (Context.InstalledComponents[i] && i != 0)
                    ExpansionDisplay.Children.Add(new Button()
                    {
                        Content = Context.InstalledComponentsStrings[i],
                    });
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            SimitonePath = SimitonePathBox.SelectedDirectory;
            TS1Path = TS1PathBox.SelectedDirectory;
            OnInstallationStart?.Invoke(this, null);
            IsEnabled = false;
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            OnSkip?.Invoke(this, null);
        }

        private void Host_Loaded(object sender, RoutedEventArgs e)
        {
            updateUI();
        }
    }
}
