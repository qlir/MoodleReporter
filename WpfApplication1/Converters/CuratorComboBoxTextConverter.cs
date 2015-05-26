using System;
using System.Windows.Data;
using ReportsGenerator.DataStructures;

namespace UIReporter.Converters
{
    public class CuratorComboBoxTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is string) return value;
            if (value is Curator)
            {
                Curator c = ((Curator)value);
                return string.Format("{0} {1}| {2}", c.Institution, c.City == null ? String.Empty : "| " + c.City, c.FullName);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
