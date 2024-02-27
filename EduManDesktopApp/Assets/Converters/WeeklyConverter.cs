using EduManModel.Dtos;
using EduManModel;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EduManDesktopApp.Assets.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class WeeklyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? id = (int?)value;
            if (id == null) return string.Empty;

            DataProcess<DtoWeekly> dp = new();
            DtoResult<DtoWeekly> rs = Task.Run(async () => await dp.GetOneAsync(new DtoWeekly { Id = id })).Result;

            return (rs != null && rs.Result != null) ? rs.Result.WeeklyName! : null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? name = (string?)value;
            if (string.IsNullOrEmpty(name)) return string.Empty;

            DataProcess<DtoWeekly> dp = new();
            DtoResult<DtoWeekly> rs = Task.Run(async () => await dp.GetOneAsync(new DtoWeekly { WeeklyName = name })).Result;

            return rs != null && rs.Result != null ? rs.Result.Id! : null!;
        }
    }
}
