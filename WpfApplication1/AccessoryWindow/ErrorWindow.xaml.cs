using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ReportsGenerator;
using ReportsGenerator.Moodle;
using UIReporter.Helpers;

namespace UIReporter.AccessoryWindow
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ImageSource ErrorImage { get; set; }
        public Exception Exception { get; set; }

        public ErrorWindow(Exception e)
        {
            Exception = e;
            ErrorImage = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Error.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            InitializeComponent();
            //ErrorExpander.Visibility = e.StackTrace == null || e is ReporterException ? Visibility.Hidden : Visibility.Visible;
            if (e is ReporterException)
            {
              //  ResizeMode = ResizeMode.NoResize;  
            }
        }

        private void window_ContentRendered(object sender, EventArgs e)
        {
            SystemSounds.Hand.Play();
        }
        public static void ShowError(Exception e)
        {
            new ErrorWindow(e).ShowDialog();
        }
    }
}
