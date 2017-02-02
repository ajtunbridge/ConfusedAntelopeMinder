using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ngen.Central.Converters
{
    [ValueConversion(typeof(string), typeof(DateTime?))]
    public class StringToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateString = (string) value;

            DateTime parsedDateTime;

            if (DateTime.TryParse(dateString, out parsedDateTime))
            {
                return parsedDateTime;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = value as DateTime?;

            return dt == null ? "Date conversion error" : dt.Value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
