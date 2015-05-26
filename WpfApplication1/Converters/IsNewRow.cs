using System;
using System.Windows.Data;
using ReportsGenerator.DataStructures;

namespace UIReporter.Converters
{
    public class IsNewRow : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value != null && value.GetType().FullName.Equals("MS.Internal.NamedObject");
        }


        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}