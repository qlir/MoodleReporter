using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ReportsGenerator.DataStructures;
using ReportsGenerator.Mail;
using ReportsGenerator.TableGenerator;

namespace ReportsGenerator
{
    public class Reporter
    {
        private readonly int[] curatorRoles = { 4, 9 };
        public static readonly string[] months = { "января", "февраля", "марта", "апреля", "мая", "июня", "июля", "августа", "сентября", "октября", "ноября", "декабря" };

        public async Task<List<Group>> GetGroups(int courseid)
        {
            if (_groupsCashe.ContainsKey(courseid))
            {
                return await _groupsCashe[courseid];
            }
            var groups = _moodle.GetCourseGroups(courseid);
            _groupsCashe.Add(courseid, groups);
            return await groups;
        }

        public string Token
        {
            get
            {
                return _moodle.Token;
            }
            set
            {
                _moodle.Token = value;
            }
        }
        public string Template { get; set; }
        public IEnumerable<ReportInfo> ReportInfo { get; set; }

        public string MoodleServer
        {
            get
            {
                return _moodle.MoodleServer;
            }
            set
            {
                _moodle.MoodleServer = value;
            }
        }

        public IEnumerable<Curator> Curators
        {
            get { return _curators; }
            set { _curators = value; }
        }

        private readonly Dictionary<int, Course> _courses = new Dictionary<int, Course>();

        private readonly Moodle.MoodleCtrl _moodle = new Moodle.MoodleCtrl();
        private readonly Mail.Mail _mail = new Mail.Mail();

        public IList<MailMessage> MessagesPreview
        {
            get { return _messagesPreview; }
        }

        private readonly ReportTableGenerator _reporter = new ReportTableGenerator();
        private readonly IDictionary<ICurator, StringBuilder> _sheetsForCurators = new Dictionary<ICurator, StringBuilder>();
        private readonly IList<MailMessage> _messagesPreview = new List<MailMessage>();
        private readonly Dictionary<int, Task<List<Group>>> _groupsCashe = new Dictionary<int, Task<List<Group>>>();
        private IEnumerable<Curator> _curators = new List<Curator>();

        public MailSettings MailSettings { get; set; }
        public string MailsPath { get; set; }
        public delegate void GenerationProgress(double progress);
        public event GenerationProgress GenerationProgressEvent;
        public async Task GenerateReportMessages()
        {
            double stepsCount = ReportInfo.Count() * 4;
            double currentStep = 0;
            MailsPath = null;
            if (!ReportInfo.Any()) throw new ReporterException("Нет данных для генерации отчета.");
            _sheetsForCurators.Clear();
            MessagesPreview.Clear();
            var coursesTask = _moodle.GetCoursesByIds(ReportInfo.Select(c => c.CourseID));
            foreach (ReportInfo rInfo in ReportInfo)
            {
                Debug.WriteLine("1 " + DateTime.Now);
                GenerationProgressEvent(++currentStep / stepsCount);
                // Генерация для каждого курса
                _reporter.reset();
                Task<List<Group>> groupsOfCourse = GetGroups(rInfo.CourseID);
                Task<List<User>> courseGroupUsers = _moodle.GetEnrolUsers(rInfo.CourseID, rInfo.GroupID);
                Task<List<Activity>> activityWithGrades = _moodle.GetGrades(rInfo.CourseID, await courseGroupUsers);
                GenerationProgressEvent(++currentStep / stepsCount);
                Debug.WriteLine("2 " + DateTime.Now);
                foreach (var activity in await activityWithGrades)
                {
                    Debug.WriteLine("3 " + DateTime.Now);
                    int temp;
                    if (!int.TryParse(activity.Id, out temp)) continue;

                    foreach (var g in activity.Grades)
                    {
                        User user = (await courseGroupUsers).First(u => u.Id == g.UserId);
                        _reporter.addItem(user, g, activity);
                    }
                    Debug.WriteLine("4 " + DateTime.Now);
                }

                GenerationProgressEvent(++currentStep / stepsCount);
                String caption = CaptionGenerate(rInfo);
                Debug.WriteLine("5 " + DateTime.Now);
                StringBuilder newSheet;
                switch (rInfo.CuratorsGenerationType)
                {
                    case DataStructures.ReportInfo.CuratorsGenerationTypeEnum.All:
                    case DataStructures.ReportInfo.CuratorsGenerationTypeEnum.MoodleCurators:
                        var institutionsCurators = (from i in await courseGroupUsers
                                                    group i by i.Institution
                                                        into g
                                                        select new
                                                        {
                                                            Institution = g.Key,
                                                            curators = Task.Run(async () =>
                                                            {
                                                                Group gr = GetInstitutionGroupFromUsers(await groupsOfCourse, g.Key, rInfo.GroupName);
                                                                return gr == null ? new List<User>() : (await _moodle.GetEnrolUsers(rInfo.CourseID, gr.Id)).Where(i => i.Roles.Any(r => curatorRoles.Contains(r.Id)));
                                                            })
                                                        }).ToArray();
                        foreach (var iCurators in institutionsCurators)
                        {
                            newSheet = _reporter.GenerateReportTable(rInfo,
                                (await coursesTask).First(c => c.Id == rInfo.CourseID), iCurators.Institution);
                            if (rInfo.CuratorsGenerationType == DataStructures.ReportInfo.CuratorsGenerationTypeEnum.All)
                            {
                                AppendSheetForCurators(Curators.Where(c => c.Institution == iCurators.Institution), caption, newSheet);
                            }
                            AppendSheetForCurators(await iCurators.curators, caption, newSheet);
                        }
                        break;
                    case DataStructures.ReportInfo.CuratorsGenerationTypeEnum.Custom:
                        var curator = Curators.First(c => c.Email == rInfo.CuratorsEmail);
                        newSheet = _reporter.GenerateReportTable(rInfo, (await coursesTask).First(c => c.Id == rInfo.CourseID), Curators.First(c => c.Email == rInfo.CuratorsEmail).Institution);
                        AppendSheetForCurator(curator, caption, newSheet);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Debug.WriteLine("6 " + DateTime.Now);
                GenerationProgressEvent(++currentStep / stepsCount);
            }

            foreach (var tables in _sheetsForCurators)
            {
                Debug.WriteLine("7 " + DateTime.Now);
                MessagesPreview.Add(GenerateMessageContent(tables.Value.ToString(), tables.Key));
                Debug.WriteLine("8 " + DateTime.Now);
            }
            try
            {
                Debug.WriteLine("9 " + DateTime.Now);
                _mail.SetMailSettings(MailSettings);
                MailsPath = await _mail.SaveMails(MessagesPreview);
                Debug.WriteLine("10 " + DateTime.Now);
            }
            catch (Exception e)
            {
                throw new ReporterException("Генерация успешно завершена, но сообщения не были сохранены из-за ошибки:\n" + e.Message, e);
            }
        }

        private void AppendSheetForCurators(IEnumerable<ICurator> curators, String caption, StringBuilder newSheet)
        {
            foreach (ICurator curator in curators) AppendSheetForCurator(curator, caption, newSheet);
        }

        private void AppendSheetForCurator(ICurator curator, String caption, StringBuilder newSheet)
        {
            curator.Caption = caption;
            var existedCurSheets = _sheetsForCurators.FirstOrDefault(c => c.Key.Email == curator.Email).Value;
            if (existedCurSheets != null)
            {
                existedCurSheets.Append(newSheet);
            }
            else
            {
                _sheetsForCurators.Add(curator, new StringBuilder(newSheet.ToString()));
            }
        }

        private Group GetInstitutionGroupFromUsers(IEnumerable<Group> groupsList, string institution, string commonGroupName)
        {
            var arr = institution.Split('_');
            var grnames = arr.Length == 1
                ? new[] { String.Format("{0}_{1}", institution, commonGroupName) }
                : new[]
                        {
                            String.Format("{0}[{1}]_{2}", arr[0], arr[1], commonGroupName),
                            String.Format("{0}_{1}_{2}", arr[0], arr[1], commonGroupName)
                        };
            Group founded = groupsList.FirstOrDefault(g => grnames.Contains(g.Name));
            if (founded != null) return founded;
            //throw new ReporterException(String.Format("Не найдена  группа \"{0}\" для организации \"{1}\".", commonGroupName, institution));
            Warnings.Add(String.Format("Не найдена  группа \"{0}\" для организации \"{1}\".", commonGroupName, institution));
            return null;
        }

        public readonly List<String> Warnings = new List<string>();

        private string CaptionGenerate(ReportInfo rinfo)
        {
            return String.Format("[Edu] результаты обучения сотрудников вашего РИЦ за период обучения с {0}{1}{2} по {3} {4} {5} года",
                rinfo.StartDate.Day,
                rinfo.StartDate.Month == rinfo.EndDate.Month ? String.Empty : " " + months[rinfo.StartDate.Month - 1],
                rinfo.StartDate.Year == rinfo.EndDate.Year ? String.Empty : " " + rinfo.StartDate.Year,
                rinfo.EndDate.Day,
                months[rinfo.EndDate.Month - 1],
                rinfo.EndDate.Year);
        }

        private MailMessage GenerateMessageContent(string tables, ICurator curator)
        {
            var email = new MailMessage();
            email.To.Add(new MailAddress(curator.Email));
            email.Subject = curator.Caption;
            email.IsBodyHtml = true;
            email.Body = Template.Replace("{{tables}}", tables).Replace(
                "{{wellcome}}",
                String.Format("Уважаем{0} {1}", (curator.IsMan ? "ый" : "ая"), curator.FirstName));
            return email;
        }

        public delegate void SendingProgress(double progress);
        public event SendingProgress SendingProgressEvent;
        public async Task<Dictionary<MailMessage,string>> SendReports()
        {
            double steps = MessagesPreview.Count;
            double curStep = 0;
            var report = new Dictionary<MailMessage, string>();
            _mail.SetMailSettings(MailSettings);
            var tasks = from message in MessagesPreview
                select new
                {
                    Mail = message,
                    Task = _mail.SendMail(message)
                };
            foreach(var item in tasks)
            {
                report.Add(item.Mail, await item.Task);
                SendingProgressEvent(++curStep/steps);
            }
            return report;
        }

        public async Task<Course> GetCourseById(int courseid)
        {
            return (await _moodle.GetCoursesByIds(new[] { courseid })).FirstOrDefault();
        }
    }

}

