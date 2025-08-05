using Microsoft.JSInterop;
using Web.Services.Interfaces;

namespace Web.Services.Implementation;

public class LoggingService : ILoggingService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly string _prefix = "[EzPro MSD]";

    public LoggingService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public void LogDebug(string message, params object[] args)
    {
        var formattedMessage = FormatMessage("DEBUG", message, args);
        _ = LogToConsole("log", formattedMessage);
    }

    public void LogInfo(string message, params object[] args)
    {
        var formattedMessage = FormatMessage("INFO", message, args);
        _ = LogToConsole("info", formattedMessage);
    }

    public void LogWarning(string message, params object[] args)
    {
        var formattedMessage = FormatMessage("WARN", message, args);
        _ = LogToConsole("warn", formattedMessage);
    }

    public void LogError(string message, params object[] args)
    {
        var formattedMessage = FormatMessage("ERROR", message, args);
        _ = LogToConsole("error", formattedMessage);
    }

    public void LogError(Exception exception, string message, params object[] args)
    {
        var formattedMessage = FormatMessage("ERROR", message, args);
        _ = LogToConsole("error", $"{formattedMessage}\nException: {exception.GetType().Name}: {exception.Message}\nStackTrace: {exception.StackTrace}");
    }

    public void LogGroup(string groupName)
    {
        _ = _jsRuntime.InvokeVoidAsync("console.group", $"{_prefix} {groupName}");
    }

    public void LogGroupEnd()
    {
        _ = _jsRuntime.InvokeVoidAsync("console.groupEnd");
    }

    public void LogTable(object data)
    {
        _ = _jsRuntime.InvokeVoidAsync("console.table", data);
    }

    private string FormatMessage(string level, string message, object[] args)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var formattedMessage = args.Length > 0 ? string.Format(message, args) : message;
        return $"{_prefix} [{timestamp}] [{level}] {formattedMessage}";
    }

    private async Task LogToConsole(string logLevel, string message)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync($"console.{logLevel}", message);
        }
        catch
        {
            // Ignore logging errors to prevent infinite loops
        }
    }
}