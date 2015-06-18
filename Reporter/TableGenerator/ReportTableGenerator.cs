using System.CodeDom;
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

        public IDictionary<int, double> CalcMaxByInstitutions()
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
                Average = g.Max(r => r.Average),
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
            int weekNumber = Reporter.GetWeekNumber(reportInfo);
            int cycle = Reporter.GetWeeksCount(reportInfo);
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

            if (!items.Any())
            {
                throw new ReporterException(String.Format("Группа \"{0}\" курса \"{1}\" не имеет пользователей с учреждением \"{2}\".", reportInfo.GroupName, course.ShortName, institution));
            }

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
            var passedColumnStyle = GetPassedColumnStyle(course.ShortName);
            table.AddColumnStyle(cycle > weekNumber ? weekNumber : orderedGrades.Count, passedColumnStyle);

            table.CloseColGroup();

            // Заголовки
            table.AddCaption(string.Format(GenerationSetting.Default.TableTitle, weekNumber, course.ShortName), GenerationSetting.Default.CaptionStyle);
            // table.AddRow(_baseColumnHeaders.Concat(orderedGrades.Select(a => a.TestName)), , );
            table.OpenRow(GenerationSetting.Default.HeadersColumnsStyle);
            foreach (var i in _baseColumnHeaders)
            {
                table.AddCell(i, GenerationSetting.Default.CellStyle);
            }
            for (int i = 0; i < orderedGrades.Count; i++)
            {
                var name = orderedGrades[i].TestName;
                table.AddCell(name, (i < weekNumber ? GenerationSetting.Default.CellStyle + passedColumnStyle : GenerationSetting.Default.CellStyle));
            }
            table.CloseRow();

            // Проходной балл
            table.OpenRow(GenerationSetting.Default.PassedGradeRowStyle);
            table.AddCell(GenerationSetting.Default.PassedGradeRowHeader, GenerationSetting.Default.CellStyle, nonGradeColumnsCount);
            for (int i = 0; i < orderedGrades.Count; i++)
            {
                var item = orderedGrades[i];
                table.AddCell(item.GradePass.ToString(_numberFormat), (i < weekNumber ? GenerationSetting.Default.CellStyle + passedColumnStyle : GenerationSetting.Default.CellStyle));
            }
            table.CloseRow();

            // Строки с оценками слушателей
            foreach (var i in items)
            {
                var sortedGrade = orderedGrades.Select(g => i.grades.Values.First(vg => vg.TestId == g.TestId));

                var grades = sortedGrade.Select(item => item.Grade).ToList();
                var additionalStyle = string.Empty;

                // Определение слушателей с балом равным 0
                for (int ii = 0; ii < weekNumber; ii++)
                {
                    if (grades[ii] == 0)
                    {
                        additionalStyle = GenerationSetting.Default.BadGradeStyle;
                        break;
                    }
                }

                table.OpenRow(GenerationSetting.Default.GradesRowsStyle);
                table.AddCell(i.FullName, GenerationSetting.Default.CellStyle + additionalStyle);
                table.AddCell(i.Institution, GenerationSetting.Default.CellStyle + additionalStyle);
                for (int ii = 0; ii < grades.Count; ii++)
                {
                    table.AddCell(grades[ii].ToString(_numberFormat), (ii < weekNumber ? GenerationSetting.Default.CellStyle + passedColumnStyle + additionalStyle : GenerationSetting.Default.CellStyle));
                }
            }

            // Среднее по РИЦ
            table.OpenRow(GenerationSetting.Default.AVGbyInstitutionRowStyle);
            table.AddCell(GenerationSetting.Default.AVGbyInstitutionRowHeader, GenerationSetting.Default.CellStyle, nonGradeColumnsCount);
            var institutionAvg = CalcAVGbyInstitution(institution);
            for (int i = 0; i < orderedGrades.Count; i++)
            {
                var grade = orderedGrades[i];
                table.AddCell(institutionAvg[grade.TestId].ToString(_numberFormat), (i < weekNumber ? GenerationSetting.Default.CellStyle + passedColumnStyle : GenerationSetting.Default.CellStyle));
            }
            table.CloseRow();

            // Среднее по РИЦам
            table.OpenRow(GenerationSetting.Default.AVGbyInstitutionsRowStyle);
            table.AddCell(GenerationSetting.Default.AVGbyInstitutionsRowHeader, GenerationSetting.Default.CellStyle, nonGradeColumnsCount);
            var commonAvg = CalcAVGbyInstitutions();
            for (int i = 0; i < orderedGrades.Count; i++)
            {
                var grade = orderedGrades[i];
                table.AddCell(
                    (commonAvg.ContainsKey(grade.TestId) ? commonAvg[grade.TestId] : 0).ToString(_numberFormat),
                    (i < weekNumber ? GenerationSetting.Default.CellStyle + passedColumnStyle : GenerationSetting.Default.CellStyle));
            }
            table.CloseRow();

            // Максимальный балл по РИЦам
            table.OpenRow(GenerationSetting.Default.MaxByInstitutionsRowStyle);
            table.AddCell(GenerationSetting.Default.MaxByInstitutionsRowHeader, GenerationSetting.Default.CellStyle, nonGradeColumnsCount);
            var commonMax = CalcMaxByInstitutions();
            for (int i = 0; i < orderedGrades.Count; i++)
            {
                var grade = orderedGrades[i];
                table.AddCell(
                    (commonMax.ContainsKey(grade.TestId) ? commonMax[grade.TestId] : 0).ToString(_numberFormat),
                    (i < weekNumber ? GenerationSetting.Default.CellStyle + passedColumnStyle : GenerationSetting.Default.CellStyle));
            }
            table.CloseRow();

            // Процент успевающих
            table.OpenRow(GenerationSetting.Default.ProgressRowStyle);
            table.AddCell(GenerationSetting.Default.ProgressRowHeader, GenerationSetting.Default.CellStyle, nonGradeColumnsCount);
            var testsProgress = CalcProgressInstitution(institution);
            for (int i = 0; i < orderedGrades.Count; i++)
            {
                var grade = orderedGrades[i];
                table.AddCell((testsProgress[grade.TestId] * 100).ToString(_numberFormat) + "%",
                    (i < weekNumber ? GenerationSetting.Default.CellStyle + passedColumnStyle : GenerationSetting.Default.CellStyle));
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