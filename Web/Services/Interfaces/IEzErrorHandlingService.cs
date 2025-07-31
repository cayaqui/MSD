using Web.Services.Events;

namespace Web.Services.Interfaces
{
    public interface IEzErrorHandlingService
    {
        // Evento principal para notificar errores
        event EventHandler<EzErrorEventArgs>? OnError;

        // Métodos para reportar diferentes tipos de errores
        void ReportError(Exception exception, EzErrorSeverity severity = EzErrorSeverity.Error);
        void ReportError(string message, EzErrorSeverity severity = EzErrorSeverity.Error);
        void ReportError(string message, Exception exception, EzErrorSeverity severity = EzErrorSeverity.Error);
        void ReportValidationError(string message, Dictionary<string, List<string>> validationErrors);

        // Métodos específicos por severidad
        void ReportInfo(string message);
        void ReportWarning(string message);
        void ReportError(string message);
        void ReportCritical(string message, Exception? exception = null);

        // Manejo de errores HTTP
        Task HandleHttpErrorAsync(HttpResponseMessage response);
        Task<T?> HandleApiResponseAsync<T>(HttpResponseMessage response);

        // Logging y tracking
        void LogError(Exception exception, string? context = null);
        void LogError(string message, EzErrorSeverity severity, Dictionary<string, object>? properties = null);

        // Utilidades
        string GetUserFriendlyMessage(Exception exception);
        bool ShouldRetry(Exception exception);
        Task<bool> TryWithRetryAsync(Func<Task> operation, int maxRetries = 3);
        Task<T?> TryWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3);
    }
}