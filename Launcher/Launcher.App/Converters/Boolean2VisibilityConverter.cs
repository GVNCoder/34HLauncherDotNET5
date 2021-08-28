// ReSharper disable MergeCastWithTypeCheck
// ReSharper disable ConditionIsAlwaysTrueOrFalse

using System;
using System.Windows;
using System.Windows.Data;

namespace Launcher.Converters
{
    public class Boolean2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var bValue = false;

            if (value is bool)
            {
                bValue = (bool) value;
            }
            else if (value is bool?)
            {
                var tmp = (bool?) value;
                bValue = tmp ?? false;
            }

            return (bValue) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}