namespace ReportsGenerator.Mail
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MailSettings
    {
        public string Email
        {
            get;
            set;
        }

        public bool EnableSsl
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public string SmtpServer
        {
            get;
            set;
        }

        public int Timeout
        {
            get;
            set;
        }
    }
}