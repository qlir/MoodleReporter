using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ReportsGenerator.TableGenerator
{
    class HTMLTableGenerator
    {
        private const string TableOpen = "<table style=' margin: 10px; border-collapse: collapse; border: 2px solid dimgray;'>";
        private const string TableClose = "</table>";
        private const string CaptionOpen = "<caption>";
        private const string CaptionClose = "</caption>";
        private const string TrOpen = "<tr style='{0}'>";
        private const string TrClose = "</tr>";
        private const string ThOpen = "<th style='border-collapse: collapse;padding:5px 5px; word-wrap: break-word; border: 2px solid dimgray;'>";
        private const string ThClose = "</th>";
        private const string TdOpen = "<td style='border-collapse: collapse;padding:5px 5px; border: 2px solid dimgray;{0}' colspan='{1}'>";
        private const string TdClose = "</td>";

        StringBuilder table = new StringBuilder();

        public void init()
        {
            table.Clear();
            table.Append(TableOpen);
        }

        public StringBuilder close()
        {
            return new StringBuilder().Append(table.Append(TableClose));
        }

        public void addCaption(string caption)
        {
            table.Append(CaptionOpen).Append(caption).Append(CaptionClose);
        }

        public void addCell(string value, string style, int columnSpan)
        {
            table.AppendFormat(TdOpen, style, columnSpan).Append(value).Append(TdClose);
        }
        public void addCell(string value, string style)
        {
            addCell(value, style, 1);
        }

        public void addCell(string value)
        {
            addCell(value, string.Empty, 1);
        }

        public void addCell(string value, int columnSpan)
        {
            addCell(value, string.Empty, columnSpan);
        }

        public void openRow()
        {
            openRow(string.Empty);
        }
        public void openRow(string style)
        {
            table.AppendFormat(TrOpen, style);
        }

        public void closeRow()
        {
            table.Append(TrClose);
        }

        public void addHeaderRow(IEnumerable<string> value)
        {
            table.Append(TrOpen);
            foreach (var v in value)
            {
                table.Append(ThOpen).Append(v).Append(ThClose);
            }
            table.Append(TrClose);
        }

        public void addRow(IEnumerable<string> value)
        {
            openRow();
            foreach (var v in value)
            {
                table.AppendFormat(TdOpen, string.Empty, 1).Append(v).Append(TdClose);
            }
            closeRow();
        }
    }
}
