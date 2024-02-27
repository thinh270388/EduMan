using EduManModel.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing;

namespace EduManDesktopApp.ViewModel
{
    public static class AdUse
    {
        public static DtoUserInfo LoginUser = new();
    }
    public class LoginInfo
    {
        public string? UserName { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public bool Remember { get; set; } = false;
    }
    public class ConpanyInfo
    {
        public string Name { get; set; } = string.Empty;
    }

    public class Param
    {
        public bool IsFirstRun { get; set; }
    }
}
