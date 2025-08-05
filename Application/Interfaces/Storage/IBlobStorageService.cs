using Core.DTOs.Documents.Blobs;
using Core.Enums.Documents;

namespace Application.Interfaces.Storage;

public interface IBlobStorageService
{
    // Upload operations
    Task<BlobUploadResult> UploadAsync(Stream fileStream, string containerName, string blobName, string contentType, Dictionary<string, string>? metadata = null);
    Task<BlobUploadResult> UploadAsync(byte[] fileData, string containerName, string blobName, string contentType, Dictionary<string, string>? metadata = null);
    Task<BlobUploadResult> UploadChunkedAsync(Stream fileStream, string containerName, string blobName, string contentType, long chunkSize = 4 * 1024 * 1024);
    
    // Download operations
    Task<Stream?> DownloadAsync(string containerName, string blobName);
    Task<byte[]?> DownloadBytesAsync(string containerName, string blobName);
    Task<BlobDownloadResult?> DownloadWithMetadataAsync(string containerName, string blobName);
    Task<bool> DownloadToFileAsync(string containerName, string blobName, string localFilePath);
    
    // Blob management
    Task<bool> ExistsAsync(string containerName, string blobName);
    Task<bool> DeleteAsync(string containerName, string blobName);
    Task<bool> CopyAsync(string sourceContainer, string sourceBlobName, string destContainer, string destBlobName);
    Task<bool> MoveAsync(string sourceContainer, string sourceBlobName, string destContainer, string destBlobName);
    
    // Metadata operations
    Task<Dictionary<string, string>?> GetMetadataAsync(string containerName, string blobName);
    Task<bool> SetMetadataAsync(string containerName, string blobName, Dictionary<string, string> metadata);
    
    // Container operations
    Task<bool> CreateContainerIfNotExistsAsync(string containerName, BlobContainerAccessLevel accessLevel = BlobContainerAccessLevel.Private);
    Task<bool> DeleteContainerAsync(string containerName);
    Task<bool> ContainerExistsAsync(string containerName);
    Task<IEnumerable<string>> ListContainersAsync();
    
    // Blob listing
    Task<IEnumerable<BlobItem>> ListBlobsAsync(string containerName, string? prefix = null);
    Task<BlobListResult> ListBlobsPagedAsync(string containerName, string? prefix = null, int pageSize = 100, string? continuationToken = null);
    
    // SAS token generation
    Task<string> GenerateSasTokenAsync(string containerName, string blobName, BlobSasPermissions permissions, DateTimeOffset expiresOn);
    Task<string> GenerateContainerSasTokenAsync(string containerName, BlobContainerSasPermissions permissions, DateTimeOffset expiresOn);
    
    // URL generation
    string GetBlobUrl(string containerName, string blobName);
    string GetBlobUrlWithSasToken(string containerName, string blobName, string sasToken);
    
    // Properties
    Task<BlobProperties?> GetPropertiesAsync(string containerName, string blobName);
    Task<bool> SetContentTypeAsync(string containerName, string blobName, string contentType);
    
    // Advanced operations
    Task<string> CreateSnapshotAsync(string containerName, string blobName);
    Task<bool> RestoreFromSnapshotAsync(string containerName, string blobName, string snapshotId);
    Task<bool> SetAccessTierAsync(string containerName, string blobName, BlobAccessTier accessTier);
}
