namespace Web.Services.Interfaces
{
    public interface IHttpInterceptor
    {
        // Interceptar request antes de enviarlo
        Task<HttpRequestMessage> InterceptRequestAsync(HttpRequestMessage request);

        // Interceptar response después de recibirlo
        Task<HttpResponseMessage> InterceptResponseAsync(HttpResponseMessage response);

        // Manejar errores de manera centralizada
        Task<bool> HandleErrorAsync(HttpResponseMessage response);

        // Eventos para notificar estados
        event EventHandler<HttpRequestEventArgs>? OnRequest;
        event EventHandler<HttpResponseEventArgs>? OnResponse;
        event EventHandler<HttpErrorEventArgs>? OnError;
    }

    public class HttpRequestEventArgs : EventArgs
    {
        public HttpRequestMessage Request { get; set; }
        public DateTime Timestamp { get; set; }
        public string? CorrelationId { get; set; }

        public HttpRequestEventArgs(HttpRequestMessage request)
        {
            Request = request;
            Timestamp = DateTime.UtcNow;
            CorrelationId = Guid.NewGuid().ToString();
        }
    }

    public class HttpResponseEventArgs : EventArgs
    {
        public HttpResponseMessage Response { get; set; }
        public TimeSpan Duration { get; set; }
        public string? CorrelationId { get; set; }

        public HttpResponseEventArgs(HttpResponseMessage response, TimeSpan duration, string? correlationId = null)
        {
            Response = response;
            Duration = duration;
            CorrelationId = correlationId;
        }
    }

    public class HttpErrorEventArgs : EventArgs
    {
        public HttpResponseMessage Response { get; set; }
        public Exception? Exception { get; set; }
        public bool Handled { get; set; }
        public int RetryCount { get; set; }

        public HttpErrorEventArgs(HttpResponseMessage response, Exception? exception = null)
        {
            Response = response;
            Exception = exception;
            Handled = false;
            RetryCount = 0;
        }
    }
}