using System.Net.Http.Json;
using System.Text.Json;
using Web.Services.Interfaces;

namespace Web.Services.Implementation;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IToastService _toastService;
    private readonly ILoggingService _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public ApiService(IHttpClientFactory httpClientFactory, IToastService toastService, ILoggingService logger)
    {
        _httpClient = httpClientFactory.CreateClient("EzProAPI");
        _toastService = toastService;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        _logger.LogDebug("ApiService initialized with base URL: {0}", _httpClient.BaseAddress);
    }
    
    public async Task<T?> GetAsync<T>(string endpoint)
    {
        _logger.LogDebug("GET request to: {0}", endpoint);
        
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            _logger.LogDebug("GET response from {0}: {1}", endpoint, response.StatusCode);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("GET response content length: {0} bytes", json.Length);
                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            
            _logger.LogWarning("GET request failed with status: {0}", response.StatusCode);
            await HandleErrorResponse(response);
            return default;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during GET request to {0}", endpoint);
            _toastService.ShowError("Network error. Please check your connection.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during GET request to {0}", endpoint);
            _toastService.ShowError("An unexpected error occurred.");
            throw;
        }
    }
    
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
            }
            
            await HandleErrorResponse(response);
            return default;
        }
        catch (HttpRequestException ex)
        {
            _toastService.ShowError("Network error. Please check your connection.");
            throw;
        }
        catch (Exception ex)
        {
            _toastService.ShowError("An unexpected error occurred.");
            throw;
        }
    }
    
    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, data);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
            }
            
            await HandleErrorResponse(response);
            return default;
        }
        catch (HttpRequestException ex)
        {
            _toastService.ShowError("Network error. Please check your connection.");
            throw;
        }
        catch (Exception ex)
        {
            _toastService.ShowError("An unexpected error occurred.");
            throw;
        }
    }
    
    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            await HandleErrorResponse(response);
            return false;
        }
        catch (HttpRequestException ex)
        {
            _toastService.ShowError("Network error. Please check your connection.");
            throw;
        }
        catch (Exception ex)
        {
            _toastService.ShowError("An unexpected error occurred.");
            throw;
        }
    }
    
    public async Task<byte[]> GetBytesAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            
            await HandleErrorResponse(response);
            return Array.Empty<byte>();
        }
        catch (HttpRequestException ex)
        {
            _toastService.ShowError("Network error. Please check your connection.");
            throw;
        }
        catch (Exception ex)
        {
            _toastService.ShowError("An unexpected error occurred.");
            throw;
        }
    }
    
    public async Task<TResponse?> PostFileAsync<TResponse>(string endpoint, MultipartFormDataContent content)
    {
        try
        {
            var response = await _httpClient.PostAsync(endpoint, content);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(json, _jsonOptions);
            }
            
            await HandleErrorResponse(response);
            return default;
        }
        catch (HttpRequestException ex)
        {
            _toastService.ShowError("Network error. Please check your connection.");
            throw;
        }
        catch (Exception ex)
        {
            _toastService.ShowError("An unexpected error occurred.");
            throw;
        }
    }
    
    private async Task HandleErrorResponse(HttpResponseMessage response)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        _logger.LogError("API Error - Status: {0}, Content: {1}", response.StatusCode, errorContent);
        
        switch (response.StatusCode)
        {
            case System.Net.HttpStatusCode.Unauthorized:
                _toastService.ShowError("You are not authorized to perform this action.");
                break;
            case System.Net.HttpStatusCode.Forbidden:
                _toastService.ShowError("You don't have permission to access this resource.");
                break;
            case System.Net.HttpStatusCode.NotFound:
                _toastService.ShowError("The requested resource was not found.");
                break;
            case System.Net.HttpStatusCode.BadRequest:
                if (!string.IsNullOrEmpty(errorContent))
                {
                    try
                    {
                        var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(errorContent, _jsonOptions);
                        if (problemDetails?.Errors?.Any() == true)
                        {
                            var firstError = problemDetails.Errors.First();
                            _toastService.ShowError($"{firstError.Key}: {string.Join(", ", firstError.Value)}");
                        }
                        else
                        {
                            _toastService.ShowError("Invalid request. Please check your input.");
                        }
                    }
                    catch
                    {
                        _toastService.ShowError("Invalid request. Please check your input.");
                    }
                }
                else
                {
                    _toastService.ShowError("Invalid request. Please check your input.");
                }
                break;
            default:
                _toastService.ShowError($"An error occurred: {response.StatusCode}");
                break;
        }
    }
    
    private class ValidationProblemDetails
    {
        public string? Title { get; set; }
        public int? Status { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}