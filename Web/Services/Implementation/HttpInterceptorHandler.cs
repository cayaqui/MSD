using System.Diagnostics;
using System.Net;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class HttpInterceptorHandler : DelegatingHandler
    {
        private readonly IHttpInterceptor _interceptor;
        private readonly ILogger<HttpInterceptorHandler> _logger;

        public HttpInterceptorHandler(IHttpInterceptor interceptor, ILogger<HttpInterceptorHandler> logger)
        {
            _interceptor = interceptor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Interceptar el request antes de enviarlo
                request = await _interceptor.InterceptRequestAsync(request);

                // Enviar el request
                var response = await base.SendAsync(request, cancellationToken);

                stopwatch.Stop();

                // Interceptar el response después de recibirlo
                response = await _interceptor.InterceptResponseAsync(response);

                return response;
            }
            catch (TaskCanceledException ex)
            {
                stopwatch.Stop();
                _logger.LogWarning(ex, "Request cancelled after {ElapsedMs}ms: {Method} {Uri}",
                    stopwatch.ElapsedMilliseconds, request.Method, request.RequestUri);

                // Crear una respuesta de timeout para que el interceptor pueda manejarla
                var timeoutResponse = new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                {
                    RequestMessage = request,
                    ReasonPhrase = "Request Timeout"
                };

                await _interceptor.HandleErrorAsync(timeoutResponse);
                throw;
            }
            catch (HttpRequestException ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "HTTP request failed after {ElapsedMs}ms: {Method} {Uri}",
                    stopwatch.ElapsedMilliseconds, request.Method, request.RequestUri);

                // Crear una respuesta de error de red
                var errorResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    RequestMessage = request,
                    ReasonPhrase = "Network Error"
                };

                await _interceptor.HandleErrorAsync(errorResponse);
                throw;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Unexpected error after {ElapsedMs}ms: {Method} {Uri}",
                    stopwatch.ElapsedMilliseconds, request.Method, request.RequestUri);
                throw;
            }
        }
    }
}