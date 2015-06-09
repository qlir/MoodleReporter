using System;
using System.Threading.Tasks;
using System.Windows;

namespace UIReporter
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : Window
    {

        public double Value
        {
            get
            {
                return ProgressBar.Value;
            }
            set
            {
                ProgressBar.IsIndeterminate = value < 0;
                ProgressBar.Value = value;
            }
        }

        public ProgressDialog()
        {
            InitializeComponent();
        }

        private Func<ProgressDialog, Task> _asyncFunc;

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue) Value = 0;
        }

        public void Run(Func<ProgressDialog, Task> asyncFunc)
        {
            _asyncFunc = asyncFunc;
            ShowDialog();
        }

        private async void ProgressDialog_OnContentRendered(object sender, EventArgs e)
        {
            if (_asyncFunc == null) return;
            await _asyncFunc(this);
            Close();
        }

        public static void ShowDialog(Window ownerWindow,string title, Func<ProgressDialog, Task> asyncFunc, double value = -1)
        {
            var progressDialog = new ProgressDialog { Value = value, Title = title };
            progressDialog.Owner = ownerWindow;
            progressDialog.Run(asyncFunc);
        }
    }
}
