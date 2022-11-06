using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ValheimModManager.Core.Converters
{
    public class IntToRangeConverter : IValueConverter // Todo:
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return Enumerable.Range(1, intValue);
            }

            return Enumerable.Empty<int>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<int> range)
            {
                return range.Max();
            }

            return 0;
        }
    }
}
