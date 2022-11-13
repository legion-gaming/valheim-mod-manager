using System;
using System.Globalization;

namespace ValheimModManager.Core.Converters
{
    public class GreaterThanConverter : ValueConverterBase<int, bool, int>
    {
        public override bool Convert(int value, int parameter, CultureInfo culture)
        {
            return value > parameter;
        }

        public override int Convert(bool value, int parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
