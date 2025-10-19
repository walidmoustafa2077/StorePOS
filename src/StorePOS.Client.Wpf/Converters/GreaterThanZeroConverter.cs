using System;
using System.Globalization;
using System.Windows.Data;

namespace StorePOS.Client.Wpf.Converters
{
    /// <summary>
    /// Converts a numeric value to a boolean indicating if it's greater than zero
    /// </summary>
    public class GreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            if (value is int intValue)
                return intValue > 0;

            if (value is decimal decimalValue)
                return decimalValue > 0;

            if (value is double doubleValue)
                return doubleValue > 0;

            if (value is float floatValue)
                return floatValue > 0;

            if (value is long longValue)
                return longValue > 0;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
