using System;
using System.Globalization;
using System.Windows.Data;

namespace ValheimModManager.Core.Converters
{
    public abstract class ValueConverterBase<TIn, TOut, TParam> : IValueConverter
    {
        public abstract TOut Convert(TIn value, TParam parameter, CultureInfo culture);
        public abstract TIn Convert(TOut value, TParam parameter, CultureInfo culture);

        public TOut Convert(TIn value, TParam parameter)
        {
            return Convert(value, parameter, CultureInfo.CurrentCulture);
        }

        public TOut Convert(TIn value, CultureInfo culture)
        {
            return Convert(value, default, culture);
        }

        public TOut Convert(TIn value)
        {
            return Convert(value, default, CultureInfo.CurrentCulture);
        }

        public TIn Convert(TOut value, TParam parameter)
        {
            return Convert(value, parameter, CultureInfo.CurrentCulture);
        }

        public TIn Convert(TOut value, CultureInfo culture)
        {
            return Convert(value, default, culture);
        }

        public TIn Convert(TOut value)
        {
            return Convert(value, default, CultureInfo.CurrentCulture);
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((TIn)value, (TParam)parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((TOut)value, (TParam)parameter, culture);
        }
    }

    public abstract class ValueConverterBase<TIn, TOut> : ValueConverterBase<TIn, TOut, object>
    {
    }
}
