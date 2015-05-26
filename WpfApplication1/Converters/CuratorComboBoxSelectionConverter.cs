using System;
using System.Windows.Data;
using ReportsGenerator.DataStructures;

namespace UIReporter.Converters
{
    public class CuratorComboBoxSelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is ReportInfo)
            {
                ReportInfo ri = (ReportInfo) value;

                switch (ri.CuratorsGenerationType)
                {
                    case ReportInfo.CuratorsGenerationTypeEnum.MoodleCurators:
                        return MainWindow.MoodleCuratorString;
                    case ReportInfo.CuratorsGenerationTypeEnum.All:
                        return MainWindow.AllCuratorString;
                    case ReportInfo.CuratorsGenerationTypeEnum.Custom:
                        return ri.CuratorsEmail;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return "";
        }
    }
}
