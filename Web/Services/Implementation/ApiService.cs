using System.Net;
using System.Net.Http.Headers;
using Web.Models.Responses;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(IHttpClientFactory httpClientFactory, ILogger<ApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("EzProAPI");
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                return await ProcessResponse<T>(response);
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                return new ApiResponse<T> { Success = false, Message = "Token no disponible" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar GET a {Endpoint}", endpoint);
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "Error al conectar con el servidor"
                };
            }
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, content);
                return await ProcessResponse<T>(response);
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                return new ApiResponse<T> { Success = false, Message = "Token no disponible" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar POST a {Endpoint}", endpoint);
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "Error al conectar con el servidor"
                };
            }
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(endpoint, content);
                return await ProcessResponse<T>(response);
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                return new ApiResponse<T> { Success = false, Message = "Token no disponible" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar PUT a {Endpoint}", endpoint);
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "Error al conectar con el servidor"
                };
            }
        }

        public async Task<ApiResponse<T>> PatchAsync<T>(string endpoint, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PatchAsync(endpoint, content);
                return await ProcessResponse<T>(response);
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                return new ApiResponse<T> { Success = false, Message = "Token no disponible" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar PATCH a {Endpoint}", endpoint);
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "Error al conectar con el servidor"
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                return await ProcessResponse<bool>(response);
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                return new ApiResponse<bool> { Success = false, Message = "Token no disponible" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar DELETE a {Endpoint}", endpoint);
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error al conectar con el servidor"
                };
            }
        }

        public async Task<ApiResponse<T>> PostFormDataAsync<T>(string endpoint, MultipartFormDataContent formData)
        {
            try
            {
                var response = await _httpClient.PostAsync(endpoint, formData);
                return await ProcessResponse<T>(response);
            }
            catch (AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
                return new ApiResponse<T> { Success = false, Message = "Token no disponible" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar POST con FormData a {Endpoint}", endpoint);
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "Error al conectar con el servidor"
                };
            }
        }

        private async Task<ApiResponse<T>> ProcessResponse<T>(HttpResponseMessage response)
        {
            var apiResponse = new ApiResponse<T>();

            if (response.IsSuccessStatusCode)
            {
                apiResponse.Success = true;

                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    // Para respuestas 204 No Content
                    if (typeof(T) == typeof(bool))
                    {
                        apiResponse.Data = (T)(object)true;
                    }
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        try
                        {
                            apiResponse.Data = JsonSerializer.Deserialize<T>(content, _jsonOptions);
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogError(ex, "Error al deserializar respuesta JSON");
                            apiResponse.Success = false;
                            apiResponse.Message = "Error al procesar la respuesta del servidor";
                        }
                    }
                }
            }
            else
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = (int)response.StatusCode;

                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        apiResponse.Message = "Recurso no encontrado";
                        break;
                    case HttpStatusCode.Unauthorized:
                        apiResponse.Message = "No autorizado";
                        break;
                    case HttpStatusCode.Forbidden:
                        apiResponse.Message = "Acceso denegado";
                        break;
                    case HttpStatusCode.BadRequest:
                        try
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            if (!string.IsNullOrEmpty(errorContent))
                            {
                                var validationErrors = JsonSerializer.Deserialize<ValidationProblemDetails>(
                                    errorContent, _jsonOptions);

                                if (validationErrors?.Errors != null)
                                {
                                    if (validationErrors?.Errors != null)
                                    {
                                        // Convertir Dictionary<string, List<string>> a Dictionary<string, string[]>
                                        apiResponse.ValidationErrors = validationErrors.Errors
                                            .ToDictionary(
                                                kvp => kvp.Key,
                                                kvp => kvp.Value.ToArray()
                                            );
                                        apiResponse.Message = "Error de validación";
                                    }
                                    else
                                    {
                                        apiResponse.Message = errorContent;
                                    }
                                    apiResponse.Message = "Error de validación";
                                }
                                else
                                {
                                    apiResponse.Message = errorContent;
                                }
                            }
                        }
                        catch
                        {
                            apiResponse.Message = "Solicitud inválida";
                        }
                        break;
                    case HttpStatusCode.InternalServerError:
                        apiResponse.Message = "Error interno del servidor";
                        break;
                    default:
                        apiResponse.Message = $"Error: {response.StatusCode}";
                        break;
                }
            }

            return apiResponse;
        }


        // New method implementation for byte array download
        public async Task<ApiResponse<byte[]>> GetBytesAsync(string endpoint)
        {
            return await GetBytesAsync(endpoint, null);
        }

        public async Task<ApiResponse<byte[]>> GetBytesAsync(string endpoint, Dictionary<string, string>? headers)
        {
            try
            {
                // Add any custom headers
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    return ApiResponse<byte[]>.SuccessResponse(bytes);
                }

                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to download file from {Endpoint}. Status: {StatusCode}, Error: {Error}",
                    endpoint, response.StatusCode, error);

                return ApiResponse<byte[]>.ErrorResponse($"Error downloading file: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file from {Endpoint}", endpoint);
                return ApiResponse<byte[]>.ErrorResponse($"Error: {ex.Message}");
            }
            finally
            {
                // Clean up custom headers
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        _httpClient.DefaultRequestHeaders.Remove(header.Key);
                    }
                }
            }
        }

        // Optional: Stream implementation for large files
        public async Task<ApiResponse<Stream>> GetStreamAsync(string endpoint)
        {
            try
            {

                // Use HttpCompletionOption.ResponseHeadersRead for streaming
                var response = await _httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    return ApiResponse<Stream>.SuccessResponse(stream);
                }

                var error = await response.Content.ReadAsStringAsync();
                return ApiResponse<Stream>.ErrorResponse($"Error: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error streaming from {Endpoint}", endpoint);
                return ApiResponse<Stream>.ErrorResponse($"Error: {ex.Message}");
            }
        }

        private async Task<ApiResponse<T>> ProcessResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return ApiResponse<T>.SuccessResponse(data!);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error deserializing response");
                    return ApiResponse<T>.ErrorResponse("Error processing response data");
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("API request failed. Status: {StatusCode}, Error: {Error}",
                response.StatusCode, errorContent);

            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.NotFound => ApiResponse<T>.ErrorResponse("Resource not found"),
                System.Net.HttpStatusCode.Unauthorized => ApiResponse<T>.ErrorResponse("Unauthorized"),
                System.Net.HttpStatusCode.Forbidden => ApiResponse<T>.ErrorResponse("Access denied"),
                System.Net.HttpStatusCode.BadRequest => ApiResponse<T>.ErrorResponse(errorContent ?? "Bad request"),
                _ => ApiResponse<T>.ErrorResponse($"Error: {response.StatusCode}")
            };
        }
    }

    public class ValidationProblemDetails
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? Detail { get; set; }
        public string? Instance { get; set; }
        public Dictionary<string, List<string>>? Errors { get; set; }
    }
}