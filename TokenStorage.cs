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
        public void Save(string token, string fileName = "tokens.txt")
        {
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
            string targetPath = Path.Combine(_directoryPath, fileName);
            File.WriteAllText(targetPath, token);
        }

        public string LoadAccessToken(string fileName = "tokens.txt")
        {
            string targetPath = Path.Combine (_directoryPath, fileName);
            if (!File.Exists(targetPath))
            {
                Save(string.Empty); // Создаем файл, если его нет, чтобы избежать ошибок при чтении в будущем
                return string.Empty;
            }
            return File.ReadAllText(_filePath);
        }
    }
}
