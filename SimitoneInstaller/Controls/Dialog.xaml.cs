using Simitone.Installer.UI.Pages;
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
using static Simitone.Installer.UI.Pages.DialogPage;

namespace Simitone.Installer.UI.Controls
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog : UserControl
    {
        public event DialogResultHandler ResultOK, ResultNo, ResultYes, ResultCancel, DialogClosed;

        private string _title;
        private string _text;

        public DialogPage DialogPageHost;
        public string Title
        {
            get => DialogPageHost?.Title ?? _title;
            set
            {
                if (DialogPageHost != null)                
                    DialogPageHost.Title = value;                
                _title = value;
            }
        }
        public string Text
        {
            get => DialogPageHost?.Text ?? _text;
            set
            {
                if (DialogPageHost != null)
                    DialogPageHost.Text = value;
                _text = value;
            }
        }
        public Dialog()
        {     
            InitializeComponent();
            DialogPageHost = new DialogPage();
            DialogPageFlyout.Content = DialogPageHost;
        }

        private void BackRect_Loaded(object sender, RoutedEventArgs e)
        {
            backRect.BeginAnimation(WidthProperty,
                new DoubleAnimation(ActualWidth, TimeSpan.FromSeconds(.5))
                {
                    AccelerationRatio = .5f,
                    DecelerationRatio = .5f,
                });
            DialogPageHost.DialogClosed += DialogPageHost_DialogClosed;
        }

        public static Dialog GetDialog(string Title, string Message, CMsgBoxButtons buttons = CMsgBoxButtons.OK)
        {
            Dialog d = new Dialog()
            {
                Title = Title,
                Text = Message
            };
            d.DialogPageHost.Buttons = buttons;
            return d;
        }

        public static Dialog ShowDialog(string Title, string Message, CMsgBoxButtons buttons = CMsgBoxButtons.OK)
        {
            Dialog d = null;
            MainWindow.InstallerWindow.Dispatcher.Invoke(() =>
            {
                d = GetDialog(Title, Message, buttons);
                MainWindow.ShowDialog(d);
            });
            return d;
        }

        public void Close()
        {
            var anim = new DoubleAnimation(0, TimeSpan.FromSeconds(.5))
                {
                    AccelerationRatio = .5f,
                    DecelerationRatio = .5f,
                };
            anim.Completed += (object s, EventArgs e) => MainWindow.HideDialog();
            backRect.BeginAnimation(WidthProperty, anim);
            DialogPageFlyout.Close();
        }

        private void DialogPageHost_DialogClosed(DialogPage sender, bool? result)
        {
            if (result == null)
                ResultCancel?.Invoke(sender, result);
            else if (result == true)
            {
                ResultOK?.Invoke(sender, result);
                ResultYes?.Invoke(sender, result);
            }
            else
                ResultNo?.Invoke(sender, result);
            DialogClosed?.Invoke(sender, result);
            Close();
        }
    }
}
