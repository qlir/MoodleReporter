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
        private const string UserDataDirrectory = "UserData/";
        private const string MailTemplatePath = "Template.html";

        public static async Task<string> LoadTemplate()
        {
            string result;
            using (var file = new StreamReader(UserDataDirrectory + MailTemplatePath))
            {
                result = await file.ReadToEndAsync();
            }
            return result;
        }
    }
}
