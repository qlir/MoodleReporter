namespace ReportsGenerator.TableGenerator
{
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    class HTMLTableGenerator
    {
        private const string CaptionClose = "</caption>";
        private const string CaptionOpen = "<caption style='{0}'>";
        private const string ColgroupClose = "</colgroup>";
        private const string ColgroupOpen = "<colgroup>";
        private const string ColumnStyle = @"<col span='{0}' style='{1}'></colgroup>";
        private const string TableClose = "</table>";
        private const string TableOpen = "<table style='{0}'>";
        private const string TdClose = "</td>";
        private const string TdOpen = "<td style='border-collapse: collapse; border: 2px solid dimgray;{0}' colspan='{1}'>";
        private const string TrClose = "</tr>";
        private const string TrOpen = "<tr style='{0}'>";

        readonly StringBuilder _table = new StringBuilder();

        public void AddCaption(string caption, string style = "")
        {
            _table.AppendFormat(CaptionOpen, style).Append(caption).Append(CaptionClose);
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

        public void AddColumnStyle(int span, string style)
        {
            _table.AppendFormat(ColumnStyle, span, style);
        }
        /*
                public void AddHeaderRow(IEnumerable<string> value)
                {
                    _table.Append(TrOpen);
                    foreach (var v in value)
                    {
                        _table.Append(ThOpen).Append(v).Append(ThClose);
                    }
                    _table.Append(TrClose);
                }*/

        public void AddRow(IEnumerable<string> value, string style = "", string cellStyle = "")
        {
            OpenRow(style);
            foreach (var v in value)
            {
                _table.AppendFormat(TdOpen, cellStyle, 1).Append(v).Append(TdClose);
            }
            CloseRow();
        }

        public StringBuilder Close()
        {
            return new StringBuilder().Append(_table.Append(TableClose));
        }

        public void CloseColGroup()
        {
            _table.Append(ColgroupClose);
        }

        public void CloseRow()
        {
            _table.Append(TrClose);
        }

        public void Init(string style = "")
        {
            _table.Clear();
            _table.AppendFormat(TableOpen, style);
        }

        public void OpenColGroup()
        {
            _table.Append(ColgroupOpen);
        }

        public void OpenRow()
        {
            OpenRow(string.Empty);
        }

        public void OpenRow(string style)
        {
            _table.AppendFormat(TrOpen, style);
        }
    }
}