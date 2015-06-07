using System;
using System.Linq;
using System.Windows;

namespace UIReporter.AccessoryWindow
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window
    {
        public string Text { get; set; }
        public MessageBoxButton Buttons { get; set; }

        public MessageBoxResult Result;

        public MessageBox(string text, string caption, MessageBoxButton button)
        {
            Text = text ?? string.Empty;
            Title = caption ?? string.Empty;
            InitializeComponent();
            MouseLeftButtonDown += (s, e) => DragMove();
            ContentRendered += (s, e) => System.Media.SystemSounds.Asterisk.Play();
            switch (button)
            {
                case MessageBoxButton.OKCancel:
                    CancelButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    CancelButton.Visibility = Visibility.Visible;
                    OkButton.Visibility = Visibility.Hidden;
                    break;
                case MessageBoxButton.YesNo:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    break;
            }
            Result = MessageBoxResult.Cancel;
        }

        public static MessageBoxResult Show(Object ownerWindow, string messageBoxText, string caption = null, MessageBoxButton button = MessageBoxButton.OK)
        {
            var mb = new MessageBox(messageBoxText, caption, button);
            mb.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            mb.ShowDialog();
            return mb.Result;
        }

        private void YesButton_OnClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            Close();
        }

        private void NoButton_OnClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            Close();
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            Close();
        }
    }
}
