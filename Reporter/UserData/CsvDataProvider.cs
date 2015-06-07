using System.Linq;

namespace ReportsGenerator.UserData
{
    using CsvHelper;
    using CsvHelper.Configuration;
    using Newtonsoft.Json;
    using DataStructures;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    public static class CsvDataProvider
    {
        private const string UserDataDirrectory = "UserData/";
        private const string CuratorsFileName = "Curators.csv";
        private const string ReportInfoFileName = "ReportInfo.csv";
        private const string CoursesFileName = "Courses.csv";
        private readonly static CsvConfiguration CsvConfiguration;

        static CsvDataProvider()
        {
            CsvConfiguration = new CsvConfiguration();
            CsvConfiguration.HasHeaderRecord = true;
            CsvConfiguration.IgnoreHeaderWhiteSpace = true;
            string delimiter = ReporterSettings.Default.CsvDelimiter;
            if (!string.IsNullOrEmpty(delimiter))
            {
                CsvConfiguration.Delimiter = delimiter;
            }
            CsvConfiguration.Encoding = Encoding.GetEncoding(ReporterSettings.Default.CsvEncoding);
            CsvConfiguration.RegisterClassMap<CuratorCsvMap>();
            CsvConfiguration.RegisterClassMap<CourseCsvMap>();
            CsvConfiguration.RegisterClassMap<ReportInfoCsvMap>();
        }

        public static async Task<List<Curator>> LoadCurators()
        {
            return await ReadCsvAsync<Curator>(CuratorsFileName) ?? new List<Curator>();
        }

        public static async Task<List<ReportInfo>> LoadReportInfo()
        {
            return await ReadCsvAsync<ReportInfo>(ReportInfoFileName) ?? new List<ReportInfo>();
        }

        public static async Task<List<Course>> LoadCourses()
        {
            return (await ReadCsvAsync<Course>(CoursesFileName)) ?? new List<Course>();
        }

        private static async Task<List<T>> ReadCsvAsync<T>(string fileName)
        {
            if (!File.Exists(UserDataDirrectory + fileName)) return null;

            return await Task.Run(() =>
            {
                List<T> items;
                using (var reader =
                    new StreamReader(UserDataDirrectory + fileName, CsvConfiguration.Encoding))
                {
                    var csvReader = new CsvReader(reader, CsvConfiguration);
                    items = csvReader.GetRecords<T>().ToList();
                }
                return items;
            });
        }

        public static async Task WriteToCsvAsync<T>(string fileName, IEnumerable<T> items)
        {
            await Task.Run(() =>
            {
                using (var textWriter =
                    new StreamWriter(UserDataDirrectory + fileName, false, CsvConfiguration.Encoding))
                {
                    var csvWriter = new CsvWriter(textWriter, CsvConfiguration);

                    csvWriter.WriteHeader(typeof(T));

                    foreach (var item in items)
                    {
                        csvWriter.WriteRecord(item);
                    }
                    csvWriter.Dispose();
                }
            });
        }

        public sealed class CuratorCsvMap : CsvClassMap<Curator>
        {
            public CuratorCsvMap()
            {
                Map(m => m.Email).Name("Email");
                Map(m => m.FirstName).Name("FirstName");
                Map(m => m.LastName).Name("LastName");
                Map(m => m.Gender).Name("Gender");
                Map(m => m.City).Name("City");
                Map(m => m.Institution).Name("Institution");
            }
        }

        public sealed class CourseCsvMap : CsvClassMap<Course>
        {
            public CourseCsvMap()
            {
                Map(m => m.Id).Name("Id");
                Map(m => m.ShortName).Name("ShortName");
            }
        }
        public sealed class ReportInfoCsvMap : CsvClassMap<ReportInfo>
        {
            public ReportInfoCsvMap()
            {
                Map(m => m.CourseID).Name("CourseID");
                Map(m => m.StartDate).Name("StartDate");
                Map(m => m.EndDate).Name("EndDate");
                Map(m => m.GroupID).Name("GroupID");
                Map(m => m.GroupName).Name("GroupName");
                Map(m => m.CuratorsEmail).Name("CuratorsEmail");
                Map(m => m.CuratorsGenerationType).Name("CuratorsGenerationType");
            }
        }

        public static async Task SaveCurators(IEnumerable<Curator> curators)
        {
            await WriteToCsvAsync(CuratorsFileName, curators);
        }

        public static async Task SaveCourses(IEnumerable<Course> courses)
        {
            await WriteToCsvAsync(CoursesFileName, courses);
        }

        public static async Task SaveReportInfo(IEnumerable<ReportInfo> reportInfo)
        {
            await WriteToCsvAsync(ReportInfoFileName, reportInfo);
        }
    }
}
