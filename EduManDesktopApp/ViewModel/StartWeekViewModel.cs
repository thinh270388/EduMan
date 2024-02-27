using EduManModel.Dtos;
using EduManModel;
using MVVM;
using SLHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notification.Wpf.Classes;
using Notification.Wpf;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace EduManDesktopApp.ViewModel
{
    public class StartWeekViewModel : ViewModelBase
    {
        public StartWeekViewModel()
        {
            Task.Run(async () => await LoadStartWeek()).Wait();
        }

        #region Methods
        async Task LoadStartWeek()
        {
            DataProcess<DtoStartWeek> dp = new();
            DtoResult<DtoStartWeek> rs = await dp.GetAllAsync(new DtoStartWeek());
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);

                StartWeeks = new();
                CboStartWeeks = new();
                SelectedStartWeek = null;
                return;
            }
            StartWeeks = new(rs!.Results!);
            SelectedStartWeek = StartWeeks.FirstOrDefault()!;
            CboStartWeeks = new(rs.Results!);
        }
        async Task AddStartWeek()
        {
            if (StartWeekSaved)
            {
                InputStartWeek = new();
                StartWeekFocused = true;
            }
            else
            {
                DataProcess<DtoStartWeek> dp = new();
                DtoResult<DtoStartWeek> rs = await dp.GetOneAsync(new DtoStartWeek() { OnYear = InputStartWeek!.OnYear });
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                if (InputStartWeek.Id == null)
                {
                    if (rs!.Result != null)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.OnYear}]", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.AddAsync(InputStartWeek);

                    StartWeeks.Add(rs.Result!);
                    StartWeeks = new(StartWeeks);
                }
                else
                {
                    if (rs!.Result != null && rs.Result.Id != InputStartWeek.Id)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.OnYear}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.UpdateAsync(InputStartWeek);
                    DtoStartWeek? dto = StartWeeks.FirstOrDefault(x => x.Id == InputStartWeek.Id);
                    if (dto != null)
                    {
                        dto.OnYear = InputStartWeek.OnYear;
                        dto.StartDate = InputStartWeek.StartDate;
                        dto.Used = InputStartWeek.Used;
                    }
                    StartWeeks = new(StartWeeks);
                }
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                var notificationManager = new NotificationManager();
                notificationManager.Show($"Đã lưu [{rs!.Result!.OnYear}]!", NotificationType.Success);
                if (StartWeeks.Count > 0)
                    SelectedStartWeek = StartWeeks.FirstOrDefault(x => x.Id == rs.Result.Id)!;
            }
            StartWeekSaved = !StartWeekSaved;
            StartWeekFocused = false;
        }
        void EditStartWeek()
        {
            if (StartWeekSaved)
            {
                StartWeekFocused = true;
            }
            else
            {
                if (SelectedStartWeek != null)
                {
                    InputStartWeek = new()
                    {
                        Id = SelectedStartWeek!.Id,
                        OnYear = SelectedStartWeek.OnYear,
                        StartDate = SelectedStartWeek.StartDate,
                        Used = SelectedStartWeek.Used,
                        TypeList = SelectedStartWeek.TypeList
                    };
                }
                else
                    InputStartWeek = new();
            }
            StartWeekSaved = !StartWeekSaved;
            StartWeekFocused = false;
        }
        async Task DeleteStartWeek()
        {
            if (InputStartWeek != null)
            {
                NoticeBoxResult ans = NoticeBox.Show($"Bạn có chắc chắn xóa [{InputStartWeek.OnYear}]?", "Thông báo", NoticeBoxButton.YesNo, NoticeBoxImage.Question);
                if (ans == NoticeBoxResult.No) return;

                DataProcess<DtoStartWeek> dp = new();
                DtoResult<DtoStartWeek> rs = await dp.DeleteAsync(InputStartWeek);
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else
                    {
                        if (rs!.Message!.StartsWith("The DELETE statement conflicted with the REFERENCE constraint"))
                            NoticeBox.Show($"Không thể xóa!\r\nDữ liệu đang được dùng ở bảng khác", "Thông báo", NoticeBoxImage.Error);
                        else
                            NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    }
                    return;
                }
                NoticeBox.Show($"Đã xóa [{InputStartWeek.OnYear}]!");
                StartWeeks = new(StartWeeks.Where(x => x.Id != InputStartWeek.Id));
                SelectedStartWeek = StartWeeks.FirstOrDefault()!;
            }
        }
        bool CanAddStartWeek()
        {
            return (!StartWeekSaved && InputStartWeek != null && !string.IsNullOrEmpty(InputStartWeek.OnYear) && InputStartWeek.StartDate != null) || StartWeekSaved;
        }
        bool CanEditStartWeek()
        {
            return !StartWeekSaved || (StartWeekSaved && InputStartWeek != null && !string.IsNullOrEmpty(InputStartWeek.OnYear) && InputStartWeek.StartDate != null);
        }
        bool CanDeleteStartWeek()
        {
            return StartWeekSaved && InputStartWeek != null && !string.IsNullOrEmpty(InputStartWeek.OnYear);
        }
        async Task GenerateStartWeek()
        {
            if (SelectedStartWeek == null) return;

            var notificationManager = new NotificationManager();
            using var progress = notificationManager.ShowProgressBar();
            try
            {
                DataProcess<DtoWeekly> dp = new();
                DateTime dt = Convert.ToDateTime(SelectedStartWeek!.StartDate);
                for (int i = 0; i < 35; i++)
                {
                    string weeklyName = (i + 1) < 10 ? $"Tuần 0{i + 1}" : $"Tuần {i + 1}";
                    InputWeekly = new()
                    {
                        WeeklyName = weeklyName,
                        StartWeekId = SelectedStartWeek.Id,
                        FromDate = dt,
                        ToDate = dt.AddDays(6),
                        NumberOfLession = 30,
                        InitialPoint = 140,
                        Coefficient = 1
                    };
                    DtoResult<DtoWeekly> rs = await dp.AddAsync(InputWeekly);
                    dt = dt.AddDays(7);

                    progress.Cancel.ThrowIfCancellationRequested();
                    progress.Report((null, $"Đang xử lí {i + 1}/35", "Thêm tuần học", true));
                    await Task.Delay(TimeSpan.FromSeconds(0.02), progress.Cancel).ConfigureAwait(false);
                }
                notificationManager.Show($"Đã thêm 35 tuần học năm [{SelectedStartWeek.OnYear}].", NotificationType.Success);
                Task.Run(async () => await LoadWeekly()).Wait();
            }
            catch (OperationCanceledException)
            {
                notificationManager.Show("Hoạt động bị hủy bỏ!", NotificationType.Error);
            }            
        }
        #endregion

        #region RelayCommands
        public RelayCommand AddStartWeekCommand => new(async cmd => await AddStartWeek(), canExecute => CanAddStartWeek());
        public RelayCommand EditStartWeekCommand => new(cmd => EditStartWeek(), canExecute => CanEditStartWeek());
        public RelayCommand DeleteStartWeekCommand => new(async cmd => await DeleteStartWeek(), canExecute => CanDeleteStartWeek());
        public RelayCommand GenerateStartWeekCommand => new(async cmd => await GenerateStartWeek(), canExecute => true);
        #endregion

        #region Properties
        private ObservableCollection<DtoStartWeek> startWeeks = new();
        public ObservableCollection<DtoStartWeek> StartWeeks { get { return startWeeks; } set { startWeeks = value; OnPropertyChanged(); } }
        private DtoStartWeek? selectedStartWeek = new();
        public DtoStartWeek? SelectedStartWeek
        {
            get { return selectedStartWeek; }
            set
            {
                selectedStartWeek = value; OnPropertyChanged();
                if (value != null)
                {
                    InputStartWeek = new()
                    {
                        Id = value.Id,
                        OnYear = value.OnYear,
                        StartDate = value.StartDate,
                        Used = value.Used,
                        TypeList = value.TypeList
                    };
                    Task.Run(async () => await LoadWeekly()).Wait();
                }
                else
                {
                    Weeklys = new();
                    InputStartWeek = new();
                    CboStartWeeks = new();
                    CboStartWeek = null!;                    
                }
            }
        }
        private DtoStartWeek inputStartWeek = new();
        public DtoStartWeek InputStartWeek { get { return inputStartWeek; } set { inputStartWeek = value; OnPropertyChanged(); } }
        private bool startWeekSaved = true;
        public bool StartWeekSaved { get { return startWeekSaved; } set { startWeekSaved = value; OnPropertyChanged(); } }
        private bool startWeekFocused = false;
        public bool StartWeekFocused { get { return startWeekFocused; } set { startWeekFocused = value; OnPropertyChanged(); } }
        private ObservableCollection<DtoStartWeek> cboStartWeeks = new();
        public ObservableCollection<DtoStartWeek> CboStartWeeks { get { return cboStartWeeks; } set { cboStartWeeks = value; OnPropertyChanged(); } }
        private DtoStartWeek cboStartWeek = new();
        public DtoStartWeek CboStartWeek { get { return cboStartWeek; } set { cboStartWeek = value; OnPropertyChanged(); InputWeekly.StartWeekId = value != null && value.Id != null ? value.Id : null; } }
        #endregion

        #region Methods
        async Task LoadWeekly()
        {
            DataProcess<DtoWeekly> dp = new();
            DtoResult<DtoWeekly> rs = await dp.FindAsync(new DtoWeekly() { StartWeekId = SelectedStartWeek!.Id});
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);

                Weeklys = new();
                SelectedWeekly = null!;
                CboWeeklys = new();
                return;
            }
            Weeklys = new(rs!.Results!);
            CboWeeklys = new(rs.Results!);
            SelectedWeekly = Weeklys.FirstOrDefault()!;
        }
        async Task AddWeekly()
        {
            if (WeeklySaved)
            {
                InputWeekly = new()
                {
                    WeeklyName = "Tuần ",
                    NumberOfLession = 30,
                    Coefficient = 1,
                    InitialPoint = 140,
                    StartWeekId = SelectedStartWeek!.Id
                };
                WeeklyFocused = true;
            }
            else
            {
                DataProcess<DtoWeekly> dp = new();
                DtoResult<DtoWeekly> rs = new();
                if (InputWeekly.Id == null)
                {
                    rs = await dp.AddAsync(InputWeekly);

                    Weeklys.Add(rs.Result!);
                    Weeklys = new(Weeklys);
                }
                else
                {
                    rs = await dp.UpdateAsync(InputWeekly);
                    DtoWeekly? dto = Weeklys.FirstOrDefault(x => x.Id == InputWeekly.Id);
                    if (dto != null)
                    {
                        dto.Id = InputWeekly.Id;
                        dto.WeeklyName = InputWeekly.WeeklyName;
                        dto.Coefficient = InputWeekly.Coefficient;
                        dto.FromDate = InputWeekly.FromDate;
                        dto.NumberOfLession = InputWeekly.NumberOfLession;
                        dto.ToDate = InputWeekly.ToDate;
                        dto.InitialPoint = InputWeekly.InitialPoint;
                        dto.OnDutyClass = InputWeekly.OnDutyClass;
                        dto.StartWeekId = InputWeekly.StartWeekId;
                        dto.Planning = InputWeekly.Planning;
                        dto.Sumarizing = InputWeekly.Sumarizing;
                    }
                    Weeklys = new(Weeklys);
                }
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                NoticeBox.Show($"Đã lưu [{rs!.Result!.WeeklyName}]!", "Thông báo", NoticeBoxImage.Information);
                if (Weeklys.Count > 0)
                    SelectedWeekly = Weeklys.FirstOrDefault(x => x.Id == rs.Result.Id)!;
            }
            WeeklySaved = !WeeklySaved;
            WeeklyFocused = false;
        }
        void EditWeekly()
        {
            if (WeeklySaved)
            {
                WeeklyFocused = true;
            }
            else
            {
                if (SelectedWeekly != null)
                {
                    InputWeekly = new()
                    {
                        Id = SelectedWeekly!.Id,
                        WeeklyName = SelectedWeekly.WeeklyName
                    };
                }
                else
                    InputWeekly = new();
            }
            WeeklySaved = !WeeklySaved;
            WeeklyFocused = false;
        }
        async Task DeleteWeekly()
        {
            if (InputWeekly != null)
            {
                NoticeBoxResult ans = NoticeBox.Show($"Bạn có chắc chắn xóa [{InputWeekly.WeeklyName}]?", "Thông báo", NoticeBoxButton.YesNo, NoticeBoxImage.Question);
                if (ans == NoticeBoxResult.No) return;

                DataProcess<DtoWeekly> dp = new();
                DtoResult<DtoWeekly> rs = await dp.DeleteAsync(InputWeekly);
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else
                    {
                        if (rs!.Message!.StartsWith("The DELETE statement conflicted with the REFERENCE constraint"))
                            NoticeBox.Show($"Không thể xóa!\r\nDữ liệu đang được dùng ở bảng khác", "Thông báo", NoticeBoxImage.Error);
                        else
                            NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    }
                    return;
                }
                NoticeBox.Show($"Đã xóa [{InputWeekly.WeeklyName}]!");
                Weeklys = new(Weeklys.Where(x => x.Id != InputWeekly.Id));
                SelectedWeekly = Weeklys.FirstOrDefault()!;
            }
        }
        bool CanAddWeekly()
        {
            return (!WeeklySaved && InputWeekly != null && 
                    !string.IsNullOrEmpty(InputWeekly.WeeklyName) &&
                    InputWeekly.FromDate != null &&
                    InputWeekly.ToDate != null &&
                    InputWeekly.NumberOfLession != null &&
                    InputWeekly.InitialPoint != null &&
                    InputWeekly.Coefficient != null)
                || WeeklySaved;
        }
        bool CanEditWeekly()
        {
            return !WeeklySaved || 
                   (WeeklySaved && InputWeekly != null && 
                   !string.IsNullOrEmpty(InputWeekly.WeeklyName) &&
                   InputWeekly.FromDate != null &&
                   InputWeekly.ToDate != null &&
                   InputWeekly.NumberOfLession != null &&
                   InputWeekly.InitialPoint != null &&
                   InputWeekly.Coefficient != null);
        }
        bool CanDeleteWeekly()
        {
            return WeeklySaved && InputWeekly != null && !string.IsNullOrEmpty(InputWeekly.WeeklyName);
        }
        #endregion

        #region RelayCommands
        public RelayCommand AddWeeklyCommand => new(async cmd => await AddWeekly(), canExecute => CanAddWeekly());
        public RelayCommand EditWeeklyCommand => new(cmd => EditWeekly(), canExecute => CanEditWeekly());
        public RelayCommand DeleteWeeklyCommand => new(async cmd => await DeleteWeekly(), canExecute => CanDeleteWeekly());
        #endregion

        #region Properties
        private ObservableCollection<DtoWeekly> weeklys = new();
        public ObservableCollection<DtoWeekly> Weeklys { get { return weeklys; } set { weeklys = value; OnPropertyChanged(); } }
        private DtoWeekly? selectedWeekly = new();
        public DtoWeekly? SelectedWeekly
        {
            get { return selectedWeekly; }
            set
            {
                selectedWeekly = value; OnPropertyChanged();
                if (value != null && value.StartWeekId != null)
                {
                    InputWeekly = new()
                    {
                        Id = value.Id,
                        WeeklyName = value.WeeklyName,
                        Coefficient = value.Coefficient,
                        FromDate = value.FromDate,
                        InitialPoint = value.InitialPoint,
                        NumberOfLession = value.NumberOfLession,
                        OnDutyClass = value.OnDutyClass,
                        Planning = value.Planning,
                        StartWeekId = value.StartWeekId,
                        Sumarizing = value.Sumarizing,
                        ToDate = value.ToDate,
                        TypeList = value.TypeList
                    };
                    CboStartWeek = CboStartWeeks.FirstOrDefault(x => x.Id == value.StartWeekId)!;
                }
                else
                {
                    InputWeekly = new();
                    CboStartWeek = null!;
                }
            }
        }
        private DtoWeekly inputWeekly = new();
        public DtoWeekly InputWeekly { get { return inputWeekly; } set { inputWeekly = value; OnPropertyChanged(); } }
        private bool weeklySaved = true;
        public bool WeeklySaved { get { return weeklySaved; } set { weeklySaved = value; OnPropertyChanged(); } }
        private bool weeklyFocused = false;
        public bool WeeklyFocused { get { return weeklyFocused; } set { weeklyFocused = value; OnPropertyChanged(); } }
        private ObservableCollection<DtoWeekly> cboWeeklys = new();
        public ObservableCollection<DtoWeekly> CboWeeklys { get { return cboWeeklys; } set { cboWeeklys = value; OnPropertyChanged(); } }
        private DtoWeekly cboWeekly = new();
        public DtoWeekly CboWeekly { get { return cboWeekly; } set { cboWeekly = value; OnPropertyChanged(); } }
        #endregion
    }
}
