using EduManModel.Dtos;
using EduManModel;
using MVVM;
using SLHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using EduManDesktopApp.Assets;

namespace EduManDesktopApp.ViewModel
{
    public class DisciplineManagerViewModel : ViewModelBase
    {
        public DisciplineManagerViewModel()
        {
            Date = DateTime.Today;
            Task.Run(async () => await GetStartWeekUsed()).Wait();
            Task.Run(async () => await LoadStudentDistribute()).Wait();
            Task.Run(async () => await LoadClassDistribute()).Wait();
            Task.Run(async () => await LoadDiscipline()).Wait();
        }
        async Task GetStartWeekUsed()
        {
            DataProcess<DtoStartWeek> dp = new();
            DtoResult<DtoStartWeek> rs = await dp.GetOneAsync(new DtoStartWeek() { Used = true });

            StartWeekUsed = rs.Result!;
        }
        async Task LoadDiscipline()
        {
            DataProcess<DtoDiscipline> dp = new();
            DtoResult<DtoDiscipline> rs = await dp.GetAllAsync(new DtoDiscipline());
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                return;
            }
            AllDisciplines = new(rs!.Results!);
            Discipline1s = new(AllDisciplines.Where(x => x.ApplyFor != (int)EnumHelper.ApplyFor.Student));
            Discipline2s = new(AllDisciplines.Where(x => x.ApplyFor != (int)EnumHelper.ApplyFor.Class));
        }
        async Task LoadStudentDistribute()
        {
            DataProcess<DtoStudentDistribute> dp = new();
            DtoResult<DtoStudentDistribute> rs = await dp.GetAllAsync(new DtoStudentDistribute());
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);

                AllStudentDistributes = new();
                return;
            }
            AllStudentDistributes = new(rs!.Results!);
        }
        async Task LoadClassDistribute()
        {
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
        }
        private ObservableCollection<DtoClassDistribute> classDistributes = new();
        public ObservableCollection<DtoClassDistribute> ClassDistributes { get { return classDistributes; } set { classDistributes = value; OnPropertyChanged(); } }
        private DtoClassDistribute? selectedClassDistribute = new();
        public DtoClassDistribute? SelectedClassDistribute
        {
            get { return selectedClassDistribute; }
            set
            {
                selectedClassDistribute = value; 
                OnPropertyChanged();
                if (value != null)
                {                    
                    StudentDistributes = new(AllStudentDistributes.Where(x => x.ClassDistributeId == value.Id));
                    SelectedStudentDistribute = StudentDistributes.FirstOrDefault();
                }
                else
                {                   
                    StudentDistributes = new();
                    SelectedStudentDistribute = null!;
                }
            }
        }

        private DtoStartWeek startWeekUsed = new();
        public DtoStartWeek StartWeekUsed
        {
            get { return startWeekUsed; }
            set { startWeekUsed = value; OnPropertyChanged(); }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value; 
                OnPropertyChanged();
                if (value != DateTime.MinValue)
                {
                    Task.Run(async () => await LoadClassDiscipline()).Wait();
                }
            }
        }

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
                selectedStudentDistribute = value; 
                OnPropertyChanged();               
            }
        }


        private ObservableCollection<DtoDiscipline> allDisciplines = new();
        public ObservableCollection<DtoDiscipline> AllDisciplines { get { return allDisciplines; } set { allDisciplines = value; OnPropertyChanged(); } }

        private ObservableCollection<DtoDiscipline> discipline1s = new();
        public ObservableCollection<DtoDiscipline> Discipline1s { get { return discipline1s; } set { discipline1s = value; OnPropertyChanged(); } }
        private DtoDiscipline? selectedDiscipline1 = new();
        public DtoDiscipline? SelectedDiscipline1
        {
            get { return selectedDiscipline1; }
            set
            {
                selectedDiscipline1 = value; 
                OnPropertyChanged();                
            }
        }
        private ObservableCollection<DtoDiscipline> discipline2s = new();
        public ObservableCollection<DtoDiscipline> Discipline2s { get { return discipline2s; } set { discipline2s = value; OnPropertyChanged(); } }
        private DtoDiscipline? selectedDiscipline2 = new();
        public DtoDiscipline? SelectedDiscipline2
        {
            get { return selectedDiscipline2; }
            set
            {
                selectedDiscipline2 = value;
                OnPropertyChanged();
            }
        }

        #region Methods
        async Task LoadClassDiscipline()
        {
            DataProcess<DtoClassDiscipline> dp = new();
            DtoResult<DtoClassDiscipline> rs = await dp.FindAsync(new DtoClassDiscipline() { OnDate = Date });
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                return;
            }
            ClassDisciplines = new(rs!.Results!);
            SelectedClassDiscipline = ClassDisciplines.FirstOrDefault()!;
        }
        async Task AddClassDiscipline()
        {
            if (ClassDisciplineSaved)
            {
                InputClassDiscipline = new();
                ClassDisciplineFocused = true;
            }
            else
            {
                DataProcess<DtoClassDiscipline> dp = new();
                DtoResult<DtoClassDiscipline> rs = await dp.GetOneAsync(new DtoClassDiscipline() { ClassDistributeId = InputClassDiscipline!.ClassDistributeId });
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                if (InputClassDiscipline.Id == null)
                {
                    if (rs!.Result != null)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.ClassDistributeId}]", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.AddAsync(InputClassDiscipline);

                    ClassDisciplines.Add(rs.Result!);
                    ClassDisciplines = new(ClassDisciplines);
                }
                else
                {
                    if (rs!.Result != null && rs.Result.Id != InputClassDiscipline.Id)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.ClassDistributeId}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.UpdateAsync(InputClassDiscipline);
                    DtoClassDiscipline? dto = ClassDisciplines.FirstOrDefault(x => x.Id == InputClassDiscipline.Id);
                    if (dto != null)
                        dto.ClassDistributeId = InputClassDiscipline.ClassDistributeId;
                    ClassDisciplines = new(ClassDisciplines);
                }
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                NoticeBox.Show($"Đã lưu [{rs!.Result!.ClassDistributeId}]!", "Thông báo", NoticeBoxImage.Information);
                if (ClassDisciplines.Count > 0)
                    SelectedClassDiscipline = ClassDisciplines.FirstOrDefault(x => x.Id == rs.Result.Id)!;
            }
            ClassDisciplineSaved = !ClassDisciplineSaved;
            ClassDisciplineFocused = false;
        }
        void EditClassDiscipline()
        {
            if (ClassDisciplineSaved)
            {
                ClassDisciplineFocused = true;
            }
            else
            {
                if (SelectedClassDiscipline != null)
                {
                    InputClassDiscipline = new()
                    {
                        Id = SelectedClassDiscipline!.Id,
                        ClassDistributeId = SelectedClassDiscipline.ClassDistributeId
                    };
                }
                else
                    InputClassDiscipline = new();
            }
            ClassDisciplineSaved = !ClassDisciplineSaved;
            ClassDisciplineFocused = false;
        }
        async Task DeleteClassDiscipline()
        {
            if (InputClassDiscipline != null)
            {
                NoticeBoxResult ans = NoticeBox.Show($"Bạn có chắc chắn xóa [{InputClassDiscipline.ClassDistributeId}]?", "Thông báo", NoticeBoxButton.YesNo, NoticeBoxImage.Question);
                if (ans == NoticeBoxResult.No) return;

                DataProcess<DtoClassDiscipline> dp = new();
                DtoResult<DtoClassDiscipline> rs = await dp.DeleteAsync(InputClassDiscipline);
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
                NoticeBox.Show($"Đã xóa [{InputClassDiscipline.ClassDistributeId}]!");
                ClassDisciplines = new(ClassDisciplines.Where(x => x.Id != InputClassDiscipline.Id));
                SelectedClassDiscipline = ClassDisciplines.FirstOrDefault()!;
            }
        }
        bool CanAddClassDiscipline()
        {
            return (!ClassDisciplineSaved && InputClassDiscipline != null && InputClassDiscipline.ClassDistributeId != null) || ClassDisciplineSaved;
        }
        bool CanEditClassDiscipline()
        {
            return !ClassDisciplineSaved || (ClassDisciplineSaved && InputClassDiscipline != null && InputClassDiscipline.ClassDistributeId != null);
        }
        bool CanDeleteClassDiscipline()
        {
            return ClassDisciplineSaved && InputClassDiscipline != null && InputClassDiscipline.ClassDistributeId != null;
        }
        #endregion

        #region RelayCommands
        public RelayCommand AddClassDisciplineCommand => new(async cmd => await AddClassDiscipline(), canExecute => CanAddClassDiscipline());
        public RelayCommand EditClassDisciplineCommand => new(cmd => EditClassDiscipline(), canExecute => CanEditClassDiscipline());
        public RelayCommand DeleteClassDisciplineCommand => new(async cmd => await DeleteClassDiscipline(), canExecute => CanDeleteClassDiscipline());
        #endregion

        #region Properties
        private ObservableCollection<DtoClassDiscipline> classDisciplines = new();
        public ObservableCollection<DtoClassDiscipline> ClassDisciplines { get { return classDisciplines; } set { classDisciplines = value; OnPropertyChanged(); } }
        private DtoClassDiscipline? selectedClassDiscipline = new();
        public DtoClassDiscipline? SelectedClassDiscipline
        {
            get { return selectedClassDiscipline; }
            set
            {
                selectedClassDiscipline = value; OnPropertyChanged();
                if (value != null)
                {
                    InputClassDiscipline = new()
                    {
                        Id = value.Id,
                        ClassDistributeId = value.ClassDistributeId,
                        TypeList = value.TypeList
                    };
                }
                else
                {
                    InputClassDiscipline = new();
                }
            }
        }
        private DtoClassDiscipline inputClassDiscipline = new();
        public DtoClassDiscipline InputClassDiscipline { get { return inputClassDiscipline; } set { inputClassDiscipline = value; OnPropertyChanged(); } }
        private bool classDisciplineSaved = true;
        public bool ClassDisciplineSaved { get { return classDisciplineSaved; } set { classDisciplineSaved = value; OnPropertyChanged(); } }
        private bool classDisciplineFocused = false;
        public bool ClassDisciplineFocused { get { return classDisciplineFocused; } set { classDisciplineFocused = value; OnPropertyChanged(); } }
        private ObservableCollection<DtoClassDiscipline> cboClassDisciplines = new();
        public ObservableCollection<DtoClassDiscipline> CboClassDisciplines { get { return cboClassDisciplines; } set { cboClassDisciplines = value; OnPropertyChanged(); } }
        private DtoClassDiscipline cboClassDiscipline = new();
        public DtoClassDiscipline CboClassDiscipline { get { return cboClassDiscipline; } set { cboClassDiscipline = value; OnPropertyChanged(); } }
        #endregion
    }
}
