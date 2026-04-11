namespace WildberriesDeductions.Wildberries
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

        public string LoadAccessToken()
        {
            if (!File.Exists(_filePath))
            {
                Save(string.Empty); // Создаем файл, если его нет, чтобы избежать ошибок при чтении в будущем
                return string.Empty;
            }
            return File.ReadAllText(_filePath);
        }
    }
}
