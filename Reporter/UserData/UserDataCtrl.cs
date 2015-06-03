using System.Net.Mail;
using Newtonsoft.Json;
using ReportsGenerator.DataStructures;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportsGenerator.UserData
{
    public class UserDataCtrl
    {
        private const string UserDataDirrectory = "UserData/";
        private const string CuratorsJsonPath = "Curators.json";
        private const string ReportInfoJsonPath = "ReportInfo.json";
        private const string CoursesPath = "Courses.json";
        private const string MailTemplatePath = "Template.html";

        public static async Task<IEnumerable<Curator>> LoadCurators()
        {
            return await LoadJsonFileAs<IEnumerable<Curator>>(CuratorsJsonPath) ?? new Curator[0];
        }

        public static async Task<IEnumerable<ReportInfo>> LoadReportInfo()
        {
            return await LoadJsonFileAs<IEnumerable<ReportInfo>>(ReportInfoJsonPath) ?? new ReportInfo[0];
        }

        public static async Task<IEnumerable<Course>> LoadCourses()
        {
            return await LoadJsonFileAs<IEnumerable<Course>>(CoursesPath) ?? new Course[0];
        }

        private static async Task<T> LoadJsonFileAs<T>(string fileName)
        {
            string json;
            string filePath = UserDataDirrectory + fileName;
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(UserDataDirrectory))
                {
                    Directory.CreateDirectory(UserDataDirrectory);
                }
            }
            using (var file = new StreamReader(File.Open(filePath, FileMode.OpenOrCreate)))
            {
                json = await file.ReadToEndAsync();
            }
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static async Task<string> LoadTemplate()
        {
            string result;
            using (var file = new StreamReader(UserDataDirrectory + MailTemplatePath))
            {
                result = await file.ReadToEndAsync();
            }
            return result;
        }

        public static async Task WriteStringToFile(string fileName, string strToWrite)
        {
            using (var file = new StreamWriter(UserDataDirrectory + fileName, false))
            {
                await file.WriteAsync(strToWrite);
            }
        }

        public static async Task SaveCurators(IEnumerable<Curator> curators)
        {
            var json = JsonConvert.SerializeObjectAsync(curators);
            await WriteStringToFile(CuratorsJsonPath, await json);
        }

        public static async Task SaveCourses(IEnumerable<Course> courses)
        {
            var json = JsonConvert.SerializeObjectAsync(courses);
            await WriteStringToFile(CoursesPath, await json);
        }

        public static async Task SaveReportInfo(IEnumerable<ReportInfo> reportInfo)
        {
            var json = JsonConvert.SerializeObjectAsync(reportInfo);
            await WriteStringToFile(ReportInfoJsonPath, await json);
        }
        /*
                public async Task SaveMails(IEnumerable<MailMessage> message)
                {

                }*/
    }
}
