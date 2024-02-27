using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EduManDesktopApp.Assets.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class ApplyForConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? id = (int?)value;
            return id == 1 ? "Lớp học" : id == 2 ? "Học sinh" : "Cả hai";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? txt = (string?)value;
            return txt!.Equals("Lớp học") ? 1 : txt.Equals("Học sinh") ? 2 : 0;
        }
    }
}
