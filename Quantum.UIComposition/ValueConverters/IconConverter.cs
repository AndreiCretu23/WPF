using Quantum.Utils;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Quantum.ValueConverters
{
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null) {
                return IconUtils.GetResourceIcon(value.ToString());
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
