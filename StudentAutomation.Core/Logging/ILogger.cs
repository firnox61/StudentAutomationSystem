namespace StudentAutomation.Core.Logging
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogError(string message);
    }
}
