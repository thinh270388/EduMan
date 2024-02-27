﻿using EduManModel.Dtos;
using EduManModel;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EduManDesktopApp.Assets.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class DisciplineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? id = (int?)value;
            if (id == null) return string.Empty;

            DataProcess<DtoDiscipline> dp = new();
            DtoResult<DtoDiscipline> rs = Task.Run(async () => await dp.GetOneAsync(new DtoDiscipline { Id = id })).Result;

            return (rs != null && rs.Result != null) ? rs.Result.DisciplineName! : null!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? name = (string?)value;
            if (string.IsNullOrEmpty(name)) return string.Empty;

            DataProcess<DtoDiscipline> dp = new();
            DtoResult<DtoDiscipline> rs = Task.Run(async () => await dp.GetOneAsync(new DtoDiscipline { DisciplineName = name })).Result;

            return rs != null && rs.Result != null ? rs.Result.Id! : null!;
        }
    }
}
