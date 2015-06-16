using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Outlook;
using ReportsGenerator.Settings;

namespace ReportsGenerator.Mail
{
    internal class OutlookSaver : IDisposable
    {
        private Application _outlookApp;
        private const string EmailExtention = ".oft";

        private Application OutlookApp
        {
            get
            {
                if (_outlookApp == null)
                {
                    _outlookApp = new Application();
                    _excelProc = System.Diagnostics.Process.GetProcessesByName("OUTLOOK").Last();
                }
                return _outlookApp;
            }
        }

        private System.Diagnostics.Process _excelProc;

        public void SaveMailAsOft(MailMessage message, string pathToSave)
        {
            if (!Directory.Exists(pathToSave)) Directory.CreateDirectory(pathToSave);
            MailItem mail = OutlookApp.CreateItem(OlItemType.olMailItem);
            mail.To = String.Join(";", message.To.Select(i => i.Address));
            mail.BodyFormat = OlBodyFormat.olFormatHTML;
            mail.HTMLBody = message.Body;
            mail.CC = String.Join(";", message.CC.Select(i => i.Address));
            mail.Subject = message.Subject;
            mail.SaveAs(
                pathToSave + "/" +
                string.Format(ReporterSettings.Default.EmailName + EmailExtention, message.To.FirstOrDefault(), message.Subject),
                OlSaveAsType.olTemplate);
            mail.Close(OlInspectorClose.olDiscard);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(mail);
        }

        public List<MailMessage> LoadMailsFromFolder(string folderPath)
        {
            var files = Directory.GetFiles(folderPath);
            var mails = new List<MailMessage>();
            foreach (var mailFileName in files.Where(f => Regex.IsMatch(f, string.Format("^.*?\\{0}$", EmailExtention))))
            {
                mails.Add(LoadMail(mailFileName));
            }
            return mails;
        }
        public MailMessage LoadMail(string filePath)
        {
            MailItem mail = OutlookApp.CreateItemFromTemplate(filePath) as MailItem;
            var result = new MailMessage();
            result.Body = mail.HTMLBody;
            result.IsBodyHtml = true;
            result.Subject = mail.Subject;
            if (mail.CC != null)
                foreach (var address in mail.CC.Split(';'))
                {
                    result.CC.Add(address);
                }
            if (mail.To != null)
                foreach (var address in mail.To.Split(';'))
                {
                    result.To.Add(address);
                }
            mail.Close(OlInspectorClose.olDiscard);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(mail);

            return result;
        }

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (_outlookApp != null)
                {
                    _outlookApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(_outlookApp);
                    _outlookApp = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    _excelProc.Kill();
                }
                disposed = true;
            }
        }

        ~OutlookSaver()
        {
            Dispose(false);
        }
    }
}
