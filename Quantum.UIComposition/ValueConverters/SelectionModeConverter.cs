using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Quantum.ValueConverters
{
    public class SelectionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((bool)value) {
                return SelectionMode.Extended;
            }

            return SelectionMode.Single;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !((SelectionMode)value == SelectionMode.Single);
        }
    }
}
