using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WildberriesDeductions.Wildberries.Models;

namespace WildberriesDeductions
{
    public class ConfigLoader
    {
        private readonly string _configDirectory;
        private readonly string _commonHeadersFileName;
        private RequestConfig? _commonHeaders; // Теперь это сразу RequestConfig

        public ConfigLoader(string configDirectory, string commonHeadersFileName = "common-headers.json")
        {
            _configDirectory = configDirectory;
            _commonHeadersFileName = commonHeadersFileName;

            if (!Directory.Exists(_configDirectory))
                Directory.CreateDirectory(_configDirectory);

            try
            {
                _commonHeaders = Load(_commonHeadersFileName);
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки файла с общими заголовками: {ex.Message}");
            }
            
        }

        // Загружает ОДИН файл и превращает его в RequestConfig
        private RequestConfig Load(string fileName)
        {
            string fullPath = Path.Combine(_configDirectory, fileName);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Файл {fullPath} не найден");

            var json = File.ReadAllText(fullPath);
            var result = JsonSerializer.Deserialize<RequestConfig>(json);

            return result ?? throw new Exception($"Ошибка десериализации {fullPath}");
        }

        // Метод для загрузки конкретного конфига (например, "login.json")
        public RequestConfig LoadRequest(string fileName)
        { 
            var config = Load(fileName);

            if (_commonHeaders != null)
            config.ApplyCommonHeadersReq(_commonHeaders);

            return config;
        }

        public Dictionary<string, RequestConfig> LoadAll()
        {
            var result = new Dictionary<string, RequestConfig>();
            var files = Directory.GetFiles(_configDirectory, "*.json");

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var fileFullPath = Path.Combine(_configDirectory, fileName);

                // Пропускаем общие заголовки
                if (fileName.Equals(_commonHeadersFileName))
                    continue;

                try
                {
                    var key = Path.GetFileNameWithoutExtension(fileName);
                    result[key] = LoadRequest(fileName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Пропущен файл {fileFullPath}: {ex.Message}");
                }
            }
            return result;
        }
    }
}
