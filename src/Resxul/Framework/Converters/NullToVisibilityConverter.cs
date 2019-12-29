﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Resxul.Framework.Converters
{
    internal class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            string stringValue = value as string;

            return stringValue != null
                ? (string.IsNullOrEmpty(stringValue) ? Visibility.Collapsed : Visibility.Visible)
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
