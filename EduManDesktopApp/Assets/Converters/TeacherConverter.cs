using EduManModel.Dtos;
using EduManModel;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EduManDesktopApp.Assets.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class TeacherConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? id = (int?)value;
            if (id == null) return string.Empty;

            DataProcess<DtoTeacher> dp = new();
            DtoResult<DtoTeacher> rs = Task.Run(async () => await dp.GetOneAsync(new DtoTeacher { Id = id })).Result;

            return (rs != null && rs.Result != null) ? rs.Result.FullName! : null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? name = (string?)value;
            if (string.IsNullOrEmpty(name)) return string.Empty;

            DataProcess<DtoTeacher> dp = new();
            DtoResult<DtoTeacher> rs = Task.Run(async () => await dp.GetOneAsync(new DtoTeacher { FullName = name })).Result;

            return rs != null && rs.Result != null ? rs.Result.Id! : null!;
        }
    }
}
