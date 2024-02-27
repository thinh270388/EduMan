using EduManModel.Dtos;
using EduManModel;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EduManDesktopApp.Assets.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class DisciplineGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? id = (int?)value;
            if (id == null) return string.Empty;

            DataProcess<DtoDisciplineGroup> dp = new();
            DtoResult<DtoDisciplineGroup> rs = Task.Run(async () => await dp.GetOneAsync(new DtoDisciplineGroup { Id = id })).Result;

            return (rs != null && rs.Result != null) ? rs.Result.DisciplineGroupName! : null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? name = (string?)value;
            if (string.IsNullOrEmpty(name)) return string.Empty;

            DataProcess<DtoDisciplineGroup> dp = new();
            DtoResult<DtoDisciplineGroup> rs = Task.Run(async () => await dp.GetOneAsync(new DtoDisciplineGroup { DisciplineGroupName = name })).Result;

            return rs != null && rs.Result != null ? rs.Result.Id! : null!;
        }
    }
}
