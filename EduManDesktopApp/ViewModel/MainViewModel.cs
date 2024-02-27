using EduManDesktopApp.TabType;
using EduManModel;
using EduManModel.Dtos;
using MVVM;
using SLHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EduManDesktopApp.ViewModel
{
    internal class MainViewModel : ViewModelBase
    {
        string imgPath = "/EduManDesktopApp;component/Assets/images/";
        public string AppLogo => imgPath + "applogo.png";
        public MainViewModel(DtoUserInfo userInfo)
        {
            MenuList = new()
            {
                new MenuControl()
                {
                    ItemIcon = imgPath + "student.png",
                    ItemText = "Nhập liệu",
                    OnClicked = ImportDataCommand
                },
                new MenuControl()
                {
                    ItemIcon = imgPath + "student.png",
                    ItemText = "Năm học",
                    OnClicked = StartWeekCommand
                },
                new MenuControl()
                {
                    ItemIcon = imgPath + "student.png",
                    ItemText = "Học sinh",
                    OnClicked = StudentCommand
                },                
                new MenuControl()
                {
                    ItemIcon = imgPath + "restrict.png",
                    ItemText = "Nền nếp",
                    OnClicked = DisciplineCommand
                },
                new MenuControl()
                {
                    ItemIcon = imgPath + "teacher.png",
                    ItemText = "Giáo viên",
                    OnClicked = TeacherCommand
                },

                 new MenuControl()
                {
                    ItemIcon = imgPath + "school.png",
                    ItemText = "Quản lý nền nếp",
                    OnClicked = DisciplineManagerCommand
                }
            };
            UserInfo = userInfo;
            WelcomeAdd();
        }

        private void WelcomeAdd()
        {
            WelcomeTab welcomeTab = new()
            {
                HeaderText = "Chào mừng",
                ShowClosed = Visibility.Hidden
            };
            TabList.Add(welcomeTab);
            SelectedTab = welcomeTab;
        }

        #region Properties
        public string Version
        {
            get 
            { 
                return $"Phiên bản: {Assembly.GetExecutingAssembly()!.GetName()!.Version!}";
            } 
        }

        DtoUserInfo _userInfo = new();
        public DtoUserInfo UserInfo 
        { 
            get => _userInfo;
            set
            {
                _userInfo = value;
                OnPropertyChanged();
                if (value == null || value.Id == null)
                {
                    return;
                }
                DataProcess<DtoGroupUser> dp = new();
                DtoResult<DtoGroupUser> rs = Task.Run(async () => await dp.FindAsync(new DtoGroupUser { Id = value.GroupUserId })).Result;
                if (rs != null && rs.Results != null && rs.Results.Count > 0)
                {
                    GroupUserName = rs.Results[0].GroupUserName!;
                }
            }
        }
        string _groupUserName = string.Empty;
        public string GroupUserName
        {
            get
            {
                return !string.IsNullOrEmpty(_groupUserName) ? $"[{_groupUserName}]" : string.Empty;
            }
            set
            {
                _groupUserName = value;
                OnPropertyChanged();
            }
        }
        ObservableCollection<MenuControl> menuList = new();
        public ObservableCollection<MenuControl> MenuList
        {
            get => menuList;
            set
            {
                menuList = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region RelayCommand
        public RelayCommand CloseWindowCommand => new(cmd => CloseWindow(), canExecute => true);
        public RelayCommand MinimizedCommand => new(cmd => MinimizeWindow(), canExecute => true);
        public RelayCommand MaximizedCommand => new(cmd => MaximizedWindow(), canExecute => true);

        public RelayCommand StudentCommand => new(cmd => StudentClick(), canExecute => CanClickStudent());
        public RelayCommand CloseStudentCommand => new(cmd=>CloseStudent(), canExecute => true);

        public RelayCommand TeacherCommand => new(cmd => TeacherClick(), canExecute => CanClickTeacher());
        public RelayCommand CloseTeacherCommand => new(cmd => CloseTeacher(), canExecute => true);

        public RelayCommand StartWeekCommand => new(cmd => StartWeekClick(), canExecute => true);
        public RelayCommand CloseStartWeekCommand => new(cmd => CloseStartWeek(), canExecute => true);

        public RelayCommand ImportDataCommand => new(cmd => ImportDataClick(), canExecute => true);
        public RelayCommand CloseImportDataCommand => new(cmd => CloseImportData(), canExecute => true);
        bool CanClickTeacher()
        {
            return true;
        }

        public RelayCommand ClassCommand => new(cmd => ClassClick(), canExecute => true);
        public RelayCommand DisciplineCommand => new(cmd => DisciplineClick(), canExecute => true);
        public RelayCommand CloseDisciplineCommand => new(cmd => CloseDiscipline(), canExecute => true);
        public RelayCommand DisciplineManagerCommand => new(cmd => DisciplineManagerClick(), canExecute => true);
        public RelayCommand CloseDisciplineManagerCommand => new(cmd => CloseDisciplineManager(), canExecute => true);

        #endregion


        #region Methods
        void DisciplineManagerClick()
        {
            WelcomeTab? welcome = (WelcomeTab?)TabList.FirstOrDefault(x => x.HeaderText == "Chào mừng");
            if (welcome != null)
            {
                TabList.Remove(welcome);
            }
            DisciplineManagerTab? tab = (DisciplineManagerTab?)TabList.FirstOrDefault(x => x.HeaderText == "Quản lí nền nếp");
            if (tab == null)
            {
                tab = new()
                {
                    HeaderText = "Quản lí nền nếp",
                    ShowClosed = Visibility.Visible,
                    HeaderClick = DisciplineManagerCommand,
                    CloseClick = CloseDisciplineManagerCommand
                };
                TabList.Add(tab);
            }
            SelectedTab = tab;
        }
        void CloseDisciplineManager()
        {
            DisciplineManagerTab? tab = (DisciplineManagerTab?)TabList.FirstOrDefault(x => x.HeaderText == "Quản lí nền nếp");
            if (tab == null)
                return;
            TabList.Remove(tab);
            if (TabList.Count == 0)
            {
                WelcomeAdd();
            }
        }
        void ImportDataClick()
        {
            WelcomeTab? welcome = (WelcomeTab?)TabList.FirstOrDefault(x => x.HeaderText == "Chào mừng");
            if (welcome != null)
            {
                TabList.Remove(welcome);
            }
            ImportDataTab? tab = (ImportDataTab?)TabList.FirstOrDefault(x => x.HeaderText == "Nhập liệu");
            if (tab == null)
            {
                tab = new()
                {
                    HeaderText = "Nhập liệu",
                    ShowClosed = Visibility.Visible,
                    HeaderClick = ImportDataCommand,
                    CloseClick = CloseImportDataCommand
                };
                TabList.Add(tab);
            }
            SelectedTab = tab;
        }
        void CloseImportData()
        {
            ImportDataTab? tab = (ImportDataTab?)TabList.FirstOrDefault(x => x.HeaderText == "Nhập liệu");
            if (tab == null)
                return;
            TabList.Remove(tab);
            if (TabList.Count == 0)
            {
                WelcomeAdd();
            }
        }
        void StartWeekClick()
        {
            WelcomeTab? welcome = (WelcomeTab?)TabList.FirstOrDefault(x => x.HeaderText == "Chào mừng");
            if (welcome != null)
            {
                TabList.Remove(welcome);
            }
            StartWeekTab? tab = (StartWeekTab?)TabList.FirstOrDefault(x => x.HeaderText == "Năm học");
            if (tab == null)
            {
                tab = new()
                {
                    HeaderText = "Năm học",
                    ShowClosed = Visibility.Visible,
                    HeaderClick = DisciplineCommand,
                    CloseClick = CloseDisciplineCommand
                };
                TabList.Add(tab);
            }
            SelectedTab = tab;
        }
        void CloseStartWeek()
        {
            StartWeekTab? tab = (StartWeekTab?)TabList.FirstOrDefault(x => x.HeaderText == "Năm học");
            if (tab == null)
                return;
            TabList.Remove(tab);
            if (TabList.Count == 0)
            {
                WelcomeAdd();
            }
        }
        void DisciplineClick()
        {
            WelcomeTab? welcome = (WelcomeTab?)TabList.FirstOrDefault(x => x.HeaderText == "Chào mừng");
            if (welcome != null)
            {
                TabList.Remove(welcome);
            }
            DisciplineTab? tab = (DisciplineTab?)TabList.FirstOrDefault(x => x.HeaderText == "Nền nếp");
            if (tab == null)
            {
                tab = new()
                {
                    HeaderText = "Nền nếp",
                    ShowClosed = Visibility.Visible,
                    HeaderClick = DisciplineCommand,
                    CloseClick = CloseDisciplineCommand
                };
                TabList.Add(tab);
            }
            SelectedTab = tab;
        }
        void CloseDiscipline()
        {
            DisciplineTab? tab = (DisciplineTab?)TabList.FirstOrDefault(x => x.HeaderText == "Nền nếp");
            if (tab == null)
                return;
            TabList.Remove(tab);
            if (TabList.Count == 0)
            {
                WelcomeAdd();
            }
        }
        void ClassClick()
        {
            NoticeBox.Show("Học sinh");
        }
        void TeacherClick()
        {
            WelcomeTab? welcome = (WelcomeTab?)TabList.FirstOrDefault(x => x.HeaderText == "Chào mừng");
            if (welcome != null)
            {
                TabList.Remove(welcome);
            }
            TeacherTab? tab = (TeacherTab?)TabList.FirstOrDefault(x => x.HeaderText == "Giáo viên");
            if (tab == null)
            {
                tab = new()
                {
                    HeaderText = "Giáo viên",
                    ShowClosed = Visibility.Visible,
                    HeaderClick = TeacherCommand,
                    CloseClick = CloseTeacherCommand
                };
                TabList.Add(tab);
            }
            SelectedTab = tab;
        }
        void CloseTeacher()
        {
            TeacherTab? tab = (TeacherTab?)TabList.FirstOrDefault(x => x.HeaderText == "Giáo viên");
            if (tab == null)
                return;
            TabList.Remove(tab);
            if (TabList.Count == 0)
            {
                WelcomeAdd();
            }
        }
        void StudentClick()
        {
            WelcomeTab? welcome = (WelcomeTab?)TabList.FirstOrDefault(x => x.HeaderText == "Chào mừng");
            if (welcome != null)
            {
                TabList.Remove(welcome);
            }
            StudentTab? tab = (StudentTab?)TabList.FirstOrDefault(x => x.HeaderText == "Học Sinh");
            if (tab == null)
            {
                tab = new()
                {
                    HeaderText = "Học Sinh",
                    ShowClosed = Visibility.Visible,
                    HeaderClick = StudentCommand,
                    CloseClick = CloseStudentCommand
                };
                TabList.Add(tab);
            }
            SelectedTab = tab;
        }
        void CloseStudent()
        {
            StudentTab? tab = (StudentTab?)TabList.FirstOrDefault(x => x.HeaderText == "Học Sinh");
            if (tab == null)
                return;
            TabList.Remove(tab);
            if (TabList.Count == 0)
            {
                WelcomeAdd();
            }
        }
        bool CanClickStudent()
        {
            return true;
        }
        void MaximizedWindow()
        {
            Window window = Application.Current.MainWindow;
            window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
        void MinimizeWindow()
        {
            Window window = Application.Current.MainWindow;
            window.WindowState = WindowState.Minimized;
        }
        void CloseWindow()
        {
            Window window = Application.Current.MainWindow;
            window.Close();
        }
        #endregion

        #region TabControls
        Tab selectedTab = new();
        public Tab SelectedTab
        {
            get => selectedTab;
            set { selectedTab = value; OnPropertyChanged(); }
        }
        ObservableCollection<Tab> tabList = new();
        public ObservableCollection<Tab> TabList
        {
            get => tabList;
            set { tabList = value; OnPropertyChanged(); }
        }
        #endregion
    }
}
