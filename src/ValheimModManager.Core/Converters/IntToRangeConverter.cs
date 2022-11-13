using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ValheimModManager.Core.Converters
{
    public class IntToRangeConverter : ValueConverterBase<int, IEnumerable<int>>
    {
        public override IEnumerable<int> Convert(int value, object parameter, CultureInfo culture)
        {
            return Enumerable.Range(1, value);
        }

        public override int Convert(IEnumerable<int> value, object parameter, CultureInfo culture)
        {
            return value.Max();
        }
    }
}
