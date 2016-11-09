using System;
using System.Globalization;
using System.Windows.Data;

namespace Arkivverket.Arkade.UI.Util
{
    public class RadioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? parameter : null;
        }
    }
}
