using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ReportsGenerator.Settings;
using MessageBox = UIReporter.AccessoryWindow.MessageBox;

namespace UIReporter.SettingsViews
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class BaseSettingsGrid : Grid
    {

        public BaseSettingsGrid()
        {
            InitializeComponent();
            Password.Password = MailSettings.Default.Password;
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            MailSettings mail = MailSettings.Default;
            mail.Email = mail.Email.Trim();
            if (!Regex.IsMatch(mail.Email,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase))
            {
                MessageBox.Show(this, "Некорректный E-mail.");
                return;
            }
            mail.SmtpServer = mail.SmtpServer.Trim();
            if (string.IsNullOrEmpty(mail.SmtpServer))
            {
                MessageBox.Show(this, "Поле 'SMPT' сервер не может быть пустым");
                return;
            }
            //mailDefault.Port = mailDefault.Port.Trim();
            mail.Password = Password.Password.Trim();
            if (string.IsNullOrEmpty(mail.Password))
            {
                MessageBox.Show(this, "Поле 'Пароль' не может быть пустым");
                return;
            }

            MoodleSettings moodle = MoodleSettings.Default;
            moodle.Token = moodle.Token.Trim();
            if (string.IsNullOrEmpty(moodle.Token))
            {
                MessageBox.Show(this, "Полу 'Токен' не может быть пустым");
                return;
            }
            moodle.Server = moodle.Server.Trim();
            if (string.IsNullOrEmpty(moodle.Server))
            {
                MessageBox.Show(this, "Поле 'Сервер' не может быть пустым.");
                return;
            }

            MailSettings.Default.Save();
            MoodleSettings.Default.Save();
            // Close();
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (Char.IsNumber(c))
                e.Handled = false;
            else
                e.Handled = true;

            base.OnPreviewTextInput(e);
        }

        private void Discard_OnClick(object sender, RoutedEventArgs e)
        {
            MailSettings.Default.Reset();
            MailSettings.Default.Save();
            MoodleSettings.Default.Reset();
            MoodleSettings.Default.Save();
        }
    }
}
