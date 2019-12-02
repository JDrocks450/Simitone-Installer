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

namespace Simitone.Installer.UI.Controls
{
    /// <summary>
    /// Interaction logic for SimitoneTextbox.xaml
    /// </summary>
    public partial class SimitoneTextbox : TextBox
    {
        public string ActualText
        {
            get => (Template.FindName("BaseTextBox", this) as TextBox).Text;
        }

        public SimitoneTextbox()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = (sender as TextBox).Text;
        }
    }
}
