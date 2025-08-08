namespace Web.Services.Interfaces;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
    Task PostAsync<TRequest>(string endpoint, TRequest data);
    Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data);
    Task PutAsync<TRequest>(string endpoint, TRequest data);
    Task<bool> DeleteAsync(string endpoint);
    Task<byte[]> GetBytesAsync(string endpoint);
    Task<TResponse?> PostFileAsync<TResponse>(string endpoint, MultipartFormDataContent content);
}