using System;
using System.Globalization;
using System.Windows.Data;

namespace StorePOS.Client.Wpf.Converters
{
    /// <summary>
    /// Converts an ItemsControl AlternationIndex to a display number (adds 1)
    /// </summary>
    public class IndexToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return (index + 1).ToString();
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
