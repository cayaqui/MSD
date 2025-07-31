using Core.Dtos.Files;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Web.Models.Responses;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class FileService : IFileService
    {
        private readonly IApiService _apiService;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileService> _logger;
        private const string BaseEndpoint = "/api/files";

        // Configuración de límites
        private readonly long _maxFileSize;
        private readonly HashSet<string> _allowedExtensions;
        private readonly Dictionary<string, string> _mimeTypes;

        public FileService(
            IApiService apiService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<FileService> logger)
        {
            _apiService = apiService;
            _httpClient = httpClientFactory.CreateClient("EzProAPI");
            _configuration = configuration;
            _logger = logger;

            // Cargar configuración
            _maxFileSize = configuration.GetValue<long>("Features:MaxFileUploadSize", 10485760); // 10MB por defecto
            _allowedExtensions = configuration.GetSection("Features:SupportedFileTypes")
                .Get<List<string>>()?.ToHashSet(StringComparer.OrdinalIgnoreCase)
                ?? new HashSet<string> { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg", ".dwg", ".dxf" };

            _mimeTypes = new Dictionary<string, string>
            {
                { ".pdf", "application/pdf" },
                { ".doc", "application/msword" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".png", "image/png" },
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".dwg", "application/acad" },
                { ".dxf", "application/dxf" }
            };
        }

        // Operaciones básicas de archivos
        public async Task<ApiResponse<FileUploadResultDto>> UploadFileAsync(
            Stream fileStream, string fileName, string contentType, string category = "general")
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var streamContent = new StreamContent(fileStream);

                streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Add(streamContent, "file", fileName);
                content.Add(new StringContent(category), "category");

                var response = await _httpClient.PostAsync($"{BaseEndpoint}/upload", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<FileUploadResultDto>();
                    return ApiResponse<FileUploadResultDto>.SuccessResponse(result!);
                }

                var error = await response.Content.ReadAsStringAsync();
                return ApiResponse<FileUploadResultDto>.ErrorResponse($"Error al subir archivo: {error}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir archivo {FileName}", fileName);
                return ApiResponse<FileUploadResultDto>.ErrorResponse("Error al subir el archivo");
            }
        }

        public async Task<ApiResponse<FileUploadResultDto>> UploadProjectFileAsync(
            Guid projectId, Stream fileStream, string fileName, string contentType, string category = "documents")
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var streamContent = new StreamContent(fileStream);

                streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Add(streamContent, "file", fileName);
                content.Add(new StringContent(category), "category");
                content.Add(new StringContent(projectId.ToString()), "projectId");

                var response = await _httpClient.PostAsync($"{BaseEndpoint}/projects/{projectId}/upload", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<FileUploadResultDto>();
                    return ApiResponse<FileUploadResultDto>.SuccessResponse(result!);
                }

                var error = await response.Content.ReadAsStringAsync();
                return ApiResponse<FileUploadResultDto>.ErrorResponse($"Error al subir archivo: {error}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir archivo del proyecto {ProjectId}", projectId);
                return ApiResponse<FileUploadResultDto>.ErrorResponse("Error al subir el archivo del proyecto");
            }
        }

        public async Task<ApiResponse<byte[]>> DownloadFileAsync(Guid fileId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/{fileId}/download");

                if (response.IsSuccessStatusCode)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    return ApiResponse<byte[]>.SuccessResponse(bytes);
                }

                return ApiResponse<byte[]>.ErrorResponse("Error al descargar el archivo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al descargar archivo {FileId}", fileId);
                return ApiResponse<byte[]>.ErrorResponse("Error al descargar el archivo");
            }
        }

        public async Task<ApiResponse<FileDto>> GetFileInfoAsync(Guid fileId)
        {
            try
            {
                return await _apiService.GetAsync<FileDto>($"{BaseEndpoint}/{fileId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener información del archivo {FileId}", fileId);
                return ApiResponse<FileDto>.ErrorResponse("Error al obtener información del archivo");
            }
        }

        public async Task<ApiResponse<bool>> DeleteFileAsync(Guid fileId)
        {
            try
            {
                return await _apiService.DeleteAsync($"{BaseEndpoint}/{fileId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar archivo {FileId}", fileId);
                return ApiResponse<bool>.ErrorResponse("Error al eliminar el archivo");
            }
        }

        // Operaciones múltiples
        public async Task<ApiResponse<List<FileUploadResultDto>>> UploadMultipleFilesAsync(List<FileUploadDto> files)
        {
            try
            {
                var results = new List<FileUploadResultDto>();

                foreach (var file in files)
                {
                    var result = await UploadFileAsync(
                        file.FileStream,
                        file.FileName,
                        file.ContentType,
                        file.Category ?? "general");

                    if (result.Success && result.Data != null)
                    {
                        results.Add(result.Data);
                    }
                    else
                    {
                        results.Add(new FileUploadResultDto
                        {
                            FileName = file.FileName,
                            Success = false,
                            ErrorMessage = result.Message
                        });
                    }
                }

                return ApiResponse<List<FileUploadResultDto>>.SuccessResponse(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir múltiples archivos");
                return ApiResponse<List<FileUploadResultDto>>.ErrorResponse("Error al subir los archivos");
            }
        }

        public async Task<ApiResponse<int>> DeleteMultipleFilesAsync(List<Guid> fileIds)
        {
            try
            {
                var response = await _apiService.PostAsync<int>($"{BaseEndpoint}/delete-multiple", new { fileIds });
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar múltiples archivos");
                return ApiResponse<int>.ErrorResponse("Error al eliminar los archivos");
            }
        }

        // Búsqueda y listado
        public async Task<ApiResponse<List<FileDto>>> GetProjectFilesAsync(Guid projectId, string? category = null)
        {
            try
            {
                var endpoint = $"{BaseEndpoint}/projects/{projectId}";
                if (!string.IsNullOrEmpty(category))
                {
                    endpoint += $"?category={category}";
                }

                return await _apiService.GetAsync<List<FileDto>>(endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener archivos del proyecto {ProjectId}", projectId);
                return ApiResponse<List<FileDto>>.ErrorResponse("Error al obtener los archivos del proyecto");
            }
        }

        public async Task<ApiResponse<PagedResult<FileDto>>> SearchFilesAsync(FileSearchDto searchCriteria)
        {
            try
            {
                return await _apiService.PostAsync<PagedResult<FileDto>>($"{BaseEndpoint}/search", searchCriteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar archivos");
                return ApiResponse<PagedResult<FileDto>>.ErrorResponse("Error al buscar archivos");
            }
        }

        public async Task<ApiResponse<List<FileCategoryDto>>> GetFileCategoriesAsync()
        {
            try
            {
                return await _apiService.GetAsync<List<FileCategoryDto>>($"{BaseEndpoint}/categories");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías de archivos");
                return ApiResponse<List<FileCategoryDto>>.ErrorResponse("Error al obtener las categorías");
            }
        }

        // Operaciones con imágenes
        public async Task<ApiResponse<FileUploadResultDto>> UploadImageAsync(
            Stream imageStream, string fileName, string contentType, ImageProcessingOptions? options = null)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var streamContent = new StreamContent(imageStream);

                streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                content.Add(streamContent, "file", fileName);
                content.Add(new StringContent("images"), "category");

                if (options != null)
                {
                    content.Add(new StringContent(System.Text.Json.JsonSerializer.Serialize(options)), "options");
                }

                var response = await _httpClient.PostAsync($"{BaseEndpoint}/upload-image", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<FileUploadResultDto>();
                    return ApiResponse<FileUploadResultDto>.SuccessResponse(result!);
                }

                var error = await response.Content.ReadAsStringAsync();
                return ApiResponse<FileUploadResultDto>.ErrorResponse($"Error al subir imagen: {error}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir imagen {FileName}", fileName);
                return ApiResponse<FileUploadResultDto>.ErrorResponse("Error al subir la imagen");
            }
        }

        public async Task<ApiResponse<byte[]>> GetThumbnailAsync(Guid fileId, int width = 150, int height = 150)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseEndpoint}/{fileId}/thumbnail?width={width}&height={height}");

                if (response.IsSuccessStatusCode)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    return ApiResponse<byte[]>.SuccessResponse(bytes);
                }

                return ApiResponse<byte[]>.ErrorResponse("Error al obtener miniatura");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener miniatura del archivo {FileId}", fileId);
                return ApiResponse<byte[]>.ErrorResponse("Error al obtener la miniatura");
            }
        }

        // Validación
        public async Task<ApiResponse<bool>> ValidateFileAsync(Stream fileStream, string fileName, string contentType)
        {
            try
            {
                // Validación local primero
                if (!IsFileTypeAllowed(fileName, contentType))
                {
                    return ApiResponse<bool>.ErrorResponse("Tipo de archivo no permitido");
                }

                if (!IsFileSizeValid(fileStream.Length))
                {
                    return ApiResponse<bool>.ErrorResponse($"El archivo excede el tamaño máximo permitido de {_maxFileSize / 1048576}MB");
                }

                // Validación adicional en el servidor si es necesaria
                return ApiResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar archivo {FileName}", fileName);
                return ApiResponse<bool>.ErrorResponse("Error al validar el archivo");
            }
        }

        public bool IsFileSizeValid(long fileSize)
        {
            return fileSize > 0 && fileSize <= _maxFileSize;
        }

        public bool IsFileTypeAllowed(string fileName, string contentType)
        {
            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();

            if (string.IsNullOrEmpty(extension))
                return false;

            // Verificar extensión
            if (!_allowedExtensions.Contains(extension))
                return false;

            // Verificar que el content type coincida con la extensión
            if (_mimeTypes.TryGetValue(extension, out var expectedMimeType))
            {
                return contentType.Equals(expectedMimeType, StringComparison.OrdinalIgnoreCase);
            }

            return true;
        }

        // URLs y acceso directo
        public string GetFileUrl(Guid fileId)
        {
            return $"{_configuration["ApiSettings:BaseUrl"]}{BaseEndpoint}/{fileId}/download";
        }

        public string GetThumbnailUrl(Guid fileId, int width = 150, int height = 150)
        {
            return $"{_configuration["ApiSettings:BaseUrl"]}{BaseEndpoint}/{fileId}/thumbnail?width={width}&height={height}";
        }

        // Metadatos
        public async Task<ApiResponse<FileDto>> UpdateFileMetadataAsync(Guid fileId, UpdateFileMetadataDto metadata)
        {
            try
            {
                return await _apiService.PatchAsync<FileDto>($"{BaseEndpoint}/{fileId}/metadata", metadata);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar metadatos del archivo {FileId}", fileId);
                return ApiResponse<FileDto>.ErrorResponse("Error al actualizar los metadatos");
            }
        }

        public async Task<ApiResponse<bool>> MoveFileAsync(Guid fileId, Guid newProjectId, string? newCategory = null)
        {
            try
            {
                var request = new
                {
                    projectId = newProjectId,
                    category = newCategory
                };

                var response = await _apiService.PostAsync<bool>($"{BaseEndpoint}/{fileId}/move", request);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mover archivo {FileId}", fileId);
                return ApiResponse<bool>.ErrorResponse("Error al mover el archivo");
            }
        }
    }
}