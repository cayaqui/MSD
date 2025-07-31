using System.Net;
using System.Text.Json;
using Web.Services.Events;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class EzErrorHandlingService : IEzErrorHandlingService
    {
        private readonly ILogger<EzErrorHandlingService> _logger;
        private readonly IConfiguration _configuration;

        public event EventHandler<EzErrorEventArgs>? OnError;

        // Mensajes de error amigables para el usuario
        private readonly Dictionary<Type, string> _friendlyMessages = new()
        {
            { typeof(HttpRequestException), "Error de conexión. Por favor, verifica tu conexión a internet." },
            { typeof(TaskCanceledException), "La operación tardó demasiado tiempo. Por favor, intenta nuevamente." },
            { typeof(UnauthorizedAccessException), "No tienes permisos para realizar esta acción." },
            { typeof(InvalidOperationException), "Esta operación no está permitida en este momento." },
            { typeof(ArgumentException), "Los datos proporcionados no son válidos." },
            { typeof(NotImplementedException), "Esta funcionalidad aún no está disponible." },
            { typeof(TimeoutException), "La operación excedió el tiempo límite." }
        };

        // Excepciones que se pueden reintentar
        private readonly HashSet<Type> _retryableExceptions = new()
        {
            typeof(HttpRequestException),
            typeof(TaskCanceledException),
            typeof(TimeoutException),
            typeof(IOException)
        };

        public EzErrorHandlingService(ILogger<EzErrorHandlingService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // Métodos principales para reportar errores
        public void ReportError(Exception exception, EzErrorSeverity severity = EzErrorSeverity.Error)
        {
            LogError(exception);

            var args = new EzErrorEventArgs(exception, severity)
            {
                Message = GetUserFriendlyMessage(exception)
            };

            OnError?.Invoke(this, args);
        }

        public void ReportError(string message, EzErrorSeverity severity = EzErrorSeverity.Error)
        {
            LogError(message, severity);

            var args = new EzErrorEventArgs(message, severity);
            OnError?.Invoke(this, args);
        }

        public void ReportError(string message, Exception exception, EzErrorSeverity severity = EzErrorSeverity.Error)
        {
            LogError(exception, message);

            var args = new EzErrorEventArgs(message, exception, severity);
            OnError?.Invoke(this, args);
        }

        public void ReportValidationError(string message, Dictionary<string, List<string>> validationErrors)
        {
            LogError(message, EzErrorSeverity.Warning, new Dictionary<string, object> { { "ValidationErrors", validationErrors } });

            var args = new EzErrorEventArgs(message, EzErrorSeverity.Warning)
            {
                ValidationErrors = validationErrors
            };

            OnError?.Invoke(this, args);
        }

        // Métodos específicos por severidad
        public void ReportInfo(string message)
        {
            ReportError(message, EzErrorSeverity.Info);
        }

        public void ReportWarning(string message)
        {
            ReportError(message, EzErrorSeverity.Warning);
        }

        public void ReportCritical(string message, Exception? exception = null)
        {
            if (exception != null)
            {
                ReportError(message, exception, EzErrorSeverity.Critical);
            }
            else
            {
                ReportError(message, EzErrorSeverity.Critical);
            }
        }

        // Manejo de errores HTTP
        public async Task HandleHttpErrorAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var message = GetHttpErrorMessage(response.StatusCode);

            var apiException = new ApiException(message, (int)response.StatusCode)
            {
                ReasonPhrase = response.ReasonPhrase
            };

            // Intentar parsear errores de validación
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var validationProblem = JsonSerializer.Deserialize<ValidationProblemDetails>(content, options);

                    if (validationProblem?.Errors != null)
                    {
                        apiException.ValidationErrors = validationProblem.Errors;
                        ReportValidationError("Errores de validación", validationProblem.Errors);
                        return;
                    }
                }
                catch
                {
                    // Si no es un problema de validación, continuar con el manejo normal
                }
            }

            ReportError(apiException);
        }

        public async Task<T?> HandleApiResponseAsync<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                await HandleHttpErrorAsync(response);
                return default;
            }

            try
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<T>(content, options);
            }
            catch (Exception ex)
            {
                ReportError("Error al procesar la respuesta del servidor", ex);
                return default;
            }
        }

        // Logging
        public void LogError(Exception exception, string? context = null)
        {
            var logMessage = context ?? "Unhandled exception";

            if (exception is ApiException apiEx)
            {
                _logger.LogError(exception, "{Context} - Status: {StatusCode}, Reason: {Reason}",
                    logMessage, apiEx.StatusCode, apiEx.ReasonPhrase);
            }
            else if (exception is BusinessException businessEx)
            {
                _logger.LogWarning(exception, "{Context} - Code: {Code}, Severity: {Severity}",
                    logMessage, businessEx.Code, businessEx.Severity);
            }
            else
            {
                _logger.LogError(exception, "{Context}", logMessage);
            }
        }

        public void LogError(string message, EzErrorSeverity severity, Dictionary<string, object>? properties = null)
        {
            var logLevel = severity switch
            {
                EzErrorSeverity.Info => LogLevel.Information,
                EzErrorSeverity.Warning => LogLevel.Warning,
                EzErrorSeverity.Error => LogLevel.Error,
                EzErrorSeverity.Critical => LogLevel.Critical,
                _ => LogLevel.Error
            };

            using (_logger.BeginScope(properties ?? new Dictionary<string, object>()))
            {
                _logger.Log(logLevel, message);
            }
        }

        // Utilidades
        public string GetUserFriendlyMessage(Exception exception)
        {
            // Verificar si es una excepción de negocio con mensaje amigable
            if (exception is BusinessException businessEx)
            {
                return businessEx.Message;
            }

            // Verificar si es una excepción de API
            if (exception is ApiException apiEx)
            {
                return GetHttpErrorMessage((HttpStatusCode)apiEx.StatusCode);
            }

            // Buscar mensaje amigable para el tipo de excepción
            var exceptionType = exception.GetType();
            if (_friendlyMessages.TryGetValue(exceptionType, out var friendlyMessage))
            {
                return friendlyMessage;
            }

            // Mensaje genérico para producción
            var isDevelopment = _configuration.GetValue<bool>("IsDevelopment", false);
            return isDevelopment ? exception.Message : "Ha ocurrido un error inesperado. Por favor, intenta nuevamente.";
        }

        public bool ShouldRetry(Exception exception)
        {
            // Verificar si es una excepción que se puede reintentar
            if (_retryableExceptions.Contains(exception.GetType()))
            {
                return true;
            }

            // Verificar códigos de estado HTTP que se pueden reintentar
            if (exception is ApiException apiEx)
            {
                var retryableStatusCodes = new[] { 408, 429, 500, 502, 503, 504 };
                return retryableStatusCodes.Contains(apiEx.StatusCode);
            }

            return false;
        }

        public async Task<bool> TryWithRetryAsync(Func<Task> operation, int maxRetries = 3)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await operation();
                    return true;
                }
                catch (Exception ex) when (i < maxRetries - 1 && ShouldRetry(ex))
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, i)); // Exponential backoff
                    _logger.LogWarning("Operación falló, reintentando en {Delay}s. Intento {Attempt}/{MaxRetries}",
                        delay.TotalSeconds, i + 1, maxRetries);

                    await Task.Delay(delay);
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                    return false;
                }
            }

            return false;
        }

        public async Task<T?> TryWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex) when (i < maxRetries - 1 && ShouldRetry(ex))
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, i));
                    _logger.LogWarning("Operación falló, reintentando en {Delay}s. Intento {Attempt}/{MaxRetries}",
                        delay.TotalSeconds, i + 1, maxRetries);

                    await Task.Delay(delay);
                }
                catch (Exception ex)
                {
                    ReportError(ex);
                    return default;
                }
            }

            return default;
        }

        // Métodos auxiliares privados
        private string GetHttpErrorMessage(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => "Los datos enviados no son válidos",
                HttpStatusCode.Unauthorized => "No estás autorizado para realizar esta acción",
                HttpStatusCode.Forbidden => "No tienes permisos para acceder a este recurso",
                HttpStatusCode.NotFound => "El recurso solicitado no fue encontrado",
                HttpStatusCode.Conflict => "Existe un conflicto con el estado actual del recurso",
                HttpStatusCode.UnprocessableEntity => "No se puede procesar la solicitud debido a errores de validación",
                HttpStatusCode.TooManyRequests => "Has realizado demasiadas solicitudes. Por favor, espera un momento",
                HttpStatusCode.InternalServerError => "Error interno del servidor. Por favor, intenta más tarde",
                HttpStatusCode.BadGateway => "Error de comunicación con el servidor",
                HttpStatusCode.ServiceUnavailable => "El servicio no está disponible temporalmente",
                HttpStatusCode.GatewayTimeout => "El servidor tardó demasiado en responder",
                _ => $"Error del servidor (código {(int)statusCode})"
            };
        }

        public void ReportError(string message)
        {
            ReportError(message, EzErrorSeverity.Error);
        }
    }
  
}   