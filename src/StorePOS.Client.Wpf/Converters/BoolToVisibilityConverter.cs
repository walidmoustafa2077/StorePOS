using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace StorePOS.Client.Wpf.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; }
        public bool CollapseInsteadOfHide { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool b)
                return DependencyProperty.UnsetValue;

            if (Invert)
                b = !b;

            if (b)
                return Visibility.Visible;

            return CollapseInsteadOfHide ? Visibility.Collapsed : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Visibility v)
                return DependencyProperty.UnsetValue;

            return v == Visibility.Visible ? !Invert : Invert;
        }
    }
}
