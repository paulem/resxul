using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Resxul.Framework.Converters
{
    internal class BooleanToVisibilityInverseConverter : IValueConverter
    {
        private readonly BooleanToVisibilityConverter _conv;

        public BooleanToVisibilityInverseConverter()
        {
            _conv = new BooleanToVisibilityConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _conv.Convert(!(bool)value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object obj = _conv.ConvertBack(value, targetType, parameter, culture);
            return obj != null && !(bool)obj;
        }
    }
}
