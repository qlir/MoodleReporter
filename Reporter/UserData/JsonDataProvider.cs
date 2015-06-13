using System.Net.Mail;
using Newtonsoft.Json;
using ReportsGenerator.DataStructures;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReportsGenerator.UserData
{
    public class JsonDataProvider
    {
        private const string UserDataDirrectory = "UserData/";
        private const string CuratorsJsonPath = "Curators.json";
        private const string ReportInfoJsonPath = "ReportInfo.json";
        private const string CoursesPath = "Courses.json";
        private const string MailTemplatePath = "DefaultTemplate.html";

        public static async Task<List<Curator>> LoadCurators()
        {
            return await LoadJsonFileAs<List<Curator>>(CuratorsJsonPath);
        }

        public static async Task<List<ReportInfo>> LoadReportInfo()
        {
            return await LoadJsonFileAs<List<ReportInfo>>(ReportInfoJsonPath);
        }

        public static async Task<List<Course>> LoadCourses()
        {
            return await LoadJsonFileAs<List<Course>>(CoursesPath);
        }

        private static async Task<T> LoadJsonFileAs<T>(string fileName) where T : new()
        {
            string json;
            string filePath = UserDataDirrectory + fileName;
            if (!File.Exists(filePath))
            {
                return new T();
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

        public static async Task SaveCurators(List<Curator> curators)
        {
            var json = JsonConvert.SerializeObjectAsync(curators);
            await WriteStringToFile(CuratorsJsonPath, await json);
        }

        public static async Task SaveCourses(List<Course> courses)
        {
            var json = JsonConvert.SerializeObjectAsync(courses);
            await WriteStringToFile(CoursesPath, await json);
        }

        public static async Task SaveReportInfo(List<ReportInfo> reportInfo)
        {
            var json = JsonConvert.SerializeObjectAsync(reportInfo);
            await WriteStringToFile(ReportInfoJsonPath, await json);
        }
        /*
                public async Task SaveMails(List<MailMessage> message)
                {

                }*/
    }
}
