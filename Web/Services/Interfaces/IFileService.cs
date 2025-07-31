using Core.Dtos.Files;
using Web.Models.Responses;

namespace Web.Services.Interfaces
{
    public interface IFileService
    {
        // Operaciones básicas de archivos
        Task<ApiResponse<FileUploadResultDto>> UploadFileAsync(Stream fileStream, string fileName, string contentType, string category = "general");
        Task<ApiResponse<FileUploadResultDto>> UploadProjectFileAsync(Guid projectId, Stream fileStream, string fileName, string contentType, string category = "documents");
        Task<ApiResponse<byte[]>> DownloadFileAsync(Guid fileId);
        Task<ApiResponse<FileDto>> GetFileInfoAsync(Guid fileId);
        Task<ApiResponse<bool>> DeleteFileAsync(Guid fileId);

        // Operaciones múltiples
        Task<ApiResponse<List<FileUploadResultDto>>> UploadMultipleFilesAsync(List<FileUploadDto> files);
        Task<ApiResponse<int>> DeleteMultipleFilesAsync(List<Guid> fileIds);

        // Búsqueda y listado
        Task<ApiResponse<List<FileDto>>> GetProjectFilesAsync(Guid projectId, string? category = null);
        Task<ApiResponse<PagedResult<FileDto>>> SearchFilesAsync(FileSearchDto searchCriteria);
        Task<ApiResponse<List<FileCategoryDto>>> GetFileCategoriesAsync();

        // Operaciones con imágenes
        Task<ApiResponse<FileUploadResultDto>> UploadImageAsync(Stream imageStream, string fileName, string contentType, ImageProcessingOptions? options = null);
        Task<ApiResponse<byte[]>> GetThumbnailAsync(Guid fileId, int width = 150, int height = 150);

        // Validación
        Task<ApiResponse<bool>> ValidateFileAsync(Stream fileStream, string fileName, string contentType);
        bool IsFileSizeValid(long fileSize);
        bool IsFileTypeAllowed(string fileName, string contentType);

        // URLs y acceso directo
        string GetFileUrl(Guid fileId);
        string GetThumbnailUrl(Guid fileId, int width = 150, int height = 150);

        // Metadatos
        Task<ApiResponse<FileDto>> UpdateFileMetadataAsync(Guid fileId, UpdateFileMetadataDto metadata);
        Task<ApiResponse<bool>> MoveFileAsync(Guid fileId, Guid newProjectId, string? newCategory = null);
    }
}