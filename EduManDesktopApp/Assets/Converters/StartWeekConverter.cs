using EduManModel.Dtos;
using EduManModel;
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
    public class StartWeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? id = (int?)value;
            if (id == null) return string.Empty;

            DataProcess<DtoStartWeek> dp = new();
            DtoResult<DtoStartWeek> rs = Task.Run(async () => await dp.GetOneAsync(new DtoStartWeek { Id = id })).Result;

            return (rs != null && rs.Result != null) ? rs.Result.OnYear! : null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? name = (string?)value;
            if (string.IsNullOrEmpty(name)) return string.Empty;

            DataProcess<DtoStartWeek> dp = new();
            DtoResult<DtoStartWeek> rs = Task.Run(async () => await dp.GetOneAsync(new DtoStartWeek { OnYear = name })).Result;

            return rs != null && rs.Result != null ? rs.Result.Id! : null!;
        }
    }
}
