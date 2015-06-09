using System;
using System.Windows.Data;

namespace UIReporter.Converters
{
    public class ObjectDataProviderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value is ObjectDataProvider ? ((ObjectDataProvider)value).Data : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value is ObjectDataProvider ? ((ObjectDataProvider)value).Data : value;
        }
    }
}
