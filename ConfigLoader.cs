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
        }

        // Загружает ОДИН файл и превращает его в RequestConfig
        private RequestConfig Load(string fileName)
        {
            string fullPath = Path.Combine(_configDirectory, fileName);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Файл {fileName} не найден");

            var json = File.ReadAllText(fullPath);
            var result = JsonSerializer.Deserialize<RequestConfig>(json);

            return result ?? throw new Exception($"Ошибка десериализации {fileName}");
        }

        // Загружает общие заголовки один раз
        private RequestConfig GetCommonHeaders()
        {
            return _commonHeaders ??= Load(_commonHeadersFileName);
        }

        // Метод для загрузки конкретного конфига (например, "login.json")
        public RequestConfig LoadRequest(string fileName)
        {
            var common = GetCommonHeaders();
            var config = Load(fileName);

            config.ApplyCommonHeaders(common.Headers);
            return config;
        }

        // Метод, который ты хотел: загрузить ВСЁ из папки в словарь
        public Dictionary<string, RequestConfig> LoadAll()
        {
            var result = new Dictionary<string, RequestConfig>();
            var files = Directory.GetFiles(_configDirectory, "*.json");

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);

                // Пропускаем общие заголовки
                if (fileName.Equals(_commonHeadersFileName, StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    var key = Path.GetFileNameWithoutExtension(fileName);
                    result[key] = LoadRequest(fileName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Пропущен файл {fileName}: {ex.Message}");
                }
            }
            return result;
        }
    }
}
