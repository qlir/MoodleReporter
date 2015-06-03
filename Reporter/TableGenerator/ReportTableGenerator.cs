namespace ReportsGenerator.TableGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms.VisualStyles;

    using ReportsGenerator.DataStructures;

    public class ReportTableGenerator
    {
        private static readonly int NumberToRound = Settings.Default.AccuracyGrades;

        private List<Item> items = new List<Item>();

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
                          where (institution == null ? true : i.Institution == institution)
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
                Average = g.Average(r => r.Average), TestId = g.Key
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
            const int nonGradeColumnsCount = 2;
            const int cycle = 4;
            const string passedColumnStyle = "background-color:#ffeeee";
            int weekNumber = ((DateTime.Now - reportInfo.StartDate).Days + 1) / 7;
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
            table.Init();
            table.OpenColGroup();
            table.AddColumnStyle(nonGradeColumnsCount, string.Empty);
            table.AddColumnStyle(cycle > weekNumber ? weekNumber : orderedGrades.Count, passedColumnStyle);
            table.CloseColGroup();

            if (!items.Any())
            {
                throw new ReporterException(String.Format("Группа \"{1}\" курса \"{0}\" не имеет пользователей с учреждением \"{2}\".", reportInfo.GroupName, course.ShortName, institution));
            }
            table.AddCaption("Результаты " + weekNumber + "-й недели обучения по курсу \"" + course.ShortName + "\"");
            table.AddHeaderRow(new[] { "ФИО", "Учреждение  (Организация)" } .Concat(orderedGrades.Select(a => a.TestName)));

            table.OpenRow("color:brown;font-weight:bold;");
            table.AddCell("Проходной балл", nonGradeColumnsCount);
            const string numberFormat = "0.00";
            foreach (var item in orderedGrades)
            {
                table.AddCell(Math.Round(item.GradePass, NumberToRound).ToString(numberFormat));
            }
            table.CloseRow();

            foreach (var i in items)
            {
                var sortedGrade = orderedGrades.Select(g => i.grades.Values.First(vg => vg.TestId == g.TestId));
                table.AddRow(new[] { i.FullName, i.Institution } .Concat(
                                 sortedGrade.Select(item => Math.Round(item.Grade, NumberToRound).ToString(numberFormat))));
            }

            table.OpenRow("font-weight:bold;");
            table.AddCell("Среднее по РИЦ", nonGradeColumnsCount);
            var institutionAvg = CalcAVGbyInstitution(institution);
            foreach (var grade in orderedGrades)
            {
                table.AddCell(Math.Round(institutionAvg[grade.TestId], NumberToRound).ToString(numberFormat));
            }
            table.CloseRow();

            table.OpenRow("font-weight:bold;color:red;");
            table.AddCell("Среднее по всем РИЦам", nonGradeColumnsCount);
            var commonAvg = CalcAVGbyInstitutions();
            foreach (var grade in orderedGrades)
            {
                table.AddCell(Math.Round(commonAvg[grade.TestId], NumberToRound).ToString(numberFormat));
            }
            table.CloseRow();

            table.OpenRow("font-weight:bold;");
            table.AddCell("Процент успевающих", nonGradeColumnsCount);
            var testsProgress = CalcProgressInstitution(institution);
            foreach (var grade in orderedGrades)
            {
                table.AddCell(Math.Round(testsProgress[grade.TestId] * 100, NumberToRound).ToString(numberFormat) + "%");
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