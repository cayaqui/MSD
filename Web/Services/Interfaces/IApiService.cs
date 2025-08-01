using Web.Models.Responses;

namespace Web.Services.Interfaces
{
    public interface IApiService
    {
        Task<ApiResponse<T>> GetAsync<T>(string endpoint);
        Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data);
        Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data);
        Task<ApiResponse<T>> PatchAsync<T>(string endpoint, object data);
        Task<ApiResponse<bool>> DeleteAsync(string endpoint);
        Task<ApiResponse<T>> PostFormDataAsync<T>(string endpoint, MultipartFormDataContent formData);
        /// <summary>
        /// Gets byte array data from the specified endpoint (for file downloads)
        /// </summary>
        Task<ApiResponse<byte[]>> GetBytesAsync(string endpoint);

        // Optional: Add method with headers support
        Task<ApiResponse<byte[]>> GetBytesAsync(string endpoint, Dictionary<string, string>? headers);

        // Optional: Add method for streaming large files
        Task<ApiResponse<Stream>> GetStreamAsync(string endpoint);
    }
}