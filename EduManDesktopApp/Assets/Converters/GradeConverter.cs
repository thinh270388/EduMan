using EduManModel.Dtos;
using EduManModel;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EduManDesktopApp.Assets.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class GradeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? id = (int?)value;
            if (id == null) return string.Empty;

            DataProcess<DtoGrade> dp = new();
            DtoResult<DtoGrade> rs = Task.Run(async () => await dp.GetOneAsync(new DtoGrade { Id = id })).Result;

            return (rs != null && rs.Result != null) ? rs.Result.GradeName! : null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? name = (string?)value;
            if (string.IsNullOrEmpty(name)) return string.Empty;

            DataProcess<DtoGrade> dp = new();
            DtoResult<DtoGrade> rs = Task.Run(async () => await dp.GetOneAsync(new DtoGrade { GradeName = name })).Result;

            return rs != null && rs.Result != null ? rs.Result.Id! : null!;
        }
    }
}
