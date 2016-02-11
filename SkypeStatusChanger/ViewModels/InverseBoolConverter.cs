using System;
using System.Globalization;
using System.Windows.Data;

namespace SkypeStatusChanger.ViewModels
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (targetType != typeof(bool))
            //    throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
            //return (object)(bool)(!(bool)value ? 1 : 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
            //return (object)(bool)(!(bool)value ? 1 : 0);
        }
    }
}
