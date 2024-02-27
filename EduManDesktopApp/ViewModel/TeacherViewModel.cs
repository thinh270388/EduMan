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
using System.Security.Cryptography;
using Notification.Wpf;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using TextProcessing;

namespace EduManDesktopApp.ViewModel
{
    public class TeacherViewModel : ViewModelBase
    {
        public TeacherViewModel()
        {
            Task.Run(async () => await LoadClass()).Wait();
            Task.Run(async () => await LoadStartWeek()).Wait();
            Task.Run(async () => await GetStartWeekUsed()).Wait();
            Task.Run(async () => await LoadTeacher()).Wait();
            Task.Run(async () => await LoadStudentDistribute()).Wait();
            Task.Run(async () => await LoadClassDistribute()).Wait();
        }

        private DtoStartWeek startWeekUsed = new();
        public DtoStartWeek StartWeekUsed
        {
            get { return startWeekUsed; }
            set { startWeekUsed = value; OnPropertyChanged(); }
        }

        private ObservableCollection<DtoClass> cboClasses = new();
        public ObservableCollection<DtoClass> CboClasses { get { return cboClasses; } set { cboClasses = value; OnPropertyChanged(); } }
        private DtoClass cboClass = new();
        public DtoClass CboClass { get { return cboClass; } set { cboClass = value; OnPropertyChanged(); InputClassDistribute.ClassId = (value != null && value.Id != null) ? value.Id : null; } }


        private ObservableCollection<DtoStartWeek> cboStartWeeks = new();
        public ObservableCollection<DtoStartWeek> CboStartWeeks { get { return cboStartWeeks; } set { cboStartWeeks = value; OnPropertyChanged(); } }
        private DtoStartWeek cboStartWeek = new();
        public DtoStartWeek CboStartWeek { get { return cboStartWeek; } set { cboStartWeek = value; OnPropertyChanged(); InputClassDistribute.OnYear = (value != null && value.Id != null) ? value.OnYear : null; } }
        async Task GetStartWeekUsed()
        {
            DataProcess<DtoStartWeek> dp = new();
            DtoResult<DtoStartWeek> rs = await dp.GetOneAsync(new DtoStartWeek() { Used = true });

            StartWeekUsed = rs.Result!;
        }
        async Task LoadStartWeek()
        {
            DataProcess<DtoStartWeek> dp = new();
            DtoResult<DtoStartWeek> rs = await dp.GetAllAsync(new DtoStartWeek());
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);

                CboStartWeeks = new();
                return;
            }
            CboStartWeeks = new(rs!.Results!);
        }
        async Task LoadClass()
        {
            DataProcess<DtoClass> dp = new();
            DtoResult<DtoClass> rs = await dp.GetAllAsync(new DtoClass());
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);

                CboClasses = new();
                CboClass = null!;
                return;
            }
            CboClasses = new(rs!.Results!);
        }

        #region Methods
        async Task LoadTeacher()
        {
            DataProcess<DtoTeacher> dp = new();
            DtoResult<DtoTeacher> rs = await dp.GetAllAsync(new DtoTeacher());
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                return;
            }
            Teachers = new(rs!.Results!);
            SelectedTeacher = Teachers.FirstOrDefault()!;
            CboTeachers = new(rs.Results!);
        }
        async Task AddTeacher()
        {
            if (TeacherSaved)
            {
                InputTeacher = new();
                TeacherFocused = true;
            }
            else
            {
                DataProcess<DtoTeacher> dp = new();
                DtoResult<DtoTeacher> rs = await dp.GetOneAsync(new DtoTeacher() { Code = InputTeacher!.Code });
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                if (InputTeacher.Id == null)
                {
                    if (rs!.Result != null)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.Code}]", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.AddAsync(InputTeacher);

                    Teachers.Add(rs.Result!);
                    Teachers = new(Teachers);
                }
                else
                {
                    if (rs!.Result != null && rs.Result.Id != InputTeacher.Id)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.Code}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.UpdateAsync(InputTeacher);
                    DtoTeacher? dto = Teachers.FirstOrDefault(x => x.Id == InputTeacher.Id);
                    if (dto != null)
                    {
                        dto.Code = InputTeacher.Code;
                        dto.FullName = InputTeacher.FullName;
                        dto.Phone = InputTeacher.Phone;
                        dto.Email = InputTeacher.Email;
                    }
                    Teachers = new(Teachers);
                }
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                NoticeBox.Show($"Đã lưu [{rs!.Result!.Code}]!", "Thông báo", NoticeBoxImage.Information);
                if (Teachers.Count > 0)
                    SelectedTeacher = Teachers.FirstOrDefault(x => x.Id == rs.Result.Id)!;
            }
            TeacherSaved = !TeacherSaved;
            TeacherFocused = false;
        }
        void EditTeacher()
        {
            if (TeacherSaved)
            {
                TeacherFocused = true;
            }
            else
            {
                if (SelectedTeacher != null)
                {
                    InputTeacher = new()
                    {
                        Id = SelectedTeacher!.Id,
                        Code = SelectedTeacher.Code,
                        FullName = SelectedTeacher.FullName,
                        Phone = SelectedTeacher.Phone,
                        Email = SelectedTeacher.Email
                    };
                }
                else
                    InputTeacher = new();
            }
            TeacherSaved = !TeacherSaved;
            TeacherFocused = false;
        }
        async Task DeleteTeacher()
        {
            if (InputTeacher != null)
            {
                NoticeBoxResult ans = NoticeBox.Show($"Bạn có chắc chắn xóa [{InputTeacher.Code}]?", "Thông báo", NoticeBoxButton.YesNo, NoticeBoxImage.Question);
                if (ans == NoticeBoxResult.No) return;

                DataProcess<DtoTeacher> dp = new();
                DtoResult<DtoTeacher> rs = await dp.DeleteAsync(InputTeacher);
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
                NoticeBox.Show($"Đã xóa [{InputTeacher.Code}]!");
                Teachers = new(Teachers.Where(x => x.Id != InputTeacher.Id));
                SelectedTeacher = Teachers.FirstOrDefault()!;
            }
        }
        bool CanAddTeacher()
        {
            return (!TeacherSaved && InputTeacher != null && !string.IsNullOrEmpty(InputTeacher.Code) && !string.IsNullOrEmpty(InputTeacher.FullName)) || TeacherSaved;
        }
        bool CanEditTeacher()
        {
            return !TeacherSaved || (TeacherSaved && InputTeacher != null && !string.IsNullOrEmpty(InputTeacher.Code) && !string.IsNullOrEmpty(InputTeacher.FullName));
        }
        bool CanDeleteTeacher()
        {
            return TeacherSaved && InputTeacher != null && !string.IsNullOrEmpty(InputTeacher.Code);
        }
        #endregion

        #region RelayCommands
        public RelayCommand AddTeacherCommand => new(async cmd => await AddTeacher(), canExecute => CanAddTeacher());
        public RelayCommand EditTeacherCommand => new(cmd => EditTeacher(), canExecute => CanEditTeacher());
        public RelayCommand DeleteTeacherCommand => new(async cmd => await DeleteTeacher(), canExecute => CanDeleteTeacher());
        #endregion

        #region Properties
        private ObservableCollection<DtoTeacher> teachers = new();
        public ObservableCollection<DtoTeacher> Teachers { get { return teachers; } set { teachers = value; OnPropertyChanged(); } }
        private DtoTeacher? selectedTeacher = new();
        public DtoTeacher? SelectedTeacher
        {
            get { return selectedTeacher; }
            set
            {
                selectedTeacher = value; OnPropertyChanged();
                if (value != null)
                {
                    InputTeacher = new()
                    {
                        Id = value.Id,
                        Code = value.Code,
                        FullName = value.FullName,
                        Phone = value.Phone,
                        Email = value.Email,
                        TypeList = value.TypeList
                    };
                }
                else
                {
                    InputTeacher = new();
                }
            }
        }
        private DtoTeacher inputTeacher = new();
        public DtoTeacher InputTeacher { get { return inputTeacher; } set { inputTeacher = value; OnPropertyChanged(); } }
        private bool teacherSaved = true;
        public bool TeacherSaved { get { return teacherSaved; } set { teacherSaved = value; OnPropertyChanged(); } }
        private bool teacherFocused = false;
        public bool TeacherFocused { get { return teacherFocused; } set { teacherFocused = value; OnPropertyChanged(); } }
        private ObservableCollection<DtoTeacher> cboTeachers = new();
        public ObservableCollection<DtoTeacher> CboTeachers { get { return cboTeachers; } set { cboTeachers = value; OnPropertyChanged(); } }
        private DtoTeacher cboTeacher = new();
        public DtoTeacher CboTeacher { get { return cboTeacher; } set { cboTeacher = value; OnPropertyChanged(); InputClassDistribute.TeacherId = (value != null && value.Id != null) ? value.Id : null; } }
        #endregion

        #region Methods
        async Task LoadClassDistribute()
        {
            if (StartWeekUsed == null || StartWeekUsed.OnYear == null) return;

            DataProcess<DtoClassDistribute> dp = new();
            DtoResult<DtoClassDistribute> rs = await dp.FindAsync(new DtoClassDistribute() { OnYear = StartWeekUsed.OnYear });
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                return;
            }
            ClassDistributes = new(rs!.Results!);
            SelectedClassDistribute = ClassDistributes.FirstOrDefault()!;
            CboClassDistributes = new(rs.Results!);
        }
        async Task AddClassDistribute()
        {
            if (ClassDistributeSaved)
            {
                InputClassDistribute = new();
                CboClass = null!;
                CboTeacher = null!;
                CboStartWeek = null!;
                ClassDistributeFocused = true;
            }
            else
            {
                DataProcess<DtoClassDistribute> dp = new();
                DtoResult<DtoClassDistribute> rs = await dp.GetOneAsync(new DtoClassDistribute() { ClassId = InputClassDistribute!.ClassId });
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                if (InputClassDistribute.Id == null)
                {
                    if (rs!.Result != null)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.ClassId}]", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.AddAsync(InputClassDistribute);

                    ClassDistributes.Add(rs.Result!);
                    ClassDistributes = new(ClassDistributes);
                }
                else
                {
                    if (rs!.Result != null && rs.Result.Id != InputClassDistribute.Id)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.ClassId}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.UpdateAsync(InputClassDistribute);
                    DtoClassDistribute? dto = ClassDistributes.FirstOrDefault(x => x.Id == InputClassDistribute.Id);
                    if (dto != null)
                    {
                        dto.ClassId = InputClassDistribute.ClassId;
                        dto.TeacherId = InputClassDistribute.TeacherId;
                        dto.AssignDate = InputClassDistribute.AssignDate;
                        dto.OnYear = InputClassDistribute.OnYear;
                    }

                    ClassDistributes = new(ClassDistributes);
                }
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                NoticeBox.Show($"Đã lưu [{rs!.Result!.ClassId} - {rs!.Result!.TeacherId} - {rs!.Result!.OnYear}]!", "Thông báo", NoticeBoxImage.Information);
                if (ClassDistributes.Count > 0)
                    SelectedClassDistribute = ClassDistributes.FirstOrDefault(x => x.Id == rs.Result.Id)!;
            }
            ClassDistributeSaved = !ClassDistributeSaved;
            ClassDistributeFocused = false;
        }
        async Task AddCQuicklylassDistribute()
        {
            if (StartWeekUsed == null || CboClasses == null || CboTeachers == null) return;

            var notificationManager = new NotificationManager();
            using var progress = notificationManager.ShowProgressBar();
            try
            {
                DataProcess<DtoClassDistribute> dp = new();
                int i = 0;
                int sum = CboClasses.Count;
                foreach (DtoClass item in CboClasses)
                {
                    i++;
                    InputClassDistribute = new()
                    {
                        ClassId = item.Id,
                        TeacherId = CboTeachers.FirstOrDefault()!.Id,
                        AssignDate = DateTime.Today,
                        OnYear = StartWeekUsed!.OnYear
                    };
                    DtoResult<DtoClassDistribute> rs = await dp.AddAsync(InputClassDistribute);

                    progress.Cancel.ThrowIfCancellationRequested();
                    progress.Report((null, $"Đang xử lí {i}/{sum}", "Thêm phân bổ lớp", true));
                    await Task.Delay(TimeSpan.FromSeconds(0.02), progress.Cancel).ConfigureAwait(false);
                }
                
                notificationManager.Show($"Đã phân bổ {sum} lớp học năm [{StartWeekUsed!.OnYear}].", NotificationType.Success);
                Task.Run(async () => await LoadClassDistribute()).Wait();
            }
            catch (OperationCanceledException)
            {
                notificationManager.Show("Hoạt động bị hủy bỏ!", NotificationType.Error);
            }
        }
        void EditClassDistribute()
        {
            if (ClassDistributeSaved)
            {
                ClassDistributeFocused = true;
            }
            else
            {
                if (SelectedClassDistribute != null)
                {
                    InputClassDistribute = new()
                    {
                        Id = SelectedClassDistribute!.Id,
                        ClassId = SelectedClassDistribute.ClassId,
                        TeacherId = SelectedClassDistribute.TeacherId,
                        AssignDate = SelectedClassDistribute.AssignDate,
                        OnYear = SelectedClassDistribute.OnYear
                    };
                }
                else
                    InputClassDistribute = new();
            }
            ClassDistributeSaved = !ClassDistributeSaved;
            ClassDistributeFocused = false;
        }
        async Task DeleteClassDistribute()
        {
            if (InputClassDistribute != null)
            {
                NoticeBoxResult ans = NoticeBox.Show($"Bạn có chắc chắn xóa [{InputClassDistribute.ClassId}]?", "Thông báo", NoticeBoxButton.YesNo, NoticeBoxImage.Question);
                if (ans == NoticeBoxResult.No) return;

                DataProcess<DtoClassDistribute> dp = new();
                DtoResult<DtoClassDistribute> rs = await dp.DeleteAsync(InputClassDistribute);
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
                NoticeBox.Show($"Đã xóa [{InputClassDistribute.ClassId}]!");
                ClassDistributes = new(ClassDistributes.Where(x => x.Id != InputClassDistribute.Id));
                SelectedClassDistribute = ClassDistributes.FirstOrDefault()!;
            }
        }
        bool CanAddClassDistribute()
        {
            //return true;
            return (!ClassDistributeSaved && InputClassDistribute != null && InputClassDistribute.ClassId != null && InputClassDistribute.TeacherId != null && InputClassDistribute.AssignDate != null && !string.IsNullOrEmpty(InputClassDistribute.OnYear)) || ClassDistributeSaved;
        }
        bool CanEditClassDistribute()
        {
            return !ClassDistributeSaved || (ClassDistributeSaved && InputClassDistribute != null && InputClassDistribute.ClassId != null && InputClassDistribute.TeacherId != null && InputClassDistribute.AssignDate != null && !string.IsNullOrEmpty(InputClassDistribute.OnYear));
        }
        bool CanDeleteClassDistribute()
        {
            return ClassDistributeSaved && InputClassDistribute != null && InputClassDistribute.ClassId != null;
        }
        #endregion

        #region RelayCommands
        public RelayCommand AddClassDistributeCommand => new(async cmd => await AddClassDistribute(), canExecute => CanAddClassDistribute());
        public RelayCommand EditClassDistributeCommand => new(cmd => EditClassDistribute(), canExecute => CanEditClassDistribute());
        public RelayCommand DeleteClassDistributeCommand => new(async cmd => await DeleteClassDistribute(), canExecute => CanDeleteClassDistribute());
        public RelayCommand AddQuicklyClassDistributeCommand => new(async cmd => await AddCQuicklylassDistribute(), canExecute => CanAddClassDistribute());

        #endregion

        #region Properties
        private ObservableCollection<DtoClassDistribute> classDistributes = new();
        public ObservableCollection<DtoClassDistribute> ClassDistributes { get { return classDistributes; } set { classDistributes = value; OnPropertyChanged(); } }
        private DtoClassDistribute? selectedClassDistribute = new();
        public DtoClassDistribute? SelectedClassDistribute
        {
            get { return selectedClassDistribute; }
            set
            {
                selectedClassDistribute = value; OnPropertyChanged();
                if (value != null)
                {
                    InputClassDistribute = new()
                    {
                        Id = value.Id,
                        ClassId = value.ClassId,
                        TeacherId = value.TeacherId,
                        AssignDate = value.AssignDate,
                        OnYear = value.OnYear,
                        TypeList = value.TypeList
                    };
                    CboClass = CboClasses.FirstOrDefault(x => x.Id == value.ClassId)!;
                    CboTeacher = CboTeachers.FirstOrDefault(x => x.Id == value.TeacherId)!;
                    CboStartWeek = CboStartWeeks.FirstOrDefault(x => x.OnYear == value.OnYear)!;

                    StudentDistributes = new(AllStudentDistributes.Where(x => x.ClassDistributeId == value.Id));
                    SelectedStudentDistribute = StudentDistributes.FirstOrDefault();
                }
                else
                {
                    InputClassDistribute = new();
                    CboClass = null!;
                    CboTeacher = null!;
                    CboStartWeek = null!;
                    StudentDistributes = new();
                    SelectedStudentDistribute = null!;
                }
            }
        }
        private DtoClassDistribute inputClassDistribute = new();
        public DtoClassDistribute InputClassDistribute { get { return inputClassDistribute; } set { inputClassDistribute = value; OnPropertyChanged(); } }
        private bool classDistributeSaved = true;
        public bool ClassDistributeSaved { get { return classDistributeSaved; } set { classDistributeSaved = value; OnPropertyChanged(); } }
        private bool classDistributeFocused = false;
        public bool ClassDistributeFocused { get { return classDistributeFocused; } set { classDistributeFocused = value; OnPropertyChanged(); } }
        private ObservableCollection<DtoClassDistribute> cboClassDistributes = new();
        public ObservableCollection<DtoClassDistribute> CboClassDistributes { get { return cboClassDistributes; } set { cboClassDistributes = value; OnPropertyChanged(); } }
        private DtoClassDistribute cboClassDistribute = new();
        public DtoClassDistribute CboClassDistribute { get { return cboClassDistribute; } set { cboClassDistribute = value; OnPropertyChanged(); } }
        #endregion

        #region Methods
        async Task LoadStudentDistribute()
        {
            DataProcess<DtoStudentDistribute> dp = new();
            DtoResult<DtoStudentDistribute> rs = await dp.GetAllAsync(new DtoStudentDistribute());
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);

                AllStudentDistributes = new();
                //SelectedStudentDistribute = null!;
                return;

            }
            AllStudentDistributes = new(rs!.Results!);
            //SelectedStudentDistribute = StudentDistributes.FirstOrDefault()!;
        }
        async Task AddStudentDistribute()
        {
            if (StudentDistributeSaved)
            {
                InputStudentDistribute = new();
                StudentDistributeFocused = true;
            }
            else
            {
                DataProcess<DtoStudentDistribute> dp = new();
                DtoResult<DtoStudentDistribute> rs = await dp.GetOneAsync(new DtoStudentDistribute() { ClassDistributeId = InputStudentDistribute!.ClassDistributeId, StudentId = InputStudentDistribute.StudentId });
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                if (InputStudentDistribute.Id == null)
                {
                    if (rs!.Result != null)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.StudentId}]", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.AddAsync(InputStudentDistribute);

                    StudentDistributes.Add(rs.Result!);
                    StudentDistributes = new(StudentDistributes);
                }
                else
                {
                    if (rs!.Result != null && rs.Result.Id != InputStudentDistribute.Id)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.StudentId}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.UpdateAsync(InputStudentDistribute);
                    DtoStudentDistribute? dto = StudentDistributes.FirstOrDefault(x => x.Id == InputStudentDistribute.Id);
                    if (dto != null)
                    {
                        dto.ClassDistributeId = InputStudentDistribute.ClassDistributeId;
                        dto.StudentId = InputStudentDistribute.StudentId;
                        dto.AssignDate = InputStudentDistribute.AssignDate;
                    }
                    StudentDistributes = new(StudentDistributes);
                }
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                NoticeBox.Show($"Đã lưu [{rs!.Result!.StudentId}]!", "Thông báo", NoticeBoxImage.Information);
                if (StudentDistributes.Count > 0)
                    SelectedStudentDistribute = StudentDistributes.FirstOrDefault(x => x.Id == rs.Result.Id)!;
            }
            StudentDistributeSaved = !StudentDistributeSaved;
            StudentDistributeFocused = false;
        }
        void EditStudentDistribute()
        {
            if (StudentDistributeSaved)
            {
                StudentDistributeFocused = true;
            }
            else
            {
                if (SelectedStudentDistribute != null)
                {
                    InputStudentDistribute = new()
                    {
                        Id = SelectedStudentDistribute!.Id,
                        ClassDistributeId = SelectedStudentDistribute.ClassDistributeId,
                        StudentId = SelectedStudentDistribute.StudentId,
                        AssignDate = SelectedStudentDistribute.AssignDate
                    };
                }
                else
                    InputStudentDistribute = new();
            }
            StudentDistributeSaved = !StudentDistributeSaved;
            StudentDistributeFocused = false;
        }
        async Task DeleteStudentDistribute()
        {
            if (InputStudentDistribute != null)
            {
                NoticeBoxResult ans = NoticeBox.Show($"Bạn có chắc chắn xóa [{InputStudentDistribute.StudentId}]?", "Thông báo", NoticeBoxButton.YesNo, NoticeBoxImage.Question);
                if (ans == NoticeBoxResult.No) return;

                DataProcess<DtoStudentDistribute> dp = new();
                DtoResult<DtoStudentDistribute> rs = await dp.DeleteAsync(InputStudentDistribute);
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
                NoticeBox.Show($"Đã xóa [{InputStudentDistribute.StudentId}]!");
                StudentDistributes = new(StudentDistributes.Where(x => x.Id != InputStudentDistribute.Id));
                SelectedStudentDistribute = StudentDistributes.FirstOrDefault()!;
            }
        }
        bool CanAddStudentDistribute()
        {
            return (!StudentDistributeSaved && InputStudentDistribute != null && InputStudentDistribute.ClassDistributeId != null && InputStudentDistribute.StudentId != null && InputStudentDistribute.AssignDate != null) || StudentDistributeSaved;
        }
        bool CanEditStudentDistribute()
        {
            return !StudentDistributeSaved || (StudentDistributeSaved && InputStudentDistribute != null && InputStudentDistribute.ClassDistributeId != null && InputStudentDistribute.StudentId != null && InputStudentDistribute.AssignDate != null);
        }
        bool CanDeleteStudentDistribute()
        {
            return StudentDistributeSaved && InputStudentDistribute != null && InputStudentDistribute.Id != null;
        }
        #endregion

        #region RelayCommands
        public RelayCommand AddStudentDistributeCommand => new(async cmd => await AddStudentDistribute(), canExecute => CanAddStudentDistribute());
        public RelayCommand EditStudentDistributeCommand => new(cmd => EditStudentDistribute(), canExecute => CanEditStudentDistribute());
        public RelayCommand DeleteStudentDistributeCommand => new(async cmd => await DeleteStudentDistribute(), canExecute => CanDeleteStudentDistribute());
        #endregion

        #region Properties
        private ObservableCollection<DtoStudentDistribute> allStudentDistributes = new();
        public ObservableCollection<DtoStudentDistribute> AllStudentDistributes
        {
            get { return allStudentDistributes; }
            set
            {
                allStudentDistributes = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<DtoStudentDistribute> studentDistributes = new();
        public ObservableCollection<DtoStudentDistribute> StudentDistributes { get { return studentDistributes; } set { studentDistributes = value; OnPropertyChanged(); } }
        private DtoStudentDistribute? selectedStudentDistribute = new();
        public DtoStudentDistribute? SelectedStudentDistribute
        {
            get { return selectedStudentDistribute; }
            set
            {
                selectedStudentDistribute = value; OnPropertyChanged();
                if (value != null)
                {
                    InputStudentDistribute = new()
                    {
                        Id = value.Id,
                        ClassDistributeId = value.ClassDistributeId,
                        StudentId = value.StudentId,
                        AssignDate = value.AssignDate,
                        TypeList = value.TypeList
                    };
                }
                else
                {
                    InputStudentDistribute = new();
                }
            }
        }
        private DtoStudentDistribute inputStudentDistribute = new();
        public DtoStudentDistribute InputStudentDistribute { get { return inputStudentDistribute; } set { inputStudentDistribute = value; OnPropertyChanged(); } }
        private bool studentDistributeSaved = true;
        public bool StudentDistributeSaved { get { return studentDistributeSaved; } set { studentDistributeSaved = value; OnPropertyChanged(); } }
        private bool studentDistributeFocused = false;
        public bool StudentDistributeFocused { get { return studentDistributeFocused; } set { studentDistributeFocused = value; OnPropertyChanged(); } }
        private ObservableCollection<DtoStudentDistribute> cboStudentDistributes = new();
        public ObservableCollection<DtoStudentDistribute> CboStudentDistributes { get { return cboStudentDistributes; } set { cboStudentDistributes = value; OnPropertyChanged(); } }
        private DtoStudentDistribute cboStudentDistribute = new();
        public DtoStudentDistribute CboStudentDistribute { get { return cboStudentDistribute; } set { cboStudentDistribute = value; OnPropertyChanged(); } }
        #endregion
    }
}
