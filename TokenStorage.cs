using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using WildberriesDeductions;

namespace WildberriesDeductions
{
    public class TokenStorage
    {
        private string _filePath;
        private string _directoryPath;

        public TokenStorage()
        {
            string baseDirectory = AppContext.BaseDirectory;

            _directoryPath = Path.Combine(baseDirectory, "tokens");

            _filePath = Path.Combine(_directoryPath, "tokens.txt");
        }
        public void Save(string token)
        {
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
            File.WriteAllText(_filePath, token);
        }

        public string Load()
        {
            if (!File.Exists(_filePath))
            {
                return string.Empty;
            }
            return File.ReadAllText(_filePath);
        }
    }
}
