namespace Web.Services.Interfaces;

public interface ILoggingService
{
    void LogDebug(string message, params object[] args);
    void LogInfo(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
    void LogGroup(string groupName);
    void LogGroupEnd();
    void LogTable(object data);
}