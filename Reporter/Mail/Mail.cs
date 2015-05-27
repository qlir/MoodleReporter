using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.PropertyGridInternal;

namespace ReportsGenerator.Mail
{
    class Mail
    {
        private SmtpClient client = new SmtpClient();
        private string email;
        public void SetMailSettings(MailSettings settings)
        {
            client.Host = settings.SmtpServer;
            client.Port = settings.Port;
            client.EnableSsl = settings.EnableSsl;
            client.Credentials = new NetworkCredential(settings.Email, settings.Password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Timeout = settings.Timeout;
            email = settings.Email;
        }

        public async Task<string> SaveMails(IEnumerable<MailMessage> messages)
        {
            var oldDeliveryMethod = client.DeliveryMethod;
            client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            var oldEnableSsl = client.EnableSsl;
            client.EnableSsl = false;
            try
            {
                string mailsRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Strings.Slash + Settings.Default.DirrectoryForEmails;
                string tempDirPath = mailsRoot + Strings.TempFolder;
                string currentSessionDirPath = mailsRoot + Strings.Slash + DateTime.Now.ToString(Strings.DateFormatForFolderName);
                await Task.Run(() =>
                {
                    client.PickupDirectoryLocation = tempDirPath;
                    /*if (!Directory.Exists(tempDirPath))*/
                    if (Directory.Exists(tempDirPath)) Directory.Delete(tempDirPath, true);
                    DirectoryInfo tempDir = Directory.CreateDirectory(tempDirPath);

                    /*if (!Directory.Exists(currentSessionDirPath)) */
                    DirectoryInfo sessionDir = Directory.CreateDirectory(currentSessionDirPath);
                    foreach (var mess in messages)
                    {
                        mess.From = new MailAddress(email);
                        client.Send(mess);
                        File.Move(
                            tempDir.GetFiles().First().FullName,
                            sessionDir.FullName + Strings.Slash + string.Format(Strings.EmailName, mess.To.FirstOrDefault(), mess.Subject));
                    }
                });
                return currentSessionDirPath;
            }
            catch (Exception e)
            {
                throw new ReporterException(String.Format(Strings.SavingError, e.Message));
            }
            finally
            {
                client.DeliveryMethod = oldDeliveryMethod;
                client.EnableSsl = oldEnableSsl;
            }
        }
        internal async Task<string> SendMail(MailMessage message)
        {
            try
            {
               /* message.To.Clear();
                message.To.Add(new MailAddress("parafus@yandex.ru"));*/
                message.From = new MailAddress(email);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                /*" + message.To + ":" + message.Subject + "*/
                await client.SendMailAsync(message);
                return null;
            }
            catch (Exception e)
            {
                return string.Format(Strings.SendingError, e.Message);
            }
        }
    }
}
