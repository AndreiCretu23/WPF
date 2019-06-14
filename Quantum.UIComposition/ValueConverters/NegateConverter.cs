using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Quantum.ValueConverters
{
    public class NegateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!(value is bool booleanValue)) {
                throw new Exception("A negate converter can only be used for boolean values.");
            }

            return !booleanValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool booleanValue)) {
                throw new Exception("A negate converter can only be used for boolean values.");
            }

            return !booleanValue;
        }
    }
}
