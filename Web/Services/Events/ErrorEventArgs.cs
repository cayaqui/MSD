namespace Web.Services.Events
{
    public class EzErrorEventArgs : EventArgs
    {
        public string Message { get; set; } = string.Empty;
        public EzErrorSeverity Severity { get; set; }
        public Exception? Exception { get; set; }
        public Dictionary<string, List<string>>? ValidationErrors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Context { get; set; }
        public Dictionary<string, object>? AdditionalData { get; set; }

        public EzErrorEventArgs(string message, EzErrorSeverity severity = EzErrorSeverity.Error)
        {
            Message = message;
            Severity = severity;
        }

        public EzErrorEventArgs(Exception exception, EzErrorSeverity severity = EzErrorSeverity.Error)
        {
            Exception = exception;
            Message = exception.Message;
            Severity = severity;
        }

        public EzErrorEventArgs(string message, Exception exception, EzErrorSeverity severity = EzErrorSeverity.Error)
        {
            Message = message;
            Exception = exception;
            Severity = severity;
        }
    }

    public class ApiException : Exception
    {
        public int StatusCode { get; set; }
        public string? ReasonPhrase { get; set; }
        public Dictionary<string, List<string>>? ValidationErrors { get; set; }
        public string? TraceId { get; set; }

        public ApiException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }

        public ApiException(string message, int statusCode, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }

    public class BusinessException : Exception
    {
        public string Code { get; set; }
        public EzErrorSeverity Severity { get; set; }
        public Dictionary<string, object>? Data { get; set; }

        public BusinessException(string code, string message, EzErrorSeverity severity = EzErrorSeverity.Warning)
            : base(message)
        {
            Code = code;
            Severity = severity;
        }
    }

    public class ValidationException : Exception
    {
        public Dictionary<string, List<string>> Errors { get; set; }

        public ValidationException(Dictionary<string, List<string>> errors)
            : base("Se encontraron errores de validación")
        {
            Errors = errors;
        }

        public ValidationException(string field, string error)
            : base("Se encontraron errores de validación")
        {
            Errors = new Dictionary<string, List<string>>
            {
                { field, new List<string> { error } }
            };
        }
    }
}
