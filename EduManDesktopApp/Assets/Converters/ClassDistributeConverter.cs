using EduManModel.Dtos;
using EduManModel;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EduManDesktopApp.Assets.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class ClassDistributeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? id = (int?)value;
            if (id == null) return string.Empty;

            DataProcess<DtoClassDistribute> dp = new();
            DtoResult<DtoClassDistribute> rs = Task.Run(async () => await dp.GetOneAsync(new DtoClassDistribute { Id = id })).Result;

            int? idClass =  (rs != null && rs.Result != null) ? rs.Result.ClassId! : null!;

            DataProcess<DtoClass> dpClass = new();
            DtoResult<DtoClass> rsClass = Task.Run(async () => await dpClass.GetOneAsync(new DtoClass { Id = idClass })).Result;

            return (rsClass != null && rsClass.Result != null) ? rsClass.Result.ClassName! : null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null!;
        }
    }
}
