using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace WPF_Calendar_With_Notes.Utilities.Converters
{
    class ConverterOfMinute : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int val;
            var b = int.TryParse(value.ToString(), out val);

            if (!b)
            {
                //MessageBox.Show("This is not proper minute");
                return 0;
            }

            if (val < 0 || val > 59)
            {
                MessageBox.Show("This is not proper minute");
                return 0;
            }

            return val;
        }
    }
}
