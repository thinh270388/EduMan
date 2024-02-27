using EduManModel;
using EduManModel.Dtos;
using MVVM;
using SLHelper;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EduManDesktopApp.ViewModel
{
    public class StudentViewModel : ViewModelBase
    {
        public StudentViewModel()
        {
            Task.Run(async () => await LoadLevel()).Wait();
            Task.Run(async () => await LoadGrade()).Wait();
            Task.Run(async () => await LoadClass()).Wait();
            Task.Run(async () => await LoadStudent()).Wait();
        }

        #region Methods
        async Task LoadLevel()
        {
            DataProcess<DtoLevel> dp = new();
            DtoResult<DtoLevel> rs = await dp.GetAllAsync(new DtoLevel());
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);

                Levels = new();
                CboLevels = new();
                SelectedLevel = null!;
                return;
            }
            Levels = new(rs!.Results!);
            CboLevels = new(rs.Results!);
            SelectedLevel = Levels.FirstOrDefault()!;
        }
        async Task LoadGrade()
        {
            DataProcess<DtoGrade> dp = new();
            DtoResult<DtoGrade> rs = await dp.GetAllAsync(new DtoGrade());
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);

                Grades = new();
                SelectedGrade = null!;
                CboGrades = new();
                return;
            }
            Grades = new(rs!.Results!);
            CboGrades = new(rs.Results!);
            SelectedGrade = Grades.FirstOrDefault()!;
        }
        async Task LoadClass()
        {
            DataProcess<DtoClass> dp = new();
            DtoResult<DtoClass> rs = await dp.GetAllAsync(new DtoClass());
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);

                Classes = new();
                SelectedClass = null!;
                return;
            }
            Classes = new(rs!.Results!);
            SelectedClass = Classes.FirstOrDefault()!;
        }
        async Task LoadStudent()
        {
            DataProcess<DtoStudent> dp = new();
            DtoResult<DtoStudent> rs = await dp.GetAllAsync(new DtoStudent());
            if (rs == null || (rs != null && rs.Message != "OK"))
            {
                if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);

                Students = new();
                return;
            }
            Students = new(rs!.Results!);
            SelectedStudent = Students.FirstOrDefault()!;
        }

        async Task AddLevel()
        {
            if (LevelSaved)
            {
                InputLevel = new();
                LevelFocused = true;
            }
            else
            {
                DataProcess<DtoLevel> dp = new();
                DtoResult<DtoLevel> rs = await dp.GetOneAsync(new DtoLevel() { LevelName = InputLevel!.LevelName });
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                if (InputLevel.Id == null)
                {
                    if (rs!.Result != null)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.LevelName}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.AddAsync(InputLevel);
                    if (rs == null || (rs != null && rs.Message != "OK"))
                    {
                        if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                        else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    Levels.Add(rs!.Result!);
                    Levels = new(Levels);
                    CboLevels = new(Levels);
                }
                else
                {
                    if (rs!.Result != null && rs.Result.Id != InputLevel.Id)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.LevelName}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.UpdateAsync(InputLevel);
                    if (rs == null || (rs != null && rs.Message != "OK"))
                    {
                        if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                        else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    DtoLevel? dto = Levels.FirstOrDefault(x => x.Id == InputLevel.Id);
                    if (dto != null)
                        dto.LevelName = InputLevel.LevelName;
                    Levels = new(Levels);
                }
                NoticeBox.Show($"Đã lưu [{rs!.Result!.LevelName}]!", "Thông báo", NoticeBoxImage.Information);
                if (Levels.Count > 0)
                    SelectedLevel = Levels.FirstOrDefault(x => x.Id == rs.Result.Id)!;
            }
            LevelSaved = !LevelSaved;
            LevelFocused = false;
        }
        void EditLevel()
        {
            if (LevelSaved)
            {
                LevelFocused = true;
            }
            else
            {
                if (SelectedLevel != null)
                {
                    InputLevel = new()
                    {
                        Id = SelectedLevel.Id,
                        LevelName = SelectedLevel.LevelName
                    };
                }
                else
                    InputLevel = new();
            }
            LevelSaved = !LevelSaved;
            LevelFocused = false;
        }
        async Task DeleteLevel()
        {
            if (InputLevel != null)
            {
                NoticeBoxResult ans = NoticeBox.Show($"Bạn có chắc chắn xóa [{InputLevel.LevelName}]?", "Thông báo", NoticeBoxButton.YesNo, NoticeBoxImage.Question);
                if (ans == NoticeBoxResult.No) return;

                DataProcess<DtoLevel> dp = new();
                DtoResult<DtoLevel> rs = await dp.DeleteAsync(InputLevel);
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
                NoticeBox.Show($"Đã xóa [{InputLevel.LevelName}]!");
                Levels = new(Levels.Where(x => x.Id != InputLevel.Id));
                SelectedLevel = Levels.FirstOrDefault()!;
            }
        }
        bool CanAddLevel()
        {
            return ClassSaved && GradeSaved &&
                (!LevelSaved && InputLevel != null && !string.IsNullOrEmpty(InputLevel.LevelName) || LevelSaved);
        }
        bool CanEditLevel()
        {
            return ClassSaved && GradeSaved &&
               (!LevelSaved || LevelSaved && InputLevel != null && !string.IsNullOrEmpty(InputLevel.LevelName));
        }
        bool CanDeleteLevel()
        {
            return ClassSaved && GradeSaved && LevelSaved && InputLevel != null && !string.IsNullOrEmpty(InputLevel.LevelName);
        }

        async Task AddGrade()
        {
            if (GradeSaved)
            {
                InputGrade = new();
                CboLevel = null!;
                GradeFocused = true;
            }
            else
            {
                DataProcess<DtoGrade> dp = new();
                DtoResult<DtoGrade> rs = await dp.GetOneAsync(new DtoGrade() { GradeName = InputGrade!.GradeName });
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                if (InputGrade.Id == null)
                {
                    if (rs!.Result != null)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.GradeName}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.AddAsync(InputGrade);
                    if (rs == null || (rs != null && rs.Message != "OK"))
                    {
                        if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                        else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    Grades.Add(rs!.Result!);
                    Grades = new(Grades);
                    CboGrades = new(Grades);
                }
                else
                {
                    if (rs!.Result != null && rs.Result.Id != InputLevel.Id)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs!.Result.GradeName}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.UpdateAsync(InputGrade);
                    if (rs == null || (rs != null && rs.Message != "OK"))
                    {
                        if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                        else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    DtoGrade? dto = Grades.FirstOrDefault(x => x.Id == InputGrade.Id);
                    if (dto != null)
                        dto.GradeName = InputGrade.GradeName;
                    Grades = new(Grades);
                    CboGrades = new(Grades);
                }
                NoticeBox.Show($"Đã lưu [{rs!.Result!.GradeName}]!", "Thông báo", NoticeBoxImage.Information);
                if (Grades.Count > 0)
                    SelectedGrade = Grades.FirstOrDefault(x => x.Id == rs.Result.Id)!;
            }
            GradeSaved = !GradeSaved;
            GradeFocused = false;
        }
        void EditGrade()
        {
            if (GradeSaved)
            {
                GradeFocused = true;
            }
            else
            {
                if (SelectedGrade != null)
                {
                    InputGrade = new()
                    {
                        Id = SelectedGrade.Id,
                        GradeName = SelectedGrade.GradeName,
                        LevelId = SelectedGrade.LevelId,
                        TypeList = SelectedGrade.TypeList
                    };
                }
                else
                    InputGrade = new();
            }
            GradeSaved = !GradeSaved;
            GradeFocused = false;
        }
        async Task DeleteGrade()
        {
            if (InputLevel != null)
            {
                NoticeBoxResult ans = NoticeBox.Show($"Bạn có chắc chắn xóa [{InputGrade.GradeName}]?", "Thông báo", NoticeBoxButton.YesNo, NoticeBoxImage.Question);
                if (ans == NoticeBoxResult.No) return;

                DataProcess<DtoGrade> dp = new();
                DtoResult<DtoGrade> rs = await dp.DeleteAsync(InputGrade);
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
                NoticeBox.Show($"Đã xóa [{InputGrade.GradeName}]!");
                Grades = new(Grades.Where(x => x.Id != InputGrade.Id));
                SelectedGrade = Grades.FirstOrDefault()!;
            }
        }
        bool CanAddGrade()
        {
            return ClassSaved && LevelSaved &&
               (!GradeSaved && InputGrade != null && !string.IsNullOrEmpty(InputGrade.GradeName) && InputGrade.LevelId != null || GradeSaved);
        }
        bool CanEditGrade()
        {
            return ClassSaved && LevelSaved &&
               (!GradeSaved || GradeSaved && InputGrade != null && !string.IsNullOrEmpty(InputGrade.GradeName));
        }
        bool CanDeleteGrade()
        {
            return ClassSaved && LevelSaved &&
               (!GradeSaved && InputGrade != null && !string.IsNullOrEmpty(InputGrade.GradeName) && InputGrade.LevelId != null || GradeSaved);
        }

        async Task AddClass()
        {
            if (ClassSaved)
            {
                InputClass = new();
                CboGrade = null!;
                ClassFocused = true;
            }
            else
            {
                DataProcess<DtoClass> dp = new();
                DtoResult<DtoClass> rs = await dp.GetOneAsync(new DtoClass() { ClassName = InputClass!.ClassName });
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                if (InputClass.Id == null)
                {
                    if (rs!.Result != null)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.ClassName}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.AddAsync(InputClass);
                    if (rs == null || (rs != null && rs.Message != "OK"))
                    {
                        if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                        else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    Classes.Add(rs!.Result!);
                    Classes = new(Classes);
                }
                else
                {
                    if (rs!.Result != null && rs.Result.Id != InputLevel.Id)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs!.Result.ClassName}]", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.UpdateAsync(InputClass);
                    if (rs == null || (rs != null && rs.Message != "OK"))
                    {
                        if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                        else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    DtoClass? dto = Classes.FirstOrDefault(x => x.Id == InputClass.Id);
                    if (dto != null)
                    {
                        dto.ClassName = InputClass.ClassName;
                        dto.GradeId = InputClass.GradeId;
                    }
                    Classes = new(Classes);
                }
                NoticeBox.Show($"Đã lưu [{rs!.Result!.ClassName}]!", "Thông báo", NoticeBoxImage.Information);
                if (Classes.Count > 0)
                    SelectedClass = Classes.FirstOrDefault(x => x.Id == rs.Result.Id)!;
            }
            ClassSaved = !ClassSaved;
            ClassFocused = false;
        }
        void EditClass()
        {
            if (ClassSaved)
            {
                ClassFocused = true;
            }
            else
            {
                if (SelectedClass != null)
                {
                    InputClass = new()
                    {
                        Id = SelectedClass.Id,
                        ClassName = SelectedClass.ClassName,
                        GradeId = SelectedClass.GradeId,
                        TypeList = SelectedClass.TypeList
                    };
                }
                else
                    InputClass = new();
            }
            ClassSaved = !ClassSaved;
            ClassFocused = false;
        }
        async Task DeleteClass()
        {
            if (InputLevel != null)
            {
                NoticeBoxResult ans = NoticeBox.Show($"Bạn có chắc chắn xóa [{InputClass.ClassName}]?", "Thông báo", NoticeBoxButton.YesNo, NoticeBoxImage.Question);
                if (ans == NoticeBoxResult.No) return;

                DataProcess<DtoClass> dp = new();
                DtoResult<DtoClass> rs = await dp.DeleteAsync(InputClass);
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
                NoticeBox.Show($"Đã xóa [{InputClass.ClassName}]!");
                Classes = new(Classes.Where(x => x.Id != InputClass.Id));
                SelectedClass = Classes.FirstOrDefault()!;
            }
        }
        bool CanAddClass()
        {
            return GradeSaved && LevelSaved &&
               (!ClassSaved && InputClass != null && !string.IsNullOrEmpty(InputClass.ClassName) && InputClass.GradeId != null || ClassSaved);
        }
        bool CanEditClass()
        {
            return GradeSaved && LevelSaved &&
               (!ClassSaved || ClassSaved && InputClass != null && !string.IsNullOrEmpty(InputClass.ClassName));
        }
        bool CanDeleteClass()
        {
            return GradeSaved && LevelSaved &&
              (!ClassSaved && InputClass != null && !string.IsNullOrEmpty(InputClass.ClassName) && InputClass.GradeId != null || ClassSaved);
        }

        async Task AddStudent()
        {
            if (StudentSaved)
            {
                InputStudent = new();
                StudentFocused = true;
            }
            else
            {
                DataProcess<DtoStudent> dp = new();
                DtoResult<DtoStudent> rs = await dp.GetOneAsync(new DtoStudent() { Code = InputStudent!.Code });
                if (rs == null || (rs != null && rs.Message != "OK"))
                {
                    if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                    else NoticeBox.Show($"Có lỗi xảy ra\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                    return;
                }
                if (InputStudent.Id == null)
                {
                    if (rs!.Result != null)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.Code}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.AddAsync(InputStudent);
                    if (rs == null || (rs != null && rs.Message != "OK"))
                    {
                        if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                        else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    Students.Add(rs!.Result!);
                    Students = new(Students);
                }
                else
                {
                    if (rs!.Result != null && rs.Result.Id != InputStudent.Id)
                    {
                        NoticeBox.Show($"Đã tồn tại [{rs.Result.Code}]!", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    rs = await dp.UpdateAsync(InputStudent);
                    if (rs == null || (rs != null && rs.Message != "OK"))
                    {
                        if (rs == null) NoticeBox.Show("Mất kết nối với máy chủ!", "Thông báo", NoticeBoxImage.Error);
                        else NoticeBox.Show($"Có lỗi xảy ra!\r\n{rs.Message}", "Thông báo", NoticeBoxImage.Error);
                        return;
                    }
                    DtoStudent? dto = Students.FirstOrDefault(x => x.Id == InputStudent.Id);
                    if (dto != null)
                    {
                        dto.Code = InputStudent.Code;
                        dto.FullName = InputStudent.FullName;
                        dto.Email = InputStudent.Email;
                        dto.Phone = InputStudent.Phone;
                        dto.Birthday = InputStudent.Birthday;
                        dto.AddressCurrent = InputStudent.AddressCurrent;
                        dto.ContactInfo = InputStudent.ContactInfo;
                        dto.Note = InputStudent.Note;
                        dto.SequenceNumber = InputStudent.SequenceNumber;
                        dto.Gender = InputStudent.Gender;
                    }
                    Students = new(Students);
                }
                NoticeBox.Show($"Đã lưu [{rs!.Result!.Code}]!", "Thông báo", NoticeBoxImage.Information);
                if (Students.Count > 0)
                    SelectedStudent = Students.FirstOrDefault(x => x.Id == rs.Result.Id)!;
            }
            StudentSaved = !StudentSaved;
            StudentFocused = false;
        }
        void EditStudent()
        {
            if (StudentSaved)
            {
                StudentFocused = true;
            }
            else
            {
                if (SelectedStudent != null)
                {
                    InputStudent = new()
                    {
                        Id = SelectedStudent.Id,
                        Code = SelectedStudent.Code,
                        Gender = SelectedStudent.Gender,
                        SequenceNumber = SelectedStudent.SequenceNumber,
                        Note = SelectedStudent.Note,
                        ContactInfo = SelectedStudent.ContactInfo,
                        AddressCurrent = SelectedStudent.AddressCurrent,
                        Birthday = SelectedStudent.Birthday,
                        FullName = SelectedStudent.FullName,
                        Email = SelectedStudent.Email,
                        Phone = SelectedStudent.Phone,
                        TypeList = SelectedStudent.TypeList
                    };
                }
                else
                    InputStudent = new();
            }
            StudentSaved = !StudentSaved;
            StudentFocused = false;
        }
        async Task DeleteStudent()
        {
            if (InputStudent != null)
            {
                NoticeBoxResult ans = NoticeBox.Show($"Bạn có chắc chắn xóa [{InputStudent.Code}]?", "Thông báo", NoticeBoxButton.YesNo, NoticeBoxImage.Question);
                if (ans == NoticeBoxResult.No) return;

                DataProcess<DtoStudent> dp = new();
                DtoResult<DtoStudent> rs = await dp.DeleteAsync(InputStudent);
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
                NoticeBox.Show($"Đã xóa [{InputStudent.Code}]!");
                Students = new(Students.Where(x => x.Id != InputStudent.Id));
                SelectedStudent = Students.FirstOrDefault()!;
            }
        }
        bool CanAddStudent()
        {
            return (!StudentSaved && 
                    InputStudent != null && 
                    !string.IsNullOrEmpty(InputStudent.Code) &&
                    !string.IsNullOrEmpty(InputStudent.FullName)
                    ) 
                    || StudentSaved;
        }
        bool CanEditStudent() { return !StudentSaved || StudentSaved && InputStudent != null && !string.IsNullOrEmpty(InputStudent.Code); }
        bool CanDeleteStudent() { return StudentSaved && InputStudent != null && !string.IsNullOrEmpty(InputStudent.Code); }
        #endregion

        #region RelayCommands
        public RelayCommand AddLevelCommand => new(async cmd => await AddLevel(), canExecute => CanAddLevel());
        public RelayCommand EditLevelCommand => new(cmd => EditLevel(), canExecute => CanEditLevel());
        public RelayCommand DeleteLevelCommand => new(async cmd => await DeleteLevel(), canExecute => CanDeleteLevel());

        public RelayCommand AddGradeCommand => new(async cmd => await AddGrade(), canExecute => CanAddGrade());
        public RelayCommand EditGradeCommand => new(cmd => EditGrade(), canExecute => CanEditGrade());
        public RelayCommand DeleteGradeCommand => new(async cmd => await DeleteGrade(), canExecute => CanDeleteGrade());

        public RelayCommand AddClassCommand => new(async cmd => await AddClass(), canExecute => CanAddClass());
        public RelayCommand EditClassCommand => new(cmd => EditClass(), canExecute => CanEditClass());
        public RelayCommand DeleteClassCommand => new(async cmd => await DeleteClass(), canExecute => CanDeleteClass());

        public RelayCommand AddStudentCommand => new(async cmd => await AddStudent(), canExecute => CanAddStudent());
        public RelayCommand EditStudentCommand => new(cmd => EditStudent(), canExecute => CanEditStudent());
        public RelayCommand DeleteStudentCommand => new(async cmd => await DeleteStudent(), canExecute => CanDeleteStudent());
        #endregion

        #region Properties
        private ObservableCollection<DtoLevel> levels = new();
        public ObservableCollection<DtoLevel> Levels { get => levels; set { levels = value; OnPropertyChanged(); } }
        private DtoLevel selectedLevel = new();
        public DtoLevel SelectedLevel
        {
            get { return selectedLevel; }
            set
            {
                selectedLevel = value;
                OnPropertyChanged();
                if (value != null)
                {
                    InputLevel = new()
                    {
                        Id = value.Id,
                        LevelName = value.LevelName,
                        TypeList = value.TypeList
                    };
                }
                else
                {
                    InputLevel = new();
                    Grades = new();
                }
            }
        }
        private DtoLevel inputLevel = new();
        public DtoLevel InputLevel { get { return inputLevel; } set { inputLevel = value; OnPropertyChanged(); } }
        private bool levelSaved = true;
        public bool LevelSaved { get => levelSaved; set { levelSaved = value; OnPropertyChanged(); } }
        private bool levelFocused = false;
        public bool LevelFocused { get { return levelFocused; } set { levelFocused = value; OnPropertyChanged(); } }

        private ObservableCollection<DtoGrade> grades = new();
        public ObservableCollection<DtoGrade> Grades { get { return grades; } set { grades = value; OnPropertyChanged(); } }
        private DtoGrade? selectedGrade = new();
        public DtoGrade? SelectedGrade
        {
            get { return selectedGrade; }
            set
            {
                selectedGrade = value;
                OnPropertyChanged();
                if (value != null && value.LevelId != null)
                {
                    InputGrade = new()
                    {
                        Id = value.Id,
                        GradeName = value.GradeName,
                        LevelId = value.LevelId
                    };
                    CboLevel = CboLevels.FirstOrDefault(x => x.Id == value.LevelId)!;
                }
                else
                {
                    InputGrade = new();
                    CboLevel = null!;
                    Classes = new();
                }
            }
        }
        private DtoGrade inputGrade = new();
        public DtoGrade InputGrade { get { return inputGrade; } set { inputGrade = value; OnPropertyChanged(); } }
        private bool gradeSaved = true;
        public bool GradeSaved { get { return gradeSaved; } set { gradeSaved = value; OnPropertyChanged(); } }
        private bool gradeFocused = false;
        public bool GradeFocused { get { return gradeFocused; } set { gradeFocused = value; OnPropertyChanged(); } }
        private ObservableCollection<DtoLevel> cboLevels = new();
        public ObservableCollection<DtoLevel> CboLevels { get { return cboLevels; } set { cboLevels = value; OnPropertyChanged(); } }
        private DtoLevel cboLevel = new();
        public DtoLevel CboLevel { get { return cboLevel; } set { cboLevel = value; OnPropertyChanged(); InputGrade.LevelId = value != null && value.Id != null ? value.Id : null; } }

        private ObservableCollection<DtoClass> classes = new();
        public ObservableCollection<DtoClass> Classes { get { return classes; } set { classes = value; OnPropertyChanged(); } }
        private DtoClass selectedClass = new();
        public DtoClass SelectedClass
        {
            get { return selectedClass; }
            set
            {
                selectedClass = value;
                OnPropertyChanged();
                if (value != null && value.Id != null)
                {
                    InputClass = new()
                    {
                        Id = value.Id,
                        ClassName = value.ClassName,
                        GradeId = value.GradeId,
                        TypeList = value.TypeList
                    };
                    CboGrade = CboGrades.FirstOrDefault(x => x.Id == value.GradeId)!;
                }
                else
                {
                    InputClass = new();
                    CboGrade = null!;
                }
            }
        }
        private DtoClass inputClass = new();
        public DtoClass InputClass { get { return inputClass; } set { inputClass = value; OnPropertyChanged(); } }
        private bool classSaved = true;
        public bool ClassSaved { get { return classSaved; } set { classSaved = value; OnPropertyChanged(); } }
        private bool classFocused = false;
        public bool ClassFocused { get { return classFocused; } set { classFocused = value; OnPropertyChanged(); } }
        private ObservableCollection<DtoGrade> cboGrades = new();
        public ObservableCollection<DtoGrade> CboGrades { get { return cboGrades; } set { cboGrades = value; OnPropertyChanged(); } }
        private DtoGrade cboGrade = new();
        public DtoGrade CboGrade { get { return cboGrade; } set { cboGrade = value; OnPropertyChanged(); InputClass.GradeId = value != null && value.Id != null ? value.Id : null; } }

        private ObservableCollection<DtoStudent> students = new();
        public ObservableCollection<DtoStudent> Students { get { return students; } set { students = value; OnPropertyChanged(); } }
        private DtoStudent selectedStudent = new();
        public DtoStudent SelectedStudent
        {
            get { return selectedStudent; }
            set
            {
                selectedStudent = value; OnPropertyChanged();
                if (value != null)
                {
                    InputStudent = new()
                    {
                        Id = value.Id,
                        Code = value.Code,
                        Phone = value.Phone,
                        Email = value.Email,
                        FullName = value.FullName,
                        Birthday = value.Birthday,
                        AddressCurrent = value.AddressCurrent,
                        ContactInfo = value.ContactInfo,
                        Gender = value.Gender,
                        Note = value.Note,
                        SequenceNumber = value.SequenceNumber,
                        TypeList = value.TypeList
                    };
                }
                else
                {
                    InputStudent = new();
                }
            }
        }
        private DtoStudent inputStudent = new();
        public DtoStudent InputStudent { get { return inputStudent; } set { inputStudent = value; OnPropertyChanged(); } }
        private bool studentSaved = true;
        public bool StudentSaved { get { return studentSaved; } set { studentSaved = value; OnPropertyChanged(); } }
        private bool studentFocused = false;
        public bool StudentFocused { get { return studentFocused; } set { studentFocused = value; OnPropertyChanged(); } }
        #endregion
    }
}