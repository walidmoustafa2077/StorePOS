using System;
using System.Globalization;
using System.Windows.Data;

namespace StorePOS.Client.Wpf.Converters
{
    /// <summary>
    /// Converter that calculates the number of columns for a UniformGrid based on available width.
    /// Provides responsive layout by adjusting columns based on container size.
    /// </summary>
    public class ResponsiveColumnsConverter : IValueConverter
    {
        /// <summary>
        /// Minimum column width for product cards (including margins)
        /// </summary>
        public double MinColumnWidth { get; set; } = 280; // Product card + margin

        /// <summary>
        /// Minimum number of columns to display
        /// </summary>
        public int MinColumns { get; set; } = 4;

        /// <summary>
        /// Maximum number of columns to display
        /// </summary>
        public int MaxColumns { get; set; } = 6;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width && width > 0)
            {
                // Calculate number of columns based on available width
                int columns = (int)Math.Floor(width / MinColumnWidth);
                
                // Ensure we stay within min/max bounds
                columns = Math.Max(MinColumns, Math.Min(MaxColumns, columns));
                
                return columns;
            }

            // Default fallback
            return 4;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}