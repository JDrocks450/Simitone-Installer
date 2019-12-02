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

namespace Simitone.Installer.UI.Pages
{
    /// <summary>
    /// Interaction logic for DialogPage.xaml
    /// </summary>
    public partial class DialogPage : Page, INotifyPropertyChanged
    {
        public delegate void DialogResultHandler(DialogPage sender, bool? result);
        public event DialogResultHandler ResultOK, ResultNo, ResultYes, ResultCancel, DialogClosed;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
            }
        }
        public enum CMsgBoxButtons
        {
            OK,
            YesNo,
            YesNoCancel
        }
        public CMsgBoxButtons Buttons
        {
            get => _buttons;
            set
            {
                _buttons = value;
                switchButtons(value);
            }
        }
        private CMsgBoxButtons _buttons;

        private string _text = "The user should never see this!";

        public event PropertyChangedEventHandler PropertyChanged;

        public DialogPage()
        {
            InitializeComponent();
        }
        private void switchButtons(CMsgBoxButtons newButtons)
        {
            ButtonsHost.Children.Clear();
            switch (newButtons)
            {
                case CMsgBoxButtons.OK:
                    {
                        var b = new Button() { Content = "OK" };
                        b.Foreground = ButtonDesign.Foreground;
                        b.Click += (object sender, RoutedEventArgs e) => { ResultOK?.Invoke(this, true); DialogClosed?.Invoke(this, true); };
                        ButtonsHost.Children.Add(b);
                    }
                    break;
                case CMsgBoxButtons.YesNoCancel:
                    {
                        var b = new Button() { Content = "Cancel" };
                        b.Foreground = ButtonDesign.Foreground;
                        b.Click += (object sender, RoutedEventArgs e) => { ResultCancel?.Invoke(this, null); DialogClosed?.Invoke(this, null); };
                        ButtonsHost.Children.Add(b);
                    }
                    goto case CMsgBoxButtons.YesNo;
                case CMsgBoxButtons.YesNo:
                    {
                        var b = new Button() { Content = "Yes" };
                        b.Foreground = ButtonDesign.Foreground;
                        b.Click += (object sender, RoutedEventArgs e) => { ResultYes?.Invoke(this, true); DialogClosed?.Invoke(this, true); };
                        ButtonsHost.Children.Add(b);
                        b = new Button() { Content = "No" };
                        b.Foreground = ButtonDesign.Foreground;
                        b.Click += (object sender, RoutedEventArgs e) => { ResultNo?.Invoke(this, false); DialogClosed?.Invoke(this, false); };
                        ButtonsHost.Children.Add(b);
                    }
                    break;
            }
        }
    }
}
