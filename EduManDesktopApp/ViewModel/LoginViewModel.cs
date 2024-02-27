using EduManDesktopApp.Assets;
using EduManModel;
using EduManModel.Dtos;
using Microsoft.Win32;
using MVVM;
using Newtonsoft.Json;
using SLHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TextProcessing;

namespace EduManDesktopApp.ViewModel
{
    internal class LoginViewModel : ViewModelBase
    {
        public LoginViewModel()
        {
            try
            {
                NoticeBoxResult dialogrs;
                do
                {
                    dialogrs = NoticeBoxResult.None;
                    Task.Run(async () => await LoadLogin()).Wait();
                    if (_loginList == null)
                    {
                        dialogrs = NoticeBox.Show("Mất kết nối máy chủ.\r\nKết nối lại?", "Mất kết nối", NoticeBoxButton.YesNo, NoticeBoxImage.Error);
                    }
                    if (dialogrs == NoticeBoxResult.No) Application.Current.Shutdown();
                }
                while (dialogrs == NoticeBoxResult.Yes);

                // GetDatabase();

                RegistryHelper registryHelper = new();
                string s = registryHelper.Read(ConstantHelper.REGISTRY_SUBKEY, ConstantHelper.REGISTRY_LOGIN_INFO, false)!;
                if (!string.IsNullOrEmpty(s))
                {
                    LoginInfo? loginInfo = JsonConvert.DeserializeObject<LoginInfo>(s);
                    if (loginInfo != null)
                    {
                        Window? window = Application.Current.Windows.Cast<Window>().FirstOrDefault(x => x.Name == "loginWindow");
                        if (window != null)
                        {
                            Encryption encryption = new();
                            UserName = encryption.Decrypt(loginInfo.UserName!, ConstantHelper.KEY);
                            PasswordBox? box = window.FindName("txtPassword") as PasswordBox;
                            box!.Password = encryption.Decrypt(loginInfo.Password!, ConstantHelper.KEY);
                            Remember = loginInfo.Remember;
                        }
                    }
                }                
            }
            catch (Exception)
            {
                NoticeBox.Show("Mất kết nối máy chủ.\r\nKết nối lại?", "Mất kết nối", NoticeBoxButton.YesNo, NoticeBoxImage.Error);
            }         
        }

        #region Properties
        ObservableCollection<DtoUserInfo> _loginList = new();
        DtoUserInfo _user = new();
        public DtoUserInfo User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }
        List<string> dataList = new List<string>();
        public List<string> DataList { get => dataList; set => dataList = value; }
        string? userName = null;
        public string? UserName
        {
            get => userName;
            set
            {
                userName = value;
                OnPropertyChanged();
            }
        }
        private bool remember;
        public bool Remember
        {
            get { return remember; }
            set { remember = value; OnPropertyChanged(); }
        }
        #endregion

        #region Methods
        private async Task LoadLogin()
        {
            DataProcess<DtoUserInfo> dp = new();
            DtoResult<DtoUserInfo> rs = await dp.GetAllAsync(new DtoUserInfo());
            _loginList = rs != null ? new(rs!.Results!) : null!;
        }
        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(UserName);
        }
        void GetDatabase()
        {
            //DataList.Clear();
            //using SqlConnection conn = new($"Data Source=.\\SQLEXPRESS;Persist Security Info=True;User ID=sa;Password=881421");
            //try
            //{
            //    conn.Open();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Không thể kết nối máy chủ SQL.\r\n{ex.Message}");
            //    return;
            //}
            //using SqlCommand cmd = new("SELECT name from sys.databases", conn);
            //using SqlDataReader reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    DataList.Add(reader[0].ToString()!);
            //}
            //conn.Close();
        }
        void LoginApp()
        {
            User = _loginList.FirstOrDefault(x => x.UserName!.ToLower() == UserName!.ToLower())!;
            if (User == null)
            {
                NoticeBox.Show("Không tìm thấy user");
                return;
            }
            Encryption encrypt = new();
            Window? window = Application.Current.Windows.Cast<Window>().FirstOrDefault(x => x.Name == "loginWindow");
            PasswordBox? box = window!.FindName("txtPassword") as PasswordBox;
            if (window != null)
            {
                if (box?.Password != encrypt.Decrypt(User!.UserPassword!, ConstantHelper.KEY))
                {
                    NoticeBox.Show("Sai mật khẩu");
                    return;
                }
            }

            LoginInfo loginInfo = new();
            if (Remember)
            {
                loginInfo = new()
                {
                    Remember = Remember,
                    UserName = encrypt.Encrypt(UserName!, ConstantHelper.KEY),
                    Password = encrypt.Encrypt(box!.Password, ConstantHelper.KEY)
                };
            }

            RegistryHelper registryHelper = new();
            registryHelper.Write(ConstantHelper.REGISTRY_SUBKEY, ConstantHelper.REGISTRY_LOGIN_INFO, JsonConvert.SerializeObject(loginInfo), false);

            AdUse.LoginUser = User;
            window?.Close();
        }
        private void ExitApp()
        {
            Application.Current.Shutdown();
            //Window window = Application.Current.MainWindow;
            //window.Close();
        }
        #endregion

        #region RelayCommands
        public RelayCommand LoginCommand => new(cmd => LoginApp(), canExecute => CanLogin());
        public RelayCommand ExitCommand => new(cmd => ExitApp(), canExecute => true);
        #endregion
    }
}
