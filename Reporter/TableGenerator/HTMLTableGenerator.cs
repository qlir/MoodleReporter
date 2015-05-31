using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

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
        private const string ColgroupOpen = "<colgroup>";
        private const string ColgroupClose = "</colgroup>";
        private const string ColumnStyle = @"<col span='{0}' style='{1}'></colgroup>";

        readonly StringBuilder _table = new StringBuilder();

        public void Init()
        {
            _table.Clear();
            _table.Append(TableOpen);
        }

        public void OpenColGroup()
        {
            _table.Append(ColgroupOpen);
        }

        public void CloseColGroup()
        {
            _table.Append(ColgroupClose);
        }

        public void AddColumnStyle(int span, string style)
        {
            _table.AppendFormat(ColumnStyle, span, style);
        }

        public StringBuilder Close()
        {
            return new StringBuilder().Append(_table.Append(TableClose));
        }

        public void AddCaption(string caption)
        {
            _table.Append(CaptionOpen).Append(caption).Append(CaptionClose);
        }

        public void AddCell(string value, string style, int columnSpan)
        {
            _table.AppendFormat(TdOpen, style, columnSpan).Append(value).Append(TdClose);
        }
        public void AddCell(string value, string style)
        {
            AddCell(value, style, 1);
        }

        public void AddCell(string value)
        {
            AddCell(value, string.Empty, 1);
        }

        public void AddCell(string value, int columnSpan)
        {
            AddCell(value, string.Empty, columnSpan);
        }

        public void OpenRow()
        {
            OpenRow(string.Empty);
        }
        public void OpenRow(string style)
        {
            _table.AppendFormat(TrOpen, style);
        }

        public void CloseRow()
        {
            _table.Append(TrClose);
        }

        public void AddHeaderRow(IEnumerable<string> value)
        {
            _table.Append(TrOpen);
            foreach (var v in value)
            {
                _table.Append(ThOpen).Append(v).Append(ThClose);
            }
            _table.Append(TrClose);
        }

        public void AddRow(IEnumerable<string> value)
        {
            OpenRow();
            foreach (var v in value)
            {
                _table.AppendFormat(TdOpen, string.Empty, 1).Append(v).Append(TdClose);
            }
            CloseRow();
        }
    }
}
