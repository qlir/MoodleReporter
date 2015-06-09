using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UIReporter.AccessoryWindow
{
    /// <summary>
    /// Interaction logic for SendReportWindow.xaml
    /// </summary>
    public partial class SendReportWindow : Window
    {
        public SendReportWindow(Dictionary<MailMessage, string> details)
        {
            InitializeComponent();
            StatusTextBlock.Text = string.Format("Успешно отправлено  {0} из {1} писем.", details.Count(i => i.Value == null), details.Count);
            var sb = new StringBuilder();
            foreach (KeyValuePair<MailMessage, string> i in details)
            {
                sb.Append(string.Format("{0} - Получатель: {1}, Тема: {2};{3}\n", i.Value == null ? "ОК" : "Ошибка",
                        i.Key.To.First(), i.Key.Subject, i.Value ?? string.Empty));
            }
            DatailsTextBox.Text = sb.ToString();
            ContentRendered += (s, e) => System.Media.SystemSounds.Asterisk.Play();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
