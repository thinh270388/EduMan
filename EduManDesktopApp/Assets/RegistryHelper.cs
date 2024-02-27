using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EduManDesktopApp.Assets
{
    public class RegistryHelper
    {
        public string? Read(string subKey, string keyName, bool showError = false)
        {            
            RegistryKey? registryKey = ConstantHelper.REGISTRY_KEY.OpenSubKey(@"SOFTWARE\" + subKey);
            if (registryKey == null)
            {
                return null;
            }
            else
            {
                try
                {
                    return (string)registryKey.GetValue(keyName)!;
                }
                catch (Exception e)
                {
                    if (showError) ShowErrorMessage(e, "Reading registry " + keyName);
                    return null;
                }
            }
        }
        public bool Write(string subKey, string keyName, object value, bool showError = false)
        {
            try
            {               
                RegistryKey registryKey = ConstantHelper.REGISTRY_KEY.CreateSubKey(@"SOFTWARE\" + subKey);
                registryKey.SetValue(keyName, value);

                return true;
            }
            catch (Exception e)
            {
                if (showError) ShowErrorMessage(e, "Writing registry " + keyName);
                return false;
            }
        }
        private void ShowErrorMessage(Exception e, string title)
        {
            MessageBox.Show(e.Message, title);
        }
    }
}
