using Microsoft.Win32;

namespace EduManDesktopApp.Assets
{
    public class ConstantHelper
    {
        // Key Encrypt
        public static string KEY = "khangnguyen";

        // LOGININFO
        public static RegistryKey REGISTRY_KEY = Registry.CurrentUser;
        public static string REGISTRY_SUBKEY = "EduMan";
        public static string REGISTRY_LOGIN_INFO = "LoginInfo";

        // FILEPATH
        public static string FILEPATH_PARAM = "./Param.config";
        public static string FILEPATH_DATAEXCEL = "./Assets/Documents/Data.xlsx";
    }
}
