using EduManDesktopApp.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextProcessing;

namespace EduManDesktopApp.Assets
{
    public class ConfigHelper
    {
        public void Set(Param config, string filePath)
        {
            Encryption encrypt = new();
            using FileStream stream = File.Create(filePath);
            string content = JsonConvert.SerializeObject(config);
            byte[] buffer = new UTF8Encoding(true).GetBytes(content);
            stream.Write(buffer, 0, buffer.Length);
        }
        public void Get(string filePath, out Param config)
        {
            if (!File.Exists(filePath)) config = null!;

            string content = File.ReadAllText(filePath);
            Encryption encrypt = new();
            content = encrypt.Decrypt(content, ConstantHelper.KEY);
            config = JsonConvert.DeserializeObject<Param>(content)!;
        }
    }
}
