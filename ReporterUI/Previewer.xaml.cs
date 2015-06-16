using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using ReportsGenerator;
using ReportsGenerator.Moodle;
using UIReporter.AccessoryWindow;

namespace UIReporter
{
    /// <summary>
    /// Interaction logic for Previewer.xaml
    /// </summary>
    public partial class Previewer : Window, INotifyPropertyChanged
    {
        public List<MailMessage> Messages { get; set; }

        public MailMessage DisplayedMessage
        {
            get
            {
                return Messages[Index];
            }
        }

        private int _index;

        public int Number { get { return _index + 1; } }

        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                StringBuilder sb = new StringBuilder();
                char[] s = DisplayedMessage.Body.ToCharArray();
                foreach (char c in s)
                {
                    if (Convert.ToInt32(c) > 127)
                        sb.Append("&#" + Convert.ToInt32(c) + ";");
                    else
                        sb.Append(c);
                }
                Browser.NavigateToString(sb.ToString());
            }
        }

        public void Next()
        {
            if (Index < Messages.Count - 1)
            {
                Index++;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("DisplayedMessage"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Number"));
            }
        }

        public void Back()
        {
            if (Index > 0)
            {
                Index--;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("DisplayedMessage"));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Number"));
            }
        }

        public Previewer(List<MailMessage> messagesPreview)
        {
            Messages = messagesPreview;
            InitializeComponent();
            Index = 0;
        }

        private void webBrowser1_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            e.Cancel = e.Uri != null;
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            Back();
        }

        private void Next_OnClick(object sender, RoutedEventArgs e)
        {
            Next();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
