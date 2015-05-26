using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportsGenerator.DataStructures;

namespace ReportsGenerator.TableGenerator
{
    public class ReportTableGenerator
    {
        private static readonly int NumberToRound = Settings.Default.AccuracyGrades;
        private List<Item> items = new List<Item>();

        public void reset()
        {
            items.Clear();
        }

        public void addItem(User user, Grade grade, Activity activity)
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

        public IDictionary<int, double> calcAVG(string institution)
        {
            var result = (from i in items
                          where (institution == null ? true : i.Institution == institution)
                          orderby i.TestId
                          group i by new { testId = i.TestId } into g
                          select new { Average = g.Average(r => r.Grade), g.Key.testId });
            return result.ToDictionary(g => g.testId, g => g.Average);
        }

        public IDictionary<int, double> calcAVG()
        {
            var result1 = (from i in items
                           orderby i.TestId
                           group i by new { i.TestId, i.Institution } into g
                           select new { Average = g.Average(r => r.Grade), g.Key.TestId });
            var result2 = from i in result1
                          group i by i.TestId into g
                          select new { Average = g.Average(r => r.Average), TestId = g.Key };

            return result2.ToDictionary(g => g.TestId, g => g.Average);
        }

        public StringBuilder GenerateReportTable(ReportInfo reportInfo, Course course, string institution)
        {
            int nonGradeColumnsCount = 2;
            HTMLTableGenerator table = new HTMLTableGenerator();
            table.init();
            var items = from i in this.items
                        where i.Institution == institution
                        orderby i.FullName, i.TestId
                        group i by new
                        {
                            userId = i.UserId,
                            i.FullName,
                            i.Institution
                        } into g
                        select new
                        {
                            g.Key.FullName,
                            g.Key.Institution,
                            grades = g.ToDictionary(i => i.TestId, i => new { i.TestName, i.TestId, i.Grade, i.GradePass })
                        };
            if (!items.Any()) throw new ReporterException(String.Format("Группа \"{1}\" курса \"{0}\" не имеет пользователей с учреждением \"{2}\".", reportInfo.GroupName, course.ShortName, institution));
            int weekNumber = ((DateTime.Now - reportInfo.StartDate).Days + 1) / 7;
            table.addCaption("Результаты " + weekNumber + "-й недели обучения по курсу \"" + course.ShortName + "\"");
            table.addHeaderRow(new[] { "ФИО", "Учреждение  (Организация)" }.Concat(items.First().grades.Values.Select(i => i.TestName)));

            var gradesPasses = items.First().grades.Values.Select(g => g.GradePass);
            table.openRow("color:brown;font-weight:bold;");
            table.addCell("Проходной балл", nonGradeColumnsCount);
            foreach (var gradePass in gradesPasses)
            {
                table.addCell(Math.Round(gradePass, NumberToRound).ToString());
            }
            table.closeRow();

            foreach (var i in items)
            {
                table.addRow(new string[] { i.FullName, i.Institution }.Concat(i.grades.Values.Select(item => Math.Round(item.Grade, NumberToRound).ToString())));
            }

            table.openRow("font-weight:bold;");
            table.addCell("Среднее по РИЦ", nonGradeColumnsCount);
            foreach (var grade in calcAVG(institution).Values)
            {
                table.addCell(Math.Round(grade, NumberToRound).ToString());
            }
            table.closeRow();

            table.openRow( "font-weight:bold;");
            table.addCell("Среднее по всем РИЦам",nonGradeColumnsCount);
            foreach (var grade in calcAVG().Values)
            {
                table.addCell(Math.Round(grade, NumberToRound).ToString());
            }
            table.closeRow();

            var progress = from i in this.items
                           where i.Institution == institution
                           orderby i.TestId
                           group i by i.TestId into gr
                           select gr.Count(v => v.Grade >= v.GradePass) / (double)gr.Count();
            table.openRow("font-weight:bold;");
            table.addCell("Процент успевающих", nonGradeColumnsCount);
            foreach (var value in progress)
            {
                table.addCell(Math.Round(value * 100) + "%");
            }
            table.closeRow();

            return table.close();
        }

        private class Item
        {
            internal string Institution { get; set; }
            internal double Grade { get; set; }
            internal string TestName { get; set; }
            internal int TestId { get; set; }
            internal string UserId { get; set; }
            internal double GradePass { get; set; }
            internal string FullName { get; set; }
        }
    }
}
