using System.Net.Mail;
using Newtonsoft.Json;
using ReportsGenerator.DataStructures;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportsGenerator.UserData
{
    public class TemplateProvider
    {
        private const string UserDataDirrectory = "Templates/";
        private const string MailTemplatePath = "Template.html";
        private const string MailLastTemplatePath = "SummaryTemplate.html";

        public static async Task<string> LoadDefaultTemplate()
        {
            return await LoadTemplate(MailTemplatePath);
        }

        public static async Task<string> LoadLastTemplate()
        {
            return await LoadTemplate(MailLastTemplatePath);
        }
        public static async Task<string> LoadTemplate(string templateName)
        {
            return await ReadFileAsync(UserDataDirrectory + templateName);
        }


        public static async Task<string> ReadFileAsync(string path)
        {
            if (!File.Exists(path)) return null;
            string result;

            using (var file = new StreamReader(path))
            {
                result = await file.ReadToEndAsync();
            }
            return result;
        }
    }
}
