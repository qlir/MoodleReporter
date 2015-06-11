using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReportsGenerator.DataStructures;
using ReportsGenerator.Settings;
using ReportsGenerator.TableGenerator;
using Group = ReportsGenerator.DataStructures.Group;

namespace ReportsGenerator
{
    public class Reporter
    {
        private readonly int[] _curatorRoles = { 4, 9 };
        public static readonly string[] Months = { "января", "февраля", "марта", "апреля", "мая", "июня", "июля", "августа", "сентября", "октября", "ноября", "декабря" };

        public async Task<List<Group>> GetGroups(int courseid)
        {
            if (this._groupsCashe.ContainsKey(courseid))
            {
                return await this._groupsCashe[courseid];
            }
            var groups = this._moodle.GetCourseGroups(courseid);
            this._groupsCashe.Add(courseid, groups);
            return await groups;
        }

        public string Token
        {
            get
            {
                return this._moodle.Token;
            }
            set
            {
                this._moodle.Token = value;
            }
        }
        public string Template
        {
            get;
            set;
        }
        public IEnumerable<ReportInfo> ReportInfo
        {
            get;
            set;
        }

        public string MoodleServer
        {
            get
            {
                return this._moodle.MoodleServer;
            }
            set
            {
                this._moodle.MoodleServer = value;
            }
        }

        public IEnumerable<Curator> Curators
        {
            get
            {
                return this._curators;
            }
            set
            {
                this._curators = value;
            }
        }

        private readonly Dictionary<int, Course> _courses = new Dictionary<int, Course>();

        private readonly Moodle.MoodleCtrl _moodle = new Moodle.MoodleCtrl();
        private readonly Mail.Mail _mail = new Mail.Mail();

        public IDictionary<ICurator, MailMessage> MessagesPreview
        {
            get
            {
                return this._messagesPreview;
            }
        }

        private readonly IDictionary<ICurator, StringBuilder> _sheetsForCurators = new Dictionary<ICurator, StringBuilder>();
        private readonly IDictionary<ICurator, MailMessage> _messagesPreview = new Dictionary<ICurator, MailMessage>();
        private readonly Dictionary<int, Task<List<Group>>> _groupsCashe = new Dictionary<int, Task<List<Group>>>();
        private IEnumerable<Curator> _curators = new List<Curator>();

        public string MailsPath
        {
            get;
            set;
        }

        public delegate void GenerationProgress(double progress);
        public event GenerationProgress GenerationProgressEvent;
        public async Task GenerateReportMessages()
        {
            ReportTableGenerator reporterGenerator = new ReportTableGenerator();
            double stepsCount = this.ReportInfo.Count() * 4;
            double currentStep = 0;
            this.MailsPath = null;
            if (!this.ReportInfo.Any())
            {
                throw new ReporterException("Нет данных для генерации отчета.");
            }
            this._sheetsForCurators.Clear();
            this.MessagesPreview.Clear();
            var coursesTask = this._moodle.GetCoursesByIds(this.ReportInfo.Select(c => c.CourseID));
            foreach (ReportInfo rInfo in this.ReportInfo)
            {
                this.GenerationProgressEvent(++currentStep / stepsCount);
                // Генерация для каждого курса
                reporterGenerator.Reset();
                Task<List<Group>> groupsOfCourse = this.GetGroups(rInfo.CourseID);
                Task<List<User>> courseGroupUsers = this._moodle.GetEnrolUsers(rInfo.CourseID, rInfo.GroupID);
                Task<List<Activity>> activityWithGrades = this._moodle.GetGrades(rInfo.CourseID, await courseGroupUsers);
                this.GenerationProgressEvent(++currentStep / stepsCount);
                foreach (var activity in await activityWithGrades)
                {
                    int temp;
                    if (!int.TryParse(activity.Id, out temp))
                    {
                        continue;
                    }

                    foreach (var g in activity.Grades)
                    {
                        User user = (await courseGroupUsers).First(u => u.Id == g.UserId);
                        reporterGenerator.AddItem(user, g, activity);
                    }
                }

                this.GenerationProgressEvent(++currentStep / stepsCount);
                String caption = this.CaptionGenerate(rInfo);
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
                                    return gr == null ? new List<User>() : (await _moodle.GetEnrolUsers(rInfo.CourseID, gr.Id)).Where(i => i.Roles.Any(r => _curatorRoles.Contains(r.Id)));
                                })
                            }).ToArray();
                        foreach (var iCurators in institutionsCurators)
                        {
                            Course course = (await coursesTask).First(c => c.Id == rInfo.CourseID);
                            newSheet = reporterGenerator.GenerateReportTable(
                                rInfo,
                                course,
                                iCurators.Institution);
                            if (rInfo.CuratorsGenerationType == DataStructures.ReportInfo.CuratorsGenerationTypeEnum.All)
                            {
                                this.AppendSheetForCurators(
                                    this.Curators.Where(c => c.Institution == iCurators.Institution && CheckDirection(course,c)), caption, newSheet);
                            }
                            this.AppendSheetForCurators(await iCurators.curators, caption, newSheet);
                        }
                        break;
                    case DataStructures.ReportInfo.CuratorsGenerationTypeEnum.Custom:
                        var curator = this.Curators.First(c => c.Email == rInfo.CuratorsEmail);
                        newSheet = reporterGenerator.GenerateReportTable(rInfo, (await coursesTask).First(c => c.Id == rInfo.CourseID), this.Curators.First(c => c.Email == rInfo.CuratorsEmail).Institution);
                        this.AppendSheetForCurator(curator, caption, newSheet);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                this.GenerationProgressEvent(++currentStep / stepsCount);
            }

            foreach (var tables in this._sheetsForCurators)
            {
                this.MessagesPreview.Add(tables.Key, this.GenerateMessage(tables.Value.ToString(), tables.Key));
            }
            try
            {
                this._mail.UpdateMailSettings();

                string mailPath = string.Format("{0}/{1}/{2}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), ReporterSettings.Default.DirrectoryForEmails, DateTime.Now.ToString(ReporterSettings.Default.DateFormatForFolderName));
                foreach (var mess in this.MessagesPreview)
                {
                    _mail.SaveMail(mess.Value, string.Format("{0}/{1}", mailPath, mess.Key.Institution));
                }
                this.MailsPath = mailPath;
            }
            catch (Exception e)
            {
                throw new ReporterException("Генерация успешно завершена, но сообщения не были сохранены из-за ошибки:\n" + e.Message, e);
            }
        }

        private bool CheckDirection(Course course, ICurator curator)
        {
            if (string.IsNullOrWhiteSpace(curator.Direction)) return true;
            return Regex.IsMatch(course.ShortName, string.Format(GenerationSetting.Default.PatternToCheckDirection, curator.Direction));
        }

        private void AppendSheetForCurators(IEnumerable<ICurator> curators, String caption, StringBuilder newSheet)
        {
            foreach (ICurator curator in curators)
            {
                this.AppendSheetForCurator(curator, caption, newSheet);
            }
        }

        private void AppendSheetForCurator(ICurator curator, String caption, StringBuilder newSheet)
        {
            curator.Caption = caption;
            var existedCurSheets = this._sheetsForCurators.FirstOrDefault(c => c.Key.Email == curator.Email).Value;
            if (existedCurSheets != null)
            {
                existedCurSheets.Append(newSheet);
            }
            else
            {
                this._sheetsForCurators.Add(curator, new StringBuilder(newSheet.ToString()));
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
            if (founded != null)
            {
                return founded;
            }
            this.Warnings.Add(String.Format("Не найдена  группа \"{0}\" для организации \"{1}\".", commonGroupName, institution));
            return null;
        }

        public readonly List<String> Warnings = new List<string>();

        private string CaptionGenerate(ReportInfo rinfo)
        {
            return String.Format(
                String.Format(
                    GenerationSetting.Default.MailSubject,
                    "{0}{1}{2}",
                    "{3} {4} {5}"),
                rinfo.StartDate.Day,
                rinfo.StartDate.Month == rinfo.EndDate.Month ? String.Empty : " " + Months[rinfo.StartDate.Month - 1],
                rinfo.StartDate.Year == rinfo.EndDate.Year ? String.Empty : " " + rinfo.StartDate.Year,
                rinfo.EndDate.Day,
                Months[rinfo.EndDate.Month - 1],
                rinfo.EndDate.Year);
        }

        private MailMessage GenerateMessage(string tables, ICurator curator)
        {
            var email = new MailMessage();
            email.To.Add(new MailAddress(curator.Email));
            email.Subject = curator.Caption;
            email.IsBodyHtml = true;
            foreach (var ccEmail in GenerationSetting.Default.CC.Split(';'))
            {
                if (!string.IsNullOrEmpty(ccEmail))
                {
                    email.CC.Add(new MailAddress(ccEmail));
                }
            }
            email.Body = this.Template.Replace(GenerationSetting.Default.TagToTablesPaste, tables).Replace(
                             GenerationSetting.Default.TagToWelcomePaste,
                             String.Format(GenerationSetting.Default.Welcome, (curator.IsMan ? GenerationSetting.Default.WelcomeMalePostfix : GenerationSetting.Default.WelcomeFemalePostfix), curator.FirstName));
            return email;
        }

        public delegate void SendingProgress(double progress);
        public event SendingProgress SendingProgressEvent;
        public async Task<Dictionary<MailMessage, string>> SendReports()
        {
            double steps = this.MessagesPreview.Count;
            double curStep = 0;
            var report = new Dictionary<MailMessage, string>();
            this._mail.UpdateMailSettings();
            var tasks = from message in this.MessagesPreview
                        select new
            {
                Mail = message,
                Task = this._mail.SendMail(message.Value)
            };
            foreach (var item in tasks)
            {
                report.Add(item.Mail.Value, await item.Task);
                this.SendingProgressEvent(++curStep / steps);
            }
            return report;
        }

        public async Task<Course> GetCourseById(int courseid)
        {
            return (await this._moodle.GetCoursesByIds(new[] { courseid })).FirstOrDefault();
        }
    }

}

