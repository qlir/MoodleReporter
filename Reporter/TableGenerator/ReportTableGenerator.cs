using ReportsGenerator.Settings;

namespace ReportsGenerator.TableGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using DataStructures;

    public class ReportTableGenerator
    {
        private readonly string[] _baseColumnHeaders = { GenerationSetting.Default.FioColumnTite, GenerationSetting.Default.InstitutionColumnTitle };

        private readonly Dictionary<string, string> _passedColumnStyles = new Dictionary<string, string>();

        private readonly string _numberFormat = GenerationSetting.Default.NumberFormat;

        private List<Item> items = new List<Item>();

        public ReportTableGenerator()
        {
            string pattern = "\"(.*?)\"=\"(.*?)\"";
            var match = Regex.Match(GenerationSetting.Default.PassedColumnStyle, pattern);
            if (!match.Success)
            {
                _passedColumnStyles.Add(".*", GenerationSetting.Default.PassedColumnStyle);
                return;
            }
            while (match.Success)
            {
                string key = string.Format(GenerationSetting.Default.PatternToCheckDirection, match.Groups[1].Value);
                string value = match.Groups[2].Value;
                _passedColumnStyles.Add(key, value);
                match = match.NextMatch();
            }
        }

        private string GetPassedColumnStyle(string courseName)
        {
            return _passedColumnStyles.FirstOrDefault(i => Regex.IsMatch(courseName, i.Key)).Value;
        }

        public void AddItem(User user, Grade grade, Activity activity)
        {
            int testId;
            items.Add(new Item()
            {
                UserId = user.Id,
                FullName = user.FullName,
                Institution = user.Institution,
                Grade = grade.Value == null ? 0 : grade.Value.Value,
                TestName = activity.Name,
                TestId = int.TryParse(activity.Id, out testId) ? testId : int.MaxValue,
                GradePass = activity.GradePass
            });
        }

        public IDictionary<int, double> CalcAVGbyInstitution(string institution)
        {
            var result = (from i in items
                          where (institution == null || i.Institution == institution)
                          orderby i.TestId
                          group i by new
            {
                testId = i.TestId
            }
                              into g
                              select new { Average = g.Average(r => r.Grade), g.Key.testId });
            return result.ToDictionary(g => g.testId, g => g.Average);
        }

        public IDictionary<int, double> CalcAVGbyInstitutions()
        {
            var result1 = (from i in items
                           orderby i.TestId
                           group i by new { i.TestId, i.Institution } into g
                           select new { Average = g.Average(r => r.Grade), g.Key.TestId });
            var result2 = from i in result1
                          where i.Average > 0
                          orderby i.TestId
                          group i by i.TestId into g
                          select new
            {
                Average = g.Average(r => r.Average),
                TestId = g.Key
            };

            return result2.ToDictionary(g => g.TestId, g => g.Average);
        }

        public Dictionary<int, double> CalcProgressInstitution(string institution)
        {
            return (from i in this.items
                    where i.Institution == institution
                    orderby i.TestId
                    group i by i.TestId
                        into gr
                        select new
                {
                    gr.Key,
                    Progress = gr.Count(v => v.Grade >= v.GradePass) / (double)gr.Count()
                }).ToDictionary(i => i.Key, i => i.Progress);
        }

        public StringBuilder GenerateReportTable(ReportInfo reportInfo, Course course, string institution)
        {
            int weekNumber = ((DateTime.Now - reportInfo.StartDate).Days + 1) / 7;
            int cycle = (int)Math.Round(((reportInfo.EndDate - reportInfo.StartDate).Days) / 7.0);
            int nonGradeColumnsCount = _baseColumnHeaders.Length;
            var items = from i in this.items
                        where i.Institution == institution
                        orderby i.FullName, i.TestId
                        group i by new
            {
                userId = i.UserId,
                i.FullName,
                i.Institution
            }
                            into g
                            select new
                            {
                                g.Key.FullName,
                                g.Key.Institution,
                                grades = g.ToDictionary(i => i.TestId, i => new { i.TestName, i.TestId, i.Grade, i.GradePass })
                            };

            var orderedGrades = items.First().grades.Values.ToList();
            orderedGrades.Sort((a, b) =>
            {
                var regexp = new Regex("\\d+");
                string testNumA = regexp.Match(a.TestName).Value;
                string testNumB = regexp.Match(b.TestName).Value;
                if (!string.IsNullOrEmpty(testNumA) && !string.IsNullOrEmpty(testNumB))
                {
                    return int.Parse(testNumA).CompareTo(int.Parse(testNumB));
                }
                if (string.IsNullOrEmpty(testNumA) && !string.IsNullOrEmpty(testNumB))
                {
                    return 1;
                }
                if (!string.IsNullOrEmpty(testNumA) && string.IsNullOrEmpty(testNumB))
                {
                    return -1;
                }
                return a.TestId.CompareTo(b.TestId);
            });

            HTMLTableGenerator table = new HTMLTableGenerator();
            table.Init(GenerationSetting.Default.TableStyle);
            table.OpenColGroup();
            table.AddColumnStyle(nonGradeColumnsCount, string.Empty);

            table.AddColumnStyle(cycle > weekNumber ? weekNumber : orderedGrades.Count, GetPassedColumnStyle(course.ShortName));

            table.CloseColGroup();

            if (!items.Any())
            {
                throw new ReporterException(String.Format("Группа \"{1}\" курса \"{0}\" не имеет пользователей с учреждением \"{2}\".", reportInfo.GroupName, course.ShortName, institution));
            }

            // Заголовки
            table.AddCaption(string.Format(GenerationSetting.Default.TableTitle, weekNumber, course.ShortName), GenerationSetting.Default.CaptionStyle);
            table.AddRow(_baseColumnHeaders.Concat(orderedGrades.Select(a => a.TestName)), GenerationSetting.Default.HeadersColumnsStyle, GenerationSetting.Default.CellStyle);

            // Проходной балл
            table.OpenRow(GenerationSetting.Default.PassedGradeRowStyle);
            table.AddCell(GenerationSetting.Default.PassedGradeRowHeader, GenerationSetting.Default.CellStyle, nonGradeColumnsCount);
            foreach (var item in orderedGrades)
            {
                table.AddCell(item.GradePass.ToString(_numberFormat), GenerationSetting.Default.CellStyle);
            }
            table.CloseRow();

            // Строки с оценками слушателей
            foreach (var i in items)
            {
                var sortedGrade = orderedGrades.Select(g => i.grades.Values.First(vg => vg.TestId == g.TestId));
                table.AddRow(
                    new[] { i.FullName, i.Institution }.Concat(sortedGrade.Select(item => item.Grade.ToString(_numberFormat))),
                    GenerationSetting.Default.GradesRowsStyle,
                    GenerationSetting.Default.CellStyle);
            }

            // Среднее по РИЦ
            table.OpenRow(GenerationSetting.Default.AVGbyInstitutionRowStyle);
            table.AddCell(GenerationSetting.Default.AVGbyInstitutionRowHeader, GenerationSetting.Default.CellStyle, nonGradeColumnsCount);
            var institutionAvg = CalcAVGbyInstitution(institution);
            foreach (var grade in orderedGrades)
            {
                table.AddCell(institutionAvg[grade.TestId].ToString(_numberFormat), GenerationSetting.Default.CellStyle);
            }
            table.CloseRow();

            // Среднее по РИЦам
            table.OpenRow(GenerationSetting.Default.AVGbyInstitutionsRowStyle);
            table.AddCell(GenerationSetting.Default.AVGbyInstitutionsRowHeader, GenerationSetting.Default.CellStyle, nonGradeColumnsCount);
            var commonAvg = CalcAVGbyInstitutions();
            foreach (var grade in orderedGrades)
            {
                table.AddCell((commonAvg.ContainsKey(grade.TestId) ? commonAvg[grade.TestId] : 0).ToString(_numberFormat), GenerationSetting.Default.CellStyle);
            }
            table.CloseRow();

            // Процент успевающих
            table.OpenRow(GenerationSetting.Default.ProgressRowStyle);
            table.AddCell(GenerationSetting.Default.ProgressRowHeader, GenerationSetting.Default.CellStyle, nonGradeColumnsCount);
            var testsProgress = CalcProgressInstitution(institution);
            foreach (var grade in orderedGrades)
            {
                table.AddCell((testsProgress[grade.TestId] * 100).ToString(_numberFormat) + "%", GenerationSetting.Default.CellStyle);
            }
            table.CloseRow();

            return table.Close();
        }

        public void Reset()
        {
            items.Clear();
        }

        private class Item
        {
            internal string FullName
            {
                get;
                set;
            }

            internal double Grade
            {
                get;
                set;
            }

            internal double GradePass
            {
                get;
                set;
            }

            internal string Institution
            {
                get;
                set;
            }

            internal int TestId
            {
                get;
                set;
            }

            internal string TestName
            {
                get;
                set;
            }

            internal string UserId
            {
                get;
                set;
            }
        }
    }
}