using EduManModel.Dtos;
using EduManModel;
using ExcelDataReader;
using Microsoft.Win32;
using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SLHelper;
using System.ComponentModel;
using Notification.Wpf;

namespace EduManDesktopApp.ViewModel
{
    public class ImportDataViewModel : ViewModelBase
    {
        //Important note:
        //1. Add nuget ExcelDataReader
        //2. Add nuget ExcelDataReader.Dataset
        //3. Add nuget System.Text.Encoding.CodePages

        public ImportDataViewModel() 
        {
            //this line is very important when you use Net Core
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        #region Methods
        public enum ListImport
        {
            [Description("CapHoc")]
            Level = 0,
            [Description("KhoiLop")]
            Grade = 1,
            [Description("LopHoc")]
            Class = 2,
            [Description("NhanSu")]
            Teacher = 3,
            [Description("HocSinh")]
            Student = 4,
            [Description("KieuNenNep")]
            DisciplineType = 5,
            [Description("NhomNenNep")]
            DisciplineGroup = 6,
            [Description("NenNep")]
            Discipline = 7,
            [Description("NamHoc")]
            StartWeek = 8,
            [Description("TuanHoc")]
            Weekly = 9,
            [Description("PhanBoLopHoc")]
            PhanBoLopHoc = 10,
            [Description("PhanBoHocSinh")]
            PhanBoHocSinh = 11
        }
        public static string GetDescription(Enum value)
        {
            DescriptionAttribute[] customAttributes = (DescriptionAttribute[])value.GetType().GetField(value.ToString())!.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (customAttributes.Length > 0) ? customAttributes[0].Description : value.ToString();
        }
        private void ShowErrorUnique(Exception ex, string sheetName, int primaryKey)
        {
            if (ex.Message.Contains("contains non-unique values"))
                MessageBox.Show($"Sheet [{sheetName}] có cột [{primaryKey}] trùng khóa", "Thông báo");
            else
                MessageBox.Show($"Sheet excel [{sheetName}] dùng để import không hợp lệ", "Thông báo");
        }
        private void ShowErrorDetail(int indexRow, string indexColumnName)
        {
            MessageBox.Show($"{indexColumnName} rỗng hoặc không tồn tại.\n" +
                            $"Dòng lỗi: [{indexRow}], cột lỗi: [{indexColumnName}].\n" +
                            $"Vui lòng kiểm tra dữ liệu trước khi import!", "Thông báo");
        }
        private void ShowErrowImport(Exception ex, string sheetName, int indexRow, string indexColumnName)
        {
            if (ex.Message.Contains("Object reference not set to an instance of an object")) { MessageBox.Show("Mất kết nối với máy chủ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error); }
            else MessageBox.Show($"Sheet excel [{sheetName}] dùng để import không hợp lệ.\n" +
                                 $"Vui lòng kiểm tra dữ liệu trước khi import!\n" +
                                 $"Dòng lỗi: [{indexRow}], cột lỗi [{indexColumnName}].\n" +
                                 $"Chi tiết lỗi: {ex.Message}.", "Thông báo");
        }
        private void ShowSuccessImport(string sheetName, int iCount, int total, int countAdd, int countUpdate)
        {
            MessageBox.Show($"Đã import {iCount}/{total} [{sheetName}]!\n" +
                            $"--- Thêm mới: {countAdd}\n" +
                            $"--- Cập nhật: {countUpdate}", "Thông báo");
        }
        private ExcelResult ReadExcel(string path, bool HDR)
        {
            try
            {
                using FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader;
                reader = ExcelReaderFactory.CreateReader(stream);
                var config = new ExcelDataSetConfiguration
                {
                    UseColumnDataType = true, //this line to determine that you can get Column DataType
                    ConfigureDataTable = cmd => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = HDR, //HDR = Header Row => determine that excel file contain Header Row or not
                    }
                };
                var dataset = reader.AsDataSet(config);
                string sheetName = dataset.Tables[0].TableName;
                ExcelResult rs = new();
                foreach (DataTable tb in dataset.Tables)
                {
                    rs.Tables.Add(tb);
                    rs.Sheets.Add(tb.TableName);
                    //List<FieldAttribute> attribs = new();
                    //foreach (DataColumn col in tb.Columns)
                    //{
                    //    FieldAttribute fieldAttribute = new()
                    //    {
                    //        FieldName = col.ColumnName,
                    //        FieldType = col.DataType.ToString(),
                    //        RealType = col.DataType
                    //    };
                    //    attribs.Add(fieldAttribute);
                    //}
                    //rs.FieldAttributes.Add(attribs);
                }
                return rs;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("because it is being used by another process")) { MessageBox.Show("Tệp Excel đang mở. Vui lòng đóng tệp và thực hiện lại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error); }
                else MessageBox.Show($"Lỗi khi xử lí.\n{ex.Message}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);

                return null!;
            }            
        }
        private void LoadExcel()
        {
            OpenFileDialog ofd = new()
            {
                Filter = "Excel Files (*.xls, *.xlsx)|*.xls;*.xlsx",
                Title = "Mở tệp Excel nguồn"
            };
            if (ofd.ShowDialog() == false)
                return;
            FilePath = ofd.FileName;
            ExcelRS = ReadExcel(ofd.FileName, true);
        }
        private async Task ImportData()
        {
            if (ExcelTable == null || string.IsNullOrEmpty(SelectedSheet)) return;

            if (SelectedSheet.StartsWith(GetDescription(ListImport.StartWeek))) await ImportStartWeek(ExcelTable, SelectedSheet);
            if (SelectedSheet.StartsWith(GetDescription(ListImport.Level))) await ImportLevel(ExcelTable, SelectedSheet);
            if (SelectedSheet.StartsWith(GetDescription(ListImport.Grade))) await ImportGrade(ExcelTable, SelectedSheet);
            if (SelectedSheet.StartsWith(GetDescription(ListImport.Class))) await ImportClass(ExcelTable, SelectedSheet);
            if (SelectedSheet.StartsWith(GetDescription(ListImport.Teacher))) await ImportTeacher(ExcelTable, SelectedSheet);
            if (SelectedSheet.StartsWith(GetDescription(ListImport.Student))) await ImportStudent(ExcelTable, SelectedSheet);
            if (SelectedSheet.StartsWith(GetDescription(ListImport.DisciplineType))) await ImportDisciplineType(ExcelTable, SelectedSheet);
            if (SelectedSheet.StartsWith(GetDescription(ListImport.DisciplineGroup))) await ImportDisciplineGroup(ExcelTable, SelectedSheet);
            if (SelectedSheet.StartsWith(GetDescription(ListImport.Discipline))) await ImportDiscipline(ExcelTable, SelectedSheet);
            if (SelectedSheet.StartsWith(GetDescription(ListImport.PhanBoHocSinh))) await ImportStudentDistribute(ExcelTable, SelectedSheet);
        }
        private async Task ImportStartWeek(DataTable dtExcel, string sheetName)
        {
            int primaryKey = 0;
            try { dtExcel.Columns[primaryKey].Unique = false; } catch (Exception ex) { ShowErrorUnique(ex, sheetName, primaryKey); return; }

            int iCount = 0, indexRow = 0, countAdd = 0, countUpdate = 0;
            string indexColumnName = string.Empty;

            var notificationManager = new NotificationManager();
            using var progress = notificationManager.ShowProgressBar();
            foreach (DataRow r in dtExcel.Rows)
            {
                bool addNew = false;
                indexRow++;
                try
                {
                    indexColumnName = "Năm học";
                    string? name = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName]);
                    if (string.IsNullOrEmpty(name)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    indexColumnName = "Ngày bắt đầu";
                    DateTime? startDate;
                    try { startDate = Convert.ToDateTime(r[indexColumnName]); } catch (Exception) { startDate = DateTime.MinValue; }

                    indexColumnName = "Sử dụng";
                    bool used = Convert.IsDBNull(r[indexColumnName]) ? false : Convert.ToBoolean(r[indexColumnName]);

                    DtoStartWeek InputStartWeek = new() { OnYear = name, StartDate = startDate, Used = used };
                    DataProcess<DtoStartWeek> dp = new();
                    DtoResult<DtoStartWeek> rs = await dp.GetOneAsync(new DtoStartWeek() { OnYear = InputStartWeek.OnYear });
                    if (rs!.Result == null)
                    {
                        addNew = true;
                        rs = await dp.AddAsync(InputStartWeek);
                    }
                    else
                    {
                        InputStartWeek.Id = rs.Result.Id;
                        rs = await dp.UpdateAsync(InputStartWeek);
                    }
                    iCount++;
                    if (addNew) countAdd++; else countUpdate++;
                    progress.Cancel.ThrowIfCancellationRequested();
                    progress.Report((null, $"Đang import {iCount}/{dtExcel.Rows.Count}", sheetName, true));
                    await Task.Delay(TimeSpan.FromSeconds(0.02), progress.Cancel).ConfigureAwait(false);
                }
                catch (Exception ex) { ShowErrowImport(ex, sheetName, indexRow, indexColumnName); return; }
            }
            ShowSuccessImport(sheetName, iCount, dtExcel.Rows.Count, countAdd, countUpdate);
        }
        private async Task ImportLevel(DataTable dtExcel, string sheetName)
        {
            int primaryKey = 0;
            try { dtExcel.Columns[primaryKey].Unique = false; } catch (Exception ex) { ShowErrorUnique(ex, sheetName, primaryKey); return; }

            int iCount = 0, indexRow = 0, countAdd = 0, countUpdate = 0;
            string indexColumnName = string.Empty;

            var notificationManager = new NotificationManager();
            using var progress = notificationManager.ShowProgressBar();
            foreach (DataRow r in dtExcel.Rows)
            {
                bool addNew = false;
                indexRow++;
                try
                {
                    indexColumnName = "Cấp học";
                    string? name = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName]);
                    if (string.IsNullOrEmpty(name)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DtoLevel InputLevel = new() { LevelName = name };
                    DataProcess<DtoLevel> dp = new();
                    DtoResult<DtoLevel> rs = await dp.GetOneAsync(new DtoLevel() { LevelName = InputLevel.LevelName });                    
                    if (rs!.Result == null)
                    {
                        addNew = true;
                        rs = await dp.AddAsync(InputLevel);
                    }
                    else
                    {
                        InputLevel.Id = rs.Result.Id;
                        rs = await dp.UpdateAsync(InputLevel);
                    }
                    iCount++;
                    if (addNew) countAdd++; else countUpdate++;
                    progress.Cancel.ThrowIfCancellationRequested();
                    progress.Report((null, $"Đang import {iCount}/{dtExcel.Rows.Count}", sheetName, true));
                    await Task.Delay(TimeSpan.FromSeconds(0.02), progress.Cancel).ConfigureAwait(false);
                }
                catch (Exception ex) { ShowErrowImport(ex, sheetName, indexRow, indexColumnName); return; }
            }
            ShowSuccessImport(sheetName, iCount, dtExcel.Rows.Count, countAdd, countUpdate);
        }
        private async Task ImportGrade(DataTable dtExcel, string sheetName)
        {
            int primaryKey = 0;
            try { dtExcel.Columns[primaryKey].Unique = false; } catch (Exception ex) { ShowErrorUnique(ex, sheetName, primaryKey); return; }

            int iCount = 0, indexRow = 0, countAdd = 0, countUpdate = 0;
            string indexColumnName = string.Empty;

            var notificationManager = new NotificationManager();
            using var progress = notificationManager.ShowProgressBar();
            foreach (DataRow r in dtExcel.Rows)
            {
                bool addNew = false;
                indexRow++;
                try
                {
                    indexColumnName = "Khối lớp";
                    string? name = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName]);
                    if (string.IsNullOrEmpty(name)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    indexColumnName = "Cấp học";
                    string? levelName = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName]);
                    if (string.IsNullOrEmpty(levelName)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DataProcess<DtoLevel> dpLevel = new();
                    DtoResult<DtoLevel> rsLevel = await dpLevel.GetOneAsync(new DtoLevel() { LevelName = levelName });
                    if (rsLevel.Result == null) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DtoGrade InputGrade = new()
                    {
                        GradeName = name,
                        LevelId = rsLevel.Result.Id
                    };
                    DataProcess<DtoGrade> dp = new();
                    DtoResult<DtoGrade> rs = await dp.GetOneAsync(new DtoGrade() { GradeName = InputGrade.GradeName });
                    if (rs!.Result == null)
                    {
                        addNew = true;
                        rs = await dp.AddAsync(InputGrade);
                    }
                    else
                    {
                        InputGrade.Id = rs.Result.Id;
                        rs = await dp.UpdateAsync(InputGrade);
                    }
                    iCount++;
                    if (addNew) countAdd++; else countUpdate++;

                    progress.Cancel.ThrowIfCancellationRequested();
                    progress.Report((null, $"Đang import {iCount}/{dtExcel.Rows.Count}", sheetName, true));
                    await Task.Delay(TimeSpan.FromSeconds(0.02), progress.Cancel).ConfigureAwait(false);
                }
                catch (Exception ex) { ShowErrowImport(ex, sheetName, indexRow, indexColumnName); return; }
            }
            ShowSuccessImport(sheetName, iCount, dtExcel.Rows.Count, countAdd, countUpdate);
        }
        private async Task ImportClass(DataTable dtExcel, string sheetName)
        {
            int primaryKey = 0;
            try { dtExcel.Columns[primaryKey].Unique = false; } catch (Exception ex) { ShowErrorUnique(ex, sheetName, primaryKey); return; }

            int iCount = 0, indexRow = 0, countAdd = 0, countUpdate = 0;
            string indexColumnName = string.Empty;

            var notificationManager = new NotificationManager();
            using var progress = notificationManager.ShowProgressBar();
            foreach (DataRow r in dtExcel.Rows)
            {
                bool addNew = false;
                indexRow++;
                try
                {
                    indexColumnName = "Lớp học";
                    string? name = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName]);
                    if (string.IsNullOrEmpty(name)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    indexColumnName = "Khối lớp";
                    string? gradeName = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName]);
                    if (string.IsNullOrEmpty(gradeName)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DataProcess<DtoGrade> dpGrade = new();
                    DtoResult<DtoGrade> rsGrade = await dpGrade.GetOneAsync(new DtoGrade() { GradeName = gradeName });
                    if (rsGrade.Result == null) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DtoClass InputClass = new()
                    {
                        ClassName = name,
                        GradeId = rsGrade.Result.Id
                    };
                    DataProcess<DtoClass> dp = new();
                    DtoResult<DtoClass> rs = await dp.GetOneAsync(new DtoClass() { ClassName = InputClass.ClassName });
                    if (rs!.Result == null)
                    {
                        addNew = true;
                        rs = await dp.AddAsync(InputClass);
                    }
                    else
                    {
                        InputClass.Id = rs.Result.Id;
                        rs = await dp.UpdateAsync(InputClass);
                    }
                    iCount++;
                    if (addNew) countAdd++; else countUpdate++;

                    progress.Cancel.ThrowIfCancellationRequested();
                    progress.Report((null, $"Đang import {iCount}/{dtExcel.Rows.Count}", sheetName, true));
                    await Task.Delay(TimeSpan.FromSeconds(0.02), progress.Cancel).ConfigureAwait(false);
                }
                catch (Exception ex) { ShowErrowImport(ex, sheetName, indexRow, indexColumnName); return; }
            }
            ShowSuccessImport(sheetName, iCount, dtExcel.Rows.Count, countAdd, countUpdate);
        }
        private async Task ImportTeacher(DataTable dtExcel, string sheetName)
        {
            int primaryKey = 6; // COLUMN PRIMARYKEY
            try { dtExcel.Columns[primaryKey].Unique = false; } catch (Exception ex) { ShowErrorUnique(ex, sheetName, primaryKey); return; }

            int iCount = 0, indexRow = 0, countAdd = 0, countUpdate = 0;
            string indexColumnName = string.Empty;

            var notificationManager = new NotificationManager();
            using var progress = notificationManager.ShowProgressBar();
            foreach (DataRow r in dtExcel.Rows)
            {
                bool addNew = false;
                indexRow++;
                try
                {
                    indexColumnName = "Mã";
                    string? code = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName])!.Trim();
                    if (string.IsNullOrEmpty(code)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    indexColumnName = "Họ và tên";
                    string? fullName = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName])!.Trim();
                    if (string.IsNullOrEmpty(fullName)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DtoTeacher InputTeacher = new() 
                    { 
                        Code = code,
                        FullName = fullName,
                        Phone = Convert.IsDBNull(r["Điện thoại"]) ? string.Empty : Convert.ToString(r["Điện thoại"])!.Trim(),
                        Email = Convert.IsDBNull(r["Email"]) ? string.Empty : Convert.ToString(r["Email"])!.Trim()
                    };
                    DataProcess<DtoTeacher> dp = new();
                    DtoResult<DtoTeacher> rs = await dp.GetOneAsync(new DtoTeacher() { Code = InputTeacher.Code });
                    if (rs!.Result == null)
                    {
                        addNew = true;
                        rs = await dp.AddAsync(InputTeacher);
                    }
                    else
                    {
                        InputTeacher.Id = rs.Result.Id;
                        rs = await dp.UpdateAsync(InputTeacher);
                    }
                    iCount++;
                    if (addNew) countAdd++; else countUpdate++;

                    progress.Cancel.ThrowIfCancellationRequested();
                    progress.Report((null, $"Đang import {iCount}/{dtExcel.Rows.Count}", sheetName, true));
                    await Task.Delay(TimeSpan.FromSeconds(0.02), progress.Cancel).ConfigureAwait(false);
                }
                catch (Exception ex) { ShowErrowImport(ex, sheetName, indexRow, indexColumnName); return; }
            }
            ShowSuccessImport(sheetName, iCount, dtExcel.Rows.Count, countAdd, countUpdate);
        }
        private async Task ImportStudent(DataTable dtExcel, string sheetName)
        {
            int primaryKey = 0;
            try { dtExcel.Columns[primaryKey].Unique = false; } catch (Exception ex) { ShowErrorUnique(ex, sheetName, primaryKey); return; }

            int iCount = 0, indexRow = 0, countAdd = 0, countUpdate = 0;
            string indexColumnName = string.Empty;

            var notificationManager = new NotificationManager();
            using var progress = notificationManager.ShowProgressBar();
            foreach (DataRow r in dtExcel.Rows)
            {
                bool addNew = false;
                indexRow++;
                try
                {
                    indexColumnName = "Mã";
                    string? code = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName])!.Trim();
                    if (string.IsNullOrEmpty(code)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    indexColumnName = "Họ và tên";
                    string? fullName = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName])!.Trim();
                    if (string.IsNullOrEmpty(fullName)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    bool? gender = Convert.IsDBNull(r["Giới tính"]) ? null : Convert.ToString(r["Giới tính"])!.ToUpper().Equals("NAM") ? true : false;
                    DateTime? birthday = null;
                    if (r["Ngày sinh"].GetType() == typeof(string))
                    {
                        string? ngaySinh = r["Ngày sinh"].ToString();
                        if (!string.IsNullOrEmpty(ngaySinh))
                        {
                            string[] bdate = ngaySinh.Split('/');
                            birthday = new(int.Parse(bdate[2]), int.Parse(bdate[1]), int.Parse(bdate[0]));
                        }
                    }
                    else if (r["Ngày sinh"].GetType() == typeof(DateTime))
                    {
                        birthday = (DateTime)r["Ngày sinh"];
                    }    

                    //try { birthday = Convert.ToDateTime(r["Ngày sinh"]); } catch (Exception) { birthday = DateTime.MinValue; }
                    DtoStudent InputStudent = new()
                    {
                        Code = code,
                        FullName = fullName,
                        Gender = gender,
                        Birthday = birthday,
                        Phone = Convert.IsDBNull(r["Điện thoại"]) ? null : Convert.ToString(r["Điện thoại"])!.Trim(),
                        Email = Convert.IsDBNull(r["Email"]) ? null : Convert.ToString(r["Email"])!.Trim(),
                        AddressCurrent = Convert.IsDBNull(r["Địa chỉ"]) ? null : Convert.ToString(r["Địa chỉ"])!.Trim(),
                        SequenceNumber = Convert.IsDBNull(r["Số thứ tự"]) ? null : Convert.ToInt32(r["Số thứ tự"]),
                        Note = Convert.IsDBNull(r["Ghi chú"]) ? null : Convert.ToString(r["Ghi chú"])!.Trim()
                    };
                    DataProcess<DtoStudent> dp = new();
                    DtoResult<DtoStudent> rs = await dp.GetOneAsync(new DtoStudent() { Code = InputStudent.Code });
                    if (rs!.Result == null)
                    {
                        addNew = true;
                        rs = await dp.AddAsync(InputStudent);
                    }
                    else
                    {
                        InputStudent.Id = rs.Result.Id;
                        rs = await dp.UpdateAsync(InputStudent);
                    }
                    iCount++;
                    if (addNew) countAdd++; else countUpdate++;

                    progress.Cancel.ThrowIfCancellationRequested();
                    progress.Report((null, $"Đang import {iCount}/{dtExcel.Rows.Count}", sheetName, true));
                    await Task.Delay(TimeSpan.FromSeconds(0.02), progress.Cancel).ConfigureAwait(false);
                }
                catch (Exception ex) { ShowErrowImport(ex, sheetName, indexRow, indexColumnName); return; }
            }
            ShowSuccessImport(sheetName, iCount, dtExcel.Rows.Count, countAdd, countUpdate);
        }
        private async Task ImportDisciplineType(DataTable dtExcel, string sheetName)
        {
            int primaryKey = 0;
            try { dtExcel.Columns[primaryKey].Unique = false; } catch (Exception ex) { ShowErrorUnique(ex, sheetName, primaryKey); return; }

            int iCount = 0, indexRow = 0, countAdd = 0, countUpdate = 0;
            string indexColumnName = string.Empty;

            var notificationManager = new NotificationManager();
            using var progress = notificationManager.ShowProgressBar();
            foreach (DataRow r in dtExcel.Rows)
            {
                bool addNew = false;
                indexRow++;
                try
                {
                    indexColumnName = "Kiểu nền nếp";
                    string? name = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName]);
                    if (string.IsNullOrEmpty(name)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DtoDisciplineType InputDisciplineType = new() { DisciplineTypeName = name };
                    DataProcess<DtoDisciplineType> dp = new();
                    DtoResult<DtoDisciplineType> rs = await dp.GetOneAsync(new DtoDisciplineType() { DisciplineTypeName = InputDisciplineType.DisciplineTypeName });
                    if (rs!.Result == null)
                    {
                        addNew = true;
                        rs = await dp.AddAsync(InputDisciplineType);
                    }
                    else
                    {
                        InputDisciplineType.Id = rs.Result.Id;
                        rs = await dp.UpdateAsync(InputDisciplineType);
                    }
                    iCount++;
                    if (addNew) countAdd++; else countUpdate++;
                    progress.Cancel.ThrowIfCancellationRequested();
                    progress.Report((null, $"Đang import {iCount}/{dtExcel.Rows.Count}", sheetName, true));
                    await Task.Delay(TimeSpan.FromSeconds(0.02), progress.Cancel).ConfigureAwait(false);
                }
                catch (Exception ex) { ShowErrowImport(ex, sheetName, indexRow, indexColumnName); return; }
            }
            ShowSuccessImport(sheetName, iCount, dtExcel.Rows.Count, countAdd, countUpdate);
        }
        private async Task ImportDisciplineGroup(DataTable dtExcel, string sheetName)
        {
            int primaryKey = 0;
            try { dtExcel.Columns[primaryKey].Unique = false; } catch (Exception ex) { ShowErrorUnique(ex, sheetName, primaryKey); return; }

            int iCount = 0, indexRow = 0, countAdd = 0, countUpdate = 0;
            string indexColumnName = string.Empty;

            var notificationManager = new NotificationManager();
            using var progress = notificationManager.ShowProgressBar();
            foreach (DataRow r in dtExcel.Rows)
            {
                bool addNew = false;
                indexRow++;
                try
                {
                    indexColumnName = "Nhóm nền nếp";
                    string? name = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName]);
                    if (string.IsNullOrEmpty(name)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DtoDisciplineGroup InputDisciplineGroup = new() { DisciplineGroupName = name };
                    DataProcess<DtoDisciplineGroup> dp = new();
                    DtoResult<DtoDisciplineGroup> rs = await dp.GetOneAsync(new DtoDisciplineGroup() { DisciplineGroupName = InputDisciplineGroup.DisciplineGroupName });
                    if (rs!.Result == null)
                    {
                        addNew = true;
                        rs = await dp.AddAsync(InputDisciplineGroup);
                    }
                    else
                    {
                        InputDisciplineGroup.Id = rs.Result.Id;
                        rs = await dp.UpdateAsync(InputDisciplineGroup);
                    }
                    iCount++;
                    if (addNew) countAdd++; else countUpdate++;
                    progress.Cancel.ThrowIfCancellationRequested();
                    progress.Report((null, $"Đang import {iCount}/{dtExcel.Rows.Count}", sheetName, true));
                    await Task.Delay(TimeSpan.FromSeconds(0.02), progress.Cancel).ConfigureAwait(false);
                }
                catch (Exception ex) { ShowErrowImport(ex, sheetName, indexRow, indexColumnName); return; }
            }
            ShowSuccessImport(sheetName, iCount, dtExcel.Rows.Count, countAdd, countUpdate);
        }
        private async Task ImportDiscipline(DataTable dtExcel, string sheetName)
        {
            int primaryKey = 0;
            try { dtExcel.Columns[primaryKey].Unique = false; } catch (Exception ex) { ShowErrorUnique(ex, sheetName, primaryKey); return; }

            int iCount = 0, indexRow = 0, countAdd = 0, countUpdate = 0;
            string indexColumnName = string.Empty;

            var notificationManager = new NotificationManager();
            using var progress = notificationManager.ShowProgressBar();
            foreach (DataRow r in dtExcel.Rows)
            {
                bool addNew = false;
                indexRow++;
                try
                {
                    indexColumnName = "Nền nếp";
                    string? name = Convert.IsDBNull(r[indexColumnName]) ? null : Convert.ToString(r[indexColumnName]);
                    if (string.IsNullOrEmpty(name)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    indexColumnName = "Kiểu nền nếp";
                    string? type = Convert.IsDBNull(r[indexColumnName]) ? null : Convert.ToString(r[indexColumnName]);
                    if (string.IsNullOrEmpty(type)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DataProcess<DtoDisciplineType> dpDisciplineType = new();
                    DtoResult<DtoDisciplineType> rsDisciplineType = await dpDisciplineType.GetOneAsync(new DtoDisciplineType() { DisciplineTypeName = type });
                    if (rsDisciplineType.Result == null) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    indexColumnName = "Nhóm nền nếp";
                    string? group = Convert.IsDBNull(r[indexColumnName]) ? null : Convert.ToString(r[indexColumnName]);
                    if (string.IsNullOrEmpty(group)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DataProcess<DtoDisciplineGroup> dpDisciplineGroup = new();
                    DtoResult<DtoDisciplineGroup> rsDisciplineGroup = await dpDisciplineGroup.GetOneAsync(new DtoDisciplineGroup() { DisciplineGroupName = group });
                    if (rsDisciplineGroup.Result == null) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DtoDiscipline InputDiscipline = new() 
                    { 
                        DisciplineName = name,
                        DisciplineGroupId = rsDisciplineGroup.Result.Id,
                        ApplyFor = Convert.IsDBNull(r["Áp dụng"]) ? 0 : Convert.ToString(r["Áp dụng"])!.Contains("Lớp học") ? 1 : Convert.ToString(r["Áp dụng"])!.Contains("Học sinh") ? 2 : 0,
                        PlusPoint = Convert.IsDBNull(r["Điểm cộng"]) ? 0 : Convert.ToInt32(r["Điểm cộng"]),
                        MinusPoint = Convert.IsDBNull(r["Điểm trừ"]) ? 0 : Convert.ToInt32(r["Điểm trừ"]),
                        Display = Convert.IsDBNull(r["Hiển thị"]) ? true : Convert.ToBoolean(r["Hiển thị"]),
                        DisciplineTypeId = rsDisciplineType.Result.Id,
                        SequenceNumber = Convert.IsDBNull(r["Thứ tự"]) ? 0 : Convert.ToInt32(r["Thứ tự"]),
                        Note = Convert.IsDBNull(r["Ghi chú"]) ? null : Convert.ToString(r["Ghi chú"])
                    };
                   
                    DataProcess<DtoDiscipline> dp = new();
                    DtoResult<DtoDiscipline> rs = await dp.GetOneAsync(new DtoDiscipline() { DisciplineName = InputDiscipline.DisciplineName });
                    if (rs!.Result == null)
                    {
                        addNew = true;
                        rs = await dp.AddAsync(InputDiscipline);
                    }
                    else
                    {
                        InputDiscipline.Id = rs.Result.Id;
                        rs = await dp.UpdateAsync(InputDiscipline);
                    }
                    iCount++;
                    if (addNew) countAdd++; else countUpdate++;
                    progress.Cancel.ThrowIfCancellationRequested();
                    progress.Report((null, $"Đang import {iCount}/{dtExcel.Rows.Count}", sheetName, true));
                    await Task.Delay(TimeSpan.FromSeconds(0.02), progress.Cancel).ConfigureAwait(false);
                }
                catch (Exception ex) { ShowErrowImport(ex, sheetName, indexRow, indexColumnName); return; }
            }
            ShowSuccessImport(sheetName, iCount, dtExcel.Rows.Count, countAdd, countUpdate);
        }
        private async Task ImportStudentDistribute(DataTable dtExcel, string sheetName)
        {
            int primaryKey = 0;
            try { dtExcel.Columns[primaryKey].Unique = false; } catch (Exception ex) { ShowErrorUnique(ex, sheetName, primaryKey); return; }

            int iCount = 0, indexRow = 0, countAdd = 0, countUpdate = 0;
            string indexColumnName = string.Empty;

            var notificationManager = new NotificationManager();
            using var progress = notificationManager.ShowProgressBar();
            foreach (DataRow r in dtExcel.Rows)
            {
                bool addNew = false;
                indexRow++;
                try
                {
                    // KIỂM TRA ClassDistribute
                    indexColumnName = "Lớp học";
                    string? className = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName]);
                    if (string.IsNullOrEmpty(className)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DataProcess<DtoClass> dpClass = new();
                    DtoResult<DtoClass> rsClass = await dpClass.GetOneAsync(new DtoClass() { ClassName = className });
                    if (rsClass.Result == null) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    indexColumnName = "Năm học";
                    string? onYear = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName]);
                    if (string.IsNullOrEmpty(onYear)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DataProcess<DtoClassDistribute> dpClassDistribute = new();
                    DtoResult<DtoClassDistribute> rsClassDistribute = await dpClassDistribute.GetOneAsync(new DtoClassDistribute() { ClassId = rsClass.Result.Id, OnYear = onYear });
                    if (rsClassDistribute.Result == null) { ShowErrorDetail(indexRow, indexColumnName); return; }
                    
                    // KIỂM TRA Student
                    indexColumnName = "Mã học sinh";
                    string? code = Convert.IsDBNull(r[indexColumnName]) ? string.Empty : Convert.ToString(r[indexColumnName])!.Trim();
                    if (string.IsNullOrEmpty(code)) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    DataProcess<DtoStudent> dpStudent = new();
                    DtoResult<DtoStudent> rsStudent = await dpStudent.GetOneAsync(new DtoStudent() { Code = code });
                    if (rsStudent.Result == null) { ShowErrorDetail(indexRow, indexColumnName); return; }

                    indexColumnName = "Ngày phân bổ";
                    DateTime? assignDate;
                    try { assignDate = Convert.ToDateTime(r[indexColumnName]); } catch (Exception) { assignDate = DateTime.Today; }

                    DtoStudentDistribute InputStudentDistribute = new()
                    {
                        ClassDistributeId = rsClassDistribute.Result.Id,
                        StudentId = rsStudent.Result.Id,
                        AssignDate = assignDate
                    };
                    DataProcess<DtoStudentDistribute> dp = new();
                    DtoResult<DtoStudentDistribute> rs = await dp.GetOneAsync(new DtoStudentDistribute() { ClassDistributeId = InputStudentDistribute.ClassDistributeId, StudentId = InputStudentDistribute.StudentId });
                    if (rs!.Result == null)
                    {
                        addNew = true;
                        rs = await dp.AddAsync(InputStudentDistribute);
                    }
                    else
                    {
                        InputStudentDistribute.Id = rs.Result.Id;
                        rs = await dp.UpdateAsync(InputStudentDistribute);
                    }
                    iCount++;
                    if (addNew) countAdd++; else countUpdate++;

                    progress.Cancel.ThrowIfCancellationRequested();
                    progress.Report((null, $"Đang import {iCount}/{dtExcel.Rows.Count}", sheetName, true));
                    await Task.Delay(TimeSpan.FromSeconds(0.02), progress.Cancel).ConfigureAwait(false);
                }
                catch (Exception ex) { ShowErrowImport(ex, sheetName, indexRow, indexColumnName); return; }
            }
            ShowSuccessImport(sheetName, iCount, dtExcel.Rows.Count, countAdd, countUpdate);
        }
        #endregion

        #region RelayCommands
        public RelayCommand LoadCommand => new(cmd => LoadExcel(), canExecute => true);
        public RelayCommand ImportCommand => new(async cmd => await ImportData(), canExecute => true);
        #endregion

        #region Properties
        private DataTable excelTable = new();
        public DataTable ExcelTable
        {
            get { return excelTable; }
            set
            {
                excelTable = value;
                OnPropertyChanged();
            }
        }
        private ExcelResult excelResult = new();
        public ExcelResult ExcelRS
        {
            get => excelResult;
            set
            {
                excelResult = value;
                OnPropertyChanged();
            }
        }
        string? selectedSheet = null;
        public string? SelectedSheet
        {
            get => selectedSheet;
            set
            {
                selectedSheet = value;
                OnPropertyChanged();
                if (value != null)
                {
                    int index = ExcelRS.Sheets.IndexOf(value);
                    ExcelTable = ExcelRS.Tables[index];
                    if (ExcelRS.FieldAttributes.Count > 0)
                    {
                        Fields = new(ExcelRS.FieldAttributes[index]);
                    }
                    else
                        Fields = new();
                }
            }
        }
        private ObservableCollection<FieldAttribute> fields = new();
        public ObservableCollection<FieldAttribute> Fields
        {
            get => fields;
            set
            {
                fields = value;
                OnPropertyChanged();
            }
        }
        private string filePath = string.Empty;
        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
    public class ExcelResult
    {
        public List<string> Sheets { get; set; } = new();
        public List<DataTable> Tables { get; set; } = new();
        public ObservableCollection<List<FieldAttribute>> FieldAttributes { get; set; } = new();
    }
    public class FieldAttribute
    {
        public string FieldName { get; set; } = "";
        public string FieldType { get; set; } = "";
        public Type RealType { get; set; } = typeof(string);
    }
}
