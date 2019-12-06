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
    /// Interaction logic for SimitoneBorderButton.xaml
    /// </summary>
    public partial class SimitoneBorderButton : Button, INotifyPropertyChanged
    {
        private string _text = "Hello world";

        public event PropertyChangedEventHandler PropertyChanged;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
            }
        } 
        public SimitoneBorderButton()
        {
            InitializeComponent();
        }
    }
}
