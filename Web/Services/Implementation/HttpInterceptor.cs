using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Web.Services.Events;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class HttpInterceptor : IHttpInterceptor
    {
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly NavigationManager _navigation;
        private readonly IEzErrorHandlingService _errorHandler;
        private readonly ILogger<HttpInterceptor> _logger;
        private readonly Dictionary<string, string> _requestCorrelations = new();

        public event EventHandler<HttpRequestEventArgs>? OnRequest;
        public event EventHandler<HttpResponseEventArgs>? OnResponse;
        public event EventHandler<HttpErrorEventArgs>? OnError;

        public HttpInterceptor(
            IAccessTokenProvider tokenProvider,
            NavigationManager navigation,
            IEzErrorHandlingService errorHandler,
            ILogger<HttpInterceptor> logger)
        {
            _tokenProvider = tokenProvider;
            _navigation = navigation;
            _errorHandler = errorHandler;
            _logger = logger;
        }

        public async Task<HttpRequestMessage> InterceptRequestAsync(HttpRequestMessage request)
        {
            var requestEvent = new HttpRequestEventArgs(request);

            // Guardar correlación para el tracking
            _requestCorrelations[request.GetHashCode().ToString()] = requestEvent.CorrelationId!;

            // Agregar headers comunes
            request.Headers.Add("X-Correlation-Id", requestEvent.CorrelationId);
            request.Headers.Add("X-Client-Version", GetClientVersion());

            // Intentar agregar token de autorización
            await AddAuthorizationHeaderAsync(request);

            // Logging
            _logger.LogDebug("HTTP Request: {Method} {Uri}", request.Method, request.RequestUri);

            // Notificar evento
            OnRequest?.Invoke(this, requestEvent);

            return request;
        }

        public async Task<HttpResponseMessage> InterceptResponseAsync(HttpResponseMessage response)
        {
            var requestKey = response.RequestMessage?.GetHashCode().ToString() ?? "";
            var correlationId = _requestCorrelations.GetValueOrDefault(requestKey);
            var duration = TimeSpan.FromMilliseconds(100); // En una implementación real, mediríamos el tiempo real

            var responseEvent = new HttpResponseEventArgs(response, duration, correlationId);

            // Logging
            _logger.LogDebug("HTTP Response: {StatusCode} {ReasonPhrase} in {Duration}ms",
                response.StatusCode, response.ReasonPhrase, duration.TotalMilliseconds);

            // Manejar errores si es necesario
            if (!response.IsSuccessStatusCode)
            {
                var handled = await HandleErrorAsync(response);
                if (handled)
                {
                    // Si el error fue manejado (ej: token renovado), podríamos reintentar
                    // En este caso, simplemente continuamos
                }
            }

            // Limpiar correlación
            if (!string.IsNullOrEmpty(requestKey))
            {
                _requestCorrelations.Remove(requestKey);
            }

            // Notificar evento
            OnResponse?.Invoke(this, responseEvent);

            return response;
        }

        public async Task<bool> HandleErrorAsync(HttpResponseMessage response)
        {
            var errorEvent = new HttpErrorEventArgs(response);

            try
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        return await HandleUnauthorizedAsync(response);

                    case HttpStatusCode.Forbidden:
                        HandleForbidden();
                        errorEvent.Handled = true;
                        break;

                    case HttpStatusCode.TooManyRequests:
                        await HandleRateLimitAsync(response);
                        errorEvent.Handled = true;
                        break;

                    case HttpStatusCode.ServiceUnavailable:
                    case HttpStatusCode.GatewayTimeout:
                        errorEvent.Handled = false; // Permitir reintentos
                        break;

                    default:
                        // Dejar que el ErrorHandlingService maneje otros errores
                        await _errorHandler.HandleHttpErrorAsync(response);
                        errorEvent.Handled = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                errorEvent.Exception = ex;
                _logger.LogError(ex, "Error handling HTTP response");
            }

            // Notificar evento
            OnError?.Invoke(this, errorEvent);

            return errorEvent.Handled;
        }

        private async Task AddAuthorizationHeaderAsync(HttpRequestMessage request)
        {
            try
            {
                var tokenResult = await _tokenProvider.RequestAccessToken();

                if (tokenResult.TryGetToken(out var token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to add authorization header");
                // Continuar sin token - el servidor responderá con 401 si es necesario
            }
        }

        private async Task<bool> HandleUnauthorizedAsync(HttpResponseMessage response)
        {
            _logger.LogWarning("Received 401 Unauthorized response");

            try
            {
                // Intentar renovar el token
                var tokenResult = await _tokenProvider.RequestAccessToken();

                if (!tokenResult.TryGetToken(out _))
                {
                    // No se pudo renovar el token, redirigir al login
                    _errorHandler.ReportWarning("Tu sesión ha expirado. Por favor, inicia sesión nuevamente.");
                    _navigation.NavigateTo("/authentication/login");
                    return true;
                }

                // Token renovado exitosamente
                return false; // Permitir reintento
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling unauthorized response");
                _navigation.NavigateTo("/authentication/login");
                return true;
            }
        }

        private void HandleForbidden()
        {
            _errorHandler.ReportError(
                "No tienes permisos para acceder a este recurso. Contacta al administrador si crees que esto es un error.",
                EzErrorSeverity.Warning);
        }

        private async Task HandleRateLimitAsync(HttpResponseMessage response)
        {
            var retryAfter = response.Headers.RetryAfter;
            var waitTime = retryAfter?.Delta ?? TimeSpan.FromSeconds(60);

            _errorHandler.ReportWarning(
                $"Has realizado demasiadas solicitudes. Por favor, espera {waitTime.TotalSeconds} segundos antes de intentar nuevamente.");

            await Task.Delay(waitTime);
        }

        private string GetClientVersion()
        {
            // En producción, esto vendría de la configuración o assembly
            return "1.0.0";
        }
    }
}