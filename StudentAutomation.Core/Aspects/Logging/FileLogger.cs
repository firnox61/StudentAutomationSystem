using StudentAutomation.Core.Logging;

namespace StudentAutomation.Core.Aspects.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _logFilePath = "logs/log.txt";

        public FileLogger()
        {
            var directory = Path.GetDirectoryName(_logFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void LogInfo(string message)
        {
            File.AppendAllText(_logFilePath, $"[INFO] {DateTime.Now}: {message}{Environment.NewLine}");
        }

        public void LogError(string message)
        {
            File.AppendAllText(_logFilePath, $"[ERROR] {DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}