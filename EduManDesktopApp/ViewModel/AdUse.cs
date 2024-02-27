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
    public class ConfigModify
    {
        public void WriteConfig(ConpanyInfo config)
        {
            string filepath = "./config.cfg";
            string content = "";
            Encryption encrypt = new();
            using FileStream stream = File.Create(filepath);
            content = JsonConvert.SerializeObject(config);
            byte[] buffer = new UTF8Encoding(true).GetBytes(content);
            stream.Write(buffer, 0, buffer.Length);
        }
        public ConpanyInfo? GetConfig(string filepath)
        {
            if (!File.Exists(filepath)) return null;
            string content = File.ReadAllText(filepath);
            Encryption encrypt = new();
            //content = encrypt.Decrypt(content,"1234")
            ConpanyInfo? config = JsonConvert.DeserializeObject<ConpanyInfo>(content);
            return config;
        }
    }
}
