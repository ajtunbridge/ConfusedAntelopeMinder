using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ngen.Central.Converters
{
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class FilenameToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fileName = value as string;

            var icon = IconManager.FindIconForFilename(fileName, true);

            if (icon == null)
            {
                // TODO: get default icon if no icon found for file type
            }

            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
