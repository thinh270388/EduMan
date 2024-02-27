using EduManModel;
using EduManModel.Dtos;
using MVVM;
using SLHelper;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace EduManDesktopApp.ViewModel
{
    public class DisciplineViewModel : ViewModelBase
    {
        public DisciplineViewModel()
        {
            Task.Run(async () => await LoadDisciplineGroup()).Wait();
            Task.Run(async () => await LoadDisciplineType()).Wait();
            Task.Run(async () => await LoadDiscipline()).Wait();
        }

        #region Methods
        async Task LoadDisciplineGroup()
        {
            DataProcess<DtoDisciplineGroup> dp = new();
            DtoResult<DtoDisciplineGroup> rs = await dp.GetAllAsync(new DtoDisciplineGroup());
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                return;
            }
            DisciplineGroups = new(rs!.Results!);
            SelectedDisciplineGroup = DisciplineGroups.FirstOrDefault()!;
            CboDisciplineGroups = new(rs.Results!);
        }
        async Task AddDisciplineGroup()
        {
            if (DisciplineGroupSaved)
            {
                InputDisciplineGroup = new();
                DisciplineGroupFocused = true;
            }
            else
            {
                DataProcess<DtoDisciplineGroup> dp = new();
                DtoResult<DtoDisciplineGroup> rs = await dp.GetOneAsync(new DtoDisciplineGroup() { DisciplineGroupName = InputDisciplineGroup!.DisciplineGroupName });
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                if (InputDisciplineGroup.Id == null)
                {
                    if (rs!.Result != null)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.DisciplineGroupName}]", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.AddAsync(InputDisciplineGroup);

                    DisciplineGroups.Add(rs.Result!);
                    DisciplineGroups = new(DisciplineGroups);
                }
                else
                {
                    if (rs!.Result != null && rs.Result.Id != InputDisciplineGroup.Id)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.DisciplineGroupName}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.UpdateAsync(InputDisciplineGroup);
                    DtoDisciplineGroup? dto = DisciplineGroups.FirstOrDefault(x => x.Id == InputDisciplineGroup.Id);
                    if (dto != null)
                        dto.DisciplineGroupName = InputDisciplineGroup.DisciplineGroupName;
                    DisciplineGroups = new(DisciplineGroups);
                }
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                NoticeBox.Show($"Đã lưu [{rs!.Result!.DisciplineGroupName}]!", "Thông báo", NoticeBoxImage.Information);
                if (DisciplineGroups.Count > 0)
                    SelectedDisciplineGroup = DisciplineGroups.FirstOrDefault(x => x.Id == rs.Result.Id)!;
            }
            DisciplineGroupSaved = !DisciplineGroupSaved;
            DisciplineGroupFocused = false;
        }
        void EditDisciplineGroup()
        {
            if (DisciplineGroupSaved)
            {
                DisciplineGroupFocused = true;
            }
            else
            {
                if (SelectedDisciplineGroup != null)
                {
                    InputDisciplineGroup = new()
                    {
                        Id = SelectedDisciplineGroup!.Id,
                        DisciplineGroupName = SelectedDisciplineGroup.DisciplineGroupName
                    };
                }
                else
                    InputDisciplineGroup = new();
            }
            DisciplineGroupSaved = !DisciplineGroupSaved;
            DisciplineGroupFocused = false;
        }
        async Task DeleteDisciplineGroup()
        {
            if (InputDisciplineGroup != null)
            {
                NoticeBoxResult ans = NoticeBox.Show($"Bạn có chắc chắn xóa [{InputDisciplineGroup.DisciplineGroupName}]?", "Thông báo", NoticeBoxButton.YesNo, NoticeBoxImage.Question);
                if (ans == NoticeBoxResult.No) return;

                DataProcess<DtoDisciplineGroup> dp = new();
                DtoResult<DtoDisciplineGroup> rs = await dp.DeleteAsync(InputDisciplineGroup);
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
                NoticeBox.Show($"Đã xóa [{InputDisciplineGroup.DisciplineGroupName}]!");
                DisciplineGroups = new(DisciplineGroups.Where(x => x.Id != InputDisciplineGroup.Id));
                SelectedDisciplineGroup = DisciplineGroups.FirstOrDefault()!;
            }
        }
        bool CanAddDisciplineGroup()
        {
            return !DisciplineGroupSaved && inputDisciplineGroup != null && !string.IsNullOrEmpty(inputDisciplineGroup.DisciplineGroupName) || DisciplineGroupSaved;
        }
        bool CanEditDisciplineGroup()
        {
            return !DisciplineGroupSaved || DisciplineGroupSaved && InputDisciplineGroup != null && !string.IsNullOrEmpty(InputDisciplineGroup.DisciplineGroupName);
        }
        bool CanDeleteDisciplineGroup()
        {
            return DisciplineGroupSaved && InputDisciplineGroup != null && !string.IsNullOrEmpty(InputDisciplineGroup.DisciplineGroupName);
        }

        async Task LoadDisciplineType()
        {
            DataProcess<DtoDisciplineType> dp = new();
            DtoResult<DtoDisciplineType> rs = await dp.GetAllAsync(new DtoDisciplineType());
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                return;
            }
            DisciplineTypes = new(rs!.Results!);
            SelectedDisciplineType = DisciplineTypes.FirstOrDefault()!;
            CboDisciplineTypes = new(rs.Results!);
        }
        async Task AddDisciplineType()
        {
            if (DisciplineTypeSaved) //ADD
            {
                InputDisciplineType = new();
                DisciplineTypeFocused = true;
            }
            else //SAVE
            {
                DataProcess<DtoDisciplineType> dp = new();
                DtoResult<DtoDisciplineType> rs = await dp.GetOneAsync(new DtoDisciplineType() { DisciplineTypeName = InputDisciplineType!.DisciplineTypeName });
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                if (InputDisciplineType.Id == null)  //NEW
                {
                    if (rs!.Result != null)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.DisciplineTypeName}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.AddAsync(InputDisciplineType);
                    DisciplineTypes.Add(rs.Result!);
                    DisciplineTypes = new(DisciplineTypes);
                }
                else //EDIT
                {
                    if (rs!.Result != null && rs!.Result.Id != InputDisciplineType.Id)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.DisciplineTypeName}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.UpdateAsync(InputDisciplineType);
                    DtoDisciplineType? dto = DisciplineTypes.FirstOrDefault(x => x.Id == InputDisciplineType.Id);
                    if (dto != null)
                        dto.DisciplineTypeName = InputDisciplineType.DisciplineTypeName;
                    DisciplineTypes = new(DisciplineTypes);
                }
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                NoticeBox.Show($"Đã lưu [{rs!.Result!.DisciplineTypeName}]!", "Thông báo", NoticeBoxImage.Information);
                if (DisciplineTypes.Count > 0)
                    SelectedDisciplineType = DisciplineTypes.FirstOrDefault(x => x.Id == rs.Result.Id)!;
            }
            DisciplineTypeSaved = !DisciplineTypeSaved;
            DisciplineTypeFocused = false;
        }
        void EditDisciplineType()
        {
            if (DisciplineTypeSaved)
            {
                DisciplineTypeFocused = true;
            }
            else
            {
                if (selectedDisciplineType != null)
                {
                    InputDisciplineType = new()
                    {
                        Id = SelectedDisciplineType!.Id,
                        DisciplineTypeName = SelectedDisciplineType.DisciplineTypeName
                    };
                }
                else
                    InputDisciplineType = new();
            }
            DisciplineTypeSaved = !DisciplineTypeSaved;
            DisciplineTypeFocused = false;
        }
        async Task DeleteDisciplineType()
        {
            if (InputDisciplineType != null)
            {
                NoticeBoxResult ans = NoticeBox.Show($"Bạn có chắc chắn xóa [{InputDisciplineType.DisciplineTypeName}]?", "Thông báo", NoticeBoxButton.YesNo, NoticeBoxImage.Question);
                if (ans == NoticeBoxResult.No) return;

                DataProcess<DtoDisciplineType> dp = new();
                DtoResult<DtoDisciplineType> rs = await dp.DeleteAsync(InputDisciplineType);
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
                NoticeBox.Show($"Đã xóa [{InputDisciplineType.DisciplineTypeName}]!");
                DisciplineTypes = new(DisciplineTypes.Where(x => x.Id != InputDisciplineType.Id));
                SelectedDisciplineType = DisciplineTypes.FirstOrDefault()!;
            }
        }
        bool CanAddDisciplineType()
        {
            return !DisciplineTypeSaved && InputDisciplineType != null && !string.IsNullOrEmpty(InputDisciplineType.DisciplineTypeName) || DisciplineTypeSaved;
        }
        bool CanEditDisciplineType()
        {
            return !DisciplineTypeSaved || DisciplineTypeSaved && InputDisciplineType != null && !string.IsNullOrEmpty(InputDisciplineType.DisciplineTypeName);
        }
        bool CanDeleteDisciplineType()
        {
            return DisciplineTypeSaved && InputDisciplineType != null && !string.IsNullOrEmpty(InputDisciplineType.DisciplineTypeName);
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
            Disciplines = new(rs!.Results!);
            SelectedDiscipline = Disciplines.FirstOrDefault()!;
        }
        async Task AddDiscipline()
        {
            if (DisciplineSaved) //ADD
            {
                InputDiscipline = new();
                CboDisciplineGroup = null!;
                CboDisciplineType = null!;
                DisciplineFocused = true;
            }
            else //SAVE
            {
                // CHECK NULL BEFORE ADD OR EDIT
                if (string.IsNullOrEmpty(InputDiscipline.DisciplineName)
                        || InputDiscipline.ApplyFor == null
                        || InputDiscipline.DisciplineGroupId == null
                        || InputDiscipline.DisciplineTypeId == null
                        || InputDiscipline.PlusPoint == null
                        || InputDiscipline.MinusPoint == null
                        || InputDiscipline.Display == null)
                {
                    NoticeBox.Show("Nhập thông tin chưa đầy đủ!", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                DataProcess<DtoDiscipline> dp = new();
                DtoResult<DtoDiscipline> rs = await dp.GetOneAsync(new DtoDiscipline() { DisciplineName = InputDiscipline!.DisciplineName });
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                if (InputDiscipline.Id == null)  //NEW
                {
                    if (rs!.Result != null)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.DisciplineName}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.AddAsync(InputDiscipline);
                    if (rs == null || (rs != null && rs.Message != "OK"))
                    {
                        if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                        else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    Disciplines.Add(rs!.Result!);
                    Disciplines = new(Disciplines);
                }
                else //EDIT
                {
                    if (rs!.Result != null && rs.Result.Id != InputDiscipline.Id)
                    {
                        NoticeBox.Show($"Đã tồn tại {rs.Result.DisciplineName}", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.UpdateAsync(InputDiscipline);
                    if (rs == null || (rs != null && rs.Message != "OK"))
                    {
                        if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                        else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    DtoDiscipline? dto = Disciplines.FirstOrDefault(x => x.Id == InputDiscipline.Id);
                    if (dto != null)
                    {
                        dto.DisciplineName = InputDiscipline.DisciplineName;
                        dto.DisciplineGroupId = InputDiscipline.DisciplineGroupId;
                        dto.ApplyFor = InputDiscipline.ApplyFor;
                        dto.Display = InputDiscipline.Display;
                        dto.DisciplineTypeId = InputDiscipline.DisciplineTypeId;
                        dto.PlusPoint = InputDiscipline.PlusPoint;
                        dto.MinusPoint = InputDiscipline.MinusPoint;
                        dto.SequenceNumber = InputDiscipline.SequenceNumber;
                        dto.Note = InputDiscipline.Note;
                    }
                    Disciplines = new(Disciplines);
                }
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                NoticeBox.Show($"Đã lưu [{rs!.Result!.DisciplineName}]!", "Thông báo", NoticeBoxImage.Information);
                if (Disciplines.Count > 0)
                    SelectedDiscipline = Disciplines.FirstOrDefault(x => x.Id == rs.Result.Id)!;
            }
            DisciplineSaved = !DisciplineSaved;
            DisciplineFocused = false;
        }
        void EditDiscipline()
        {
            if (DisciplineSaved)
            {
                DisciplineFocused = true;
            }
            else
            {
                if (SelectedDiscipline != null)
                {
                    InputDiscipline = new()
                    {
                        Id = SelectedDiscipline.Id,
                        DisciplineName = SelectedDiscipline.DisciplineName,
                        ApplyFor = SelectedDiscipline.ApplyFor,
                        DisciplineGroupId = SelectedDiscipline.DisciplineGroupId,
                        DisciplineTypeId = SelectedDiscipline.DisciplineTypeId,
                        Display = SelectedDiscipline.Display,
                        MinusPoint = SelectedDiscipline.MinusPoint,
                        PlusPoint = SelectedDiscipline.PlusPoint,
                        Note = SelectedDiscipline.Note,
                        SequenceNumber = SelectedDiscipline.SequenceNumber
                    };
                }
                else
                    InputDiscipline = new();
            }
            DisciplineSaved = !DisciplineSaved;
            DisciplineFocused = false;
        }
        async Task DeleteDiscipline()
        {
            if (InputDiscipline != null)
            {
                NoticeBoxResult ans = NoticeBox.Show($"Bạn có chắc chắn xóa [{InputDiscipline.DisciplineName}]?", "Thông báo", NoticeBoxButton.YesNo, NoticeBoxImage.Question);
                if (ans == NoticeBoxResult.No) return;

                DataProcess<DtoDiscipline> dp = new();
                DtoResult<DtoDiscipline> rs = await dp.DeleteAsync(InputDiscipline);
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
                NoticeBox.Show($"Đã xóa [{InputDiscipline.DisciplineName}]!");
                Disciplines = new(Disciplines.Where(x => x.Id != InputDiscipline.Id));
                SelectedDiscipline = Disciplines.FirstOrDefault()!;
            }
        }
        bool CanAddDiscipline()
        {
            return !DisciplineSaved && InputDiscipline != null && !string.IsNullOrEmpty(InputDiscipline.DisciplineName) || DisciplineSaved;
        }
        bool CanEditDiscipline()
        {
            return !DisciplineSaved || DisciplineSaved && InputDiscipline != null && !string.IsNullOrEmpty(InputDiscipline.DisciplineName);
        }
        bool CanDeleteDiscipline()
        {
            return DisciplineSaved && InputDiscipline != null && !string.IsNullOrEmpty(InputDiscipline.DisciplineName);
        }
        #endregion

        #region RelayCommand
        public RelayCommand AddDisciplineGroupCommand => new(async cmd => await AddDisciplineGroup(), canExecute => CanAddDisciplineGroup());
        public RelayCommand EditDisciplineGroupCommand => new(cmd => EditDisciplineGroup(), canExecute => CanEditDisciplineGroup());
        public RelayCommand DeleteDisciplineGroupCommand => new(async cmd => await DeleteDisciplineGroup(), canExecute => CanDeleteDisciplineGroup());

        public RelayCommand AddDisciplineTypeCommand => new(async cmd => await AddDisciplineType(), canExecute => CanAddDisciplineType());
        public RelayCommand EditDisciplineTypeCommand => new(cmd => EditDisciplineType(), canExecute => CanEditDisciplineType());
        public RelayCommand DeleteDisciplineTypeCommand => new(async cmd => await DeleteDisciplineType(), canExecute => CanDeleteDisciplineType());

        public RelayCommand AddDisciplineCommand => new(async cmd => await AddDiscipline(), canExecute => CanAddDiscipline());
        public RelayCommand EditDisciplineCommand => new(cmd => EditDiscipline(), canExecute => CanEditDiscipline());
        public RelayCommand DeleteDisciplineCommand => new(async cmd => await DeleteDiscipline(), canExecute => CanDeleteDiscipline());
        #endregion

        #region Property
        private ObservableCollection<DtoDisciplineGroup> disciplineGroups = new();
        public ObservableCollection<DtoDisciplineGroup> DisciplineGroups { get { return disciplineGroups; } set { disciplineGroups = value; OnPropertyChanged(); } }
        private DtoDisciplineGroup? selectedDisciplineGroup = new();
        public DtoDisciplineGroup? SelectedDisciplineGroup
        {
            get { return selectedDisciplineGroup; }
            set
            {
                selectedDisciplineGroup = value; OnPropertyChanged();
                if (value != null)
                {
                    InputDisciplineGroup = new()
                    {
                        Id = value.Id,
                        DisciplineGroupName = value.DisciplineGroupName,
                        TypeList = value.TypeList
                    };
                }
                else
                {
                    InputDisciplineGroup = new();
                }
            }
        }
        private DtoDisciplineGroup inputDisciplineGroup = new();
        public DtoDisciplineGroup InputDisciplineGroup { get { return inputDisciplineGroup; } set { inputDisciplineGroup = value; OnPropertyChanged(); } }
        private bool disciplineGroupSaved = true;
        public bool DisciplineGroupSaved { get { return disciplineGroupSaved; } set { disciplineGroupSaved = value; OnPropertyChanged(); } }
        private bool disciplineGroupFocused = false;
        public bool DisciplineGroupFocused { get { return disciplineGroupFocused; } set { disciplineGroupFocused = value; OnPropertyChanged(); } }
        private ObservableCollection<DtoDisciplineGroup> cboDisciplineGroups = new();
        public ObservableCollection<DtoDisciplineGroup> CboDisciplineGroups { get { return cboDisciplineGroups; } set { cboDisciplineGroups = value; OnPropertyChanged(); } }
        private DtoDisciplineGroup cboDisciplineGroup = new();
        public DtoDisciplineGroup CboDisciplineGroup { get { return cboDisciplineGroup; } set { cboDisciplineGroup = value; OnPropertyChanged(); InputDiscipline.DisciplineGroupId = value != null && value.Id != null ? value.Id : null; } }

        private ObservableCollection<DtoDisciplineType> disciplineTypes = new();
        public ObservableCollection<DtoDisciplineType> DisciplineTypes { get { return disciplineTypes; } set { disciplineTypes = value; OnPropertyChanged(); } }
        private DtoDisciplineType? selectedDisciplineType = new();
        public DtoDisciplineType? SelectedDisciplineType
        {
            get { return selectedDisciplineType; }
            set
            {
                selectedDisciplineType = value; OnPropertyChanged();
                if (value != null)
                {
                    InputDisciplineType = new()
                    {
                        Id = value.Id,
                        DisciplineTypeName = value.DisciplineTypeName,
                        TypeList = value.TypeList
                    };
                }
                else
                {
                    InputDisciplineType = new();
                }
            }
        }
        private DtoDisciplineType inputDisciplineType = new();
        public DtoDisciplineType InputDisciplineType { get { return inputDisciplineType; } set { inputDisciplineType = value; OnPropertyChanged(); } }
        private bool disciplineTypeSaved = true;
        public bool DisciplineTypeSaved { get { return disciplineTypeSaved; } set { disciplineTypeSaved = value; OnPropertyChanged(); } }
        private bool disciplineTypeFocused = false;
        public bool DisciplineTypeFocused { get { return disciplineTypeFocused; } set { disciplineTypeFocused = value; OnPropertyChanged(); } }
        private ObservableCollection<DtoDisciplineType> cboDisciplineTypes = new();
        public ObservableCollection<DtoDisciplineType> CboDisciplineTypes { get { return cboDisciplineTypes; } set { cboDisciplineTypes = value; OnPropertyChanged(); } }
        private DtoDisciplineType cboDisciplineType = new();
        public DtoDisciplineType CboDisciplineType { get { return cboDisciplineType; } set { cboDisciplineType = value; OnPropertyChanged(); InputDiscipline.DisciplineTypeId = value != null && value.Id != null ? value.Id : null; } }

        private ObservableCollection<DtoDiscipline> disciplines = new();
        public ObservableCollection<DtoDiscipline> Disciplines { get { return disciplines; } set { disciplines = value; OnPropertyChanged(); } }
        private DtoDiscipline? selectedDiscipline = new();
        public DtoDiscipline? SelectedDiscipline
        {
            get { return selectedDiscipline; }
            set
            {
                selectedDiscipline = value; OnPropertyChanged();
                if (value != null)
                {
                    InputDiscipline = new()
                    {
                        Id = value.Id,
                        DisciplineName = value.DisciplineName,
                        DisciplineGroupId = value.DisciplineGroupId,
                        ApplyFor = value.ApplyFor,
                        PlusPoint = value.PlusPoint,
                        MinusPoint = value.MinusPoint,
                        Display = value.Display,
                        DisciplineTypeId = value.DisciplineTypeId,
                        SequenceNumber = value.SequenceNumber,
                        Note = value.Note,
                        TypeList = value.TypeList
                    };
                    CboDisciplineGroup = CboDisciplineGroups.FirstOrDefault(x => x.Id == value.DisciplineGroupId)!;
                    CboDisciplineType = CboDisciplineTypes.FirstOrDefault(x => x.Id == value.DisciplineTypeId)!;
                }
                else
                {
                    InputDiscipline = new();
                    CboDisciplineGroup = null!;
                    CboDisciplineType = null!;
                }
            }
        }
        private DtoDiscipline inputDiscipline = new();
        public DtoDiscipline InputDiscipline { get { return inputDiscipline; } set { inputDiscipline = value; OnPropertyChanged(); } }
        private bool disciplineSaved = true;
        public bool DisciplineSaved { get { return disciplineSaved; } set { disciplineSaved = value; OnPropertyChanged(); } }
        private bool disciplineFocused = false;
        public bool DisciplineFocused { get { return disciplineFocused; } set { disciplineFocused = value; OnPropertyChanged(); } }
        #endregion
    }
}