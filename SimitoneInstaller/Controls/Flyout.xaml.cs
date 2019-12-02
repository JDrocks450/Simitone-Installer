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

namespace Simitone.Installer.UI.Controls
{
    /// <summary>
    /// Interaction logic for Flyout.xaml
    /// </summary>
    public partial class Flyout : Frame
    {
        private StackPanel BackgroundHost => Template.FindName("BackgroundHost", this) as StackPanel;
        public Flyout()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundHost.Width = 0;
            updateBackground();

            (Template.FindName("ContentHost", this) as Frame).Visibility = Visibility.Visible;
            BackgroundHost.BeginAnimation(WidthProperty,
                new DoubleAnimation(ActualWidth, TimeSpan.FromSeconds(.5))
                {
                    AccelerationRatio = .5f,
                    DecelerationRatio = .5f,
                });
        }

        private void updateBackground()
        {
            BackgroundHost.Children.Clear();
            var background = BackgroundHost.ActualHeight;
            while(background > 0)
            {
                var backgroundPiece = new SimitoneBackgroundPiece();
                var height = backgroundPiece.Height;
                BackgroundHost.Children.Add(backgroundPiece);
                background -= height;
            }
        }

        internal void Close(EventHandler Callback = null)
        {
            (Template.FindName("ContentHost", this) as Frame).Visibility = Visibility.Hidden;
            var anim = new DoubleAnimation(0, TimeSpan.FromSeconds(.5))
            {
                AccelerationRatio = .5f,
                DecelerationRatio = .5f,
            };
            if (Callback != null)
                anim.Completed += Callback;
            BackgroundHost.BeginAnimation(WidthProperty, anim);
        }
    }
}
