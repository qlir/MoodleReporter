using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.PropertyGridInternal;
using ReportsGenerator.Settings;

namespace ReportsGenerator.Mail
{
    class Mail
    {
        private SmtpClient client = new SmtpClient();
        private string email;
        public void UpdateMailSettings()
        {
            client.Host = MailSettings.Default.SmtpServer;
            client.Port = MailSettings.Default.Port;
            client.EnableSsl = MailSettings.Default.EnableSsl;
            client.Credentials = new NetworkCredential(MailSettings.Default.Email, MailSettings.Default.Password);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Timeout = MailSettings.Default.Timeout;
            email = MailSettings.Default.Email;
        }

        public async Task<string> SaveMails(IEnumerable<MailMessage> messages)
        {
            var oldDeliveryMethod = client.DeliveryMethod;
            client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            var oldEnableSsl = client.EnableSsl;
            client.EnableSsl = false;
            try
            {
                string mailsRoot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/" + ReporterSettings.Default.DirrectoryForEmails;
                string tempDirPath = mailsRoot + "\\temp";
                string currentSessionDirPath = mailsRoot + "/" + DateTime.Now.ToString(ReporterSettings.Default.DateFormatForFolderName);
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
                            sessionDir.FullName + "/" + string.Format(ReporterSettings.Default.EmailName, mess.To.FirstOrDefault(), mess.Subject));
                    }
                });
                return currentSessionDirPath;
            }
            catch (Exception e)
            {
                throw new ReporterException(String.Format("Ошибка сохранения письма: {0}", e.Message));
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
/*                message.To.Clear();
                message.To.Add(new MailAddress("parafus@yandex.ru"));*/
                message.From = new MailAddress(email);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                /*" + message.To + ":" + message.Subject + "*/
                await client.SendMailAsync(message);
                return null;
            }
            catch (Exception e)
            {
                return string.Format("Ошибка отправки письма: {0}", e.Message);
            }
        }
    }
}
