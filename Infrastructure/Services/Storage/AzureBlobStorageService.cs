using Application.Interfaces.Storage;
using Azure;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Core.DTOs.Documents.Blobs;
using Core.Enums.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BlobContainerSasPermissions = Core.Enums.Documents.BlobContainerSasPermissions;
using BlobDownloadResult = Core.DTOs.Documents.Blobs.BlobDownloadResult;
using BlobItem = Core.DTOs.Documents.Blobs.BlobItem;
using BlobProperties = Core.DTOs.Documents.Blobs.BlobProperties;
using BlobSasPermissions = Core.Enums.Documents.BlobSasPermissions;

namespace Infrastructure.Services.Storage;

public class AzureBlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureBlobStorageService> _logger;
    private readonly string _storageAccountName;
    private readonly StorageSharedKeyCredential? _storageSharedKeyCredential;
    private readonly bool _useManagedIdentity;

    public AzureBlobStorageService(
        IConfiguration configuration,
        ILogger<AzureBlobStorageService> logger
    )
    {
        _logger = logger;

        // Get storage account name from configuration
        _storageAccountName = configuration["AzureStorage:AccountName"] 
            ?? throw new InvalidOperationException("Azure Storage account name not configured");

        // Check if we should use Managed Identity or connection string
        var useManagedIdentity = configuration.GetValue<bool>("AzureStorage:UseManagedIdentity", true);
        _useManagedIdentity = useManagedIdentity;

        if (useManagedIdentity)
        {
            // Use DefaultAzureCredential which works for both local development and production
            // Local: Uses Azure CLI, Visual Studio, VS Code, or Azure PowerShell credentials
            // Production: Uses Managed Identity
            var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                // Exclude credentials that might cause issues
                ExcludeEnvironmentCredential = false,
                ExcludeInteractiveBrowserCredential = true,
                ExcludeAzureCliCredential = false,
                ExcludeAzurePowerShellCredential = false,
                ExcludeSharedTokenCacheCredential = false,
                ExcludeVisualStudioCodeCredential = false,
                ExcludeVisualStudioCredential = false,
                ExcludeManagedIdentityCredential = false,
                
                // Set tenant ID if specified
                TenantId = configuration["AzureStorage:TenantId"]
            });

            var blobServiceUri = new Uri($"https://{_storageAccountName}.blob.core.windows.net");
            _blobServiceClient = new BlobServiceClient(blobServiceUri, credential);
            
            _logger.LogInformation("Initialized Azure Blob Storage with Managed Identity for account: {AccountName}", _storageAccountName);
        }
        else
        {
            // Fall back to connection string if specified
            var connectionString = configuration.GetConnectionString("AzureStorage");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Azure Storage connection string not configured");
            }

            _blobServiceClient = new BlobServiceClient(connectionString);

            // Extract account key for SAS token generation
            var parts = connectionString.Split(';');
            string? accountKey = null;
            foreach (var part in parts)
            {
                if (part.StartsWith("AccountKey="))
                {
                    accountKey = part.Substring("AccountKey=".Length);
                    break;
                }
            }

            if (!string.IsNullOrEmpty(accountKey))
            {
                _storageSharedKeyCredential = new StorageSharedKeyCredential(_storageAccountName, accountKey);
            }
            
            _logger.LogInformation("Initialized Azure Blob Storage with connection string for account: {AccountName}", _storageAccountName);
        }
    }

    #region Upload Operations

    public async Task<BlobUploadResult> UploadAsync(
        Stream fileStream,
        string containerName,
        string blobName,
        string contentType,
        Dictionary<string, string>? metadata = null
    )
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType },
                Metadata = metadata,
            };

            var response = await blobClient.UploadAsync(fileStream, options);

            return new BlobUploadResult
            {
                Success = true,
                BlobUrl = blobClient.Uri.ToString(),
                BlobName = blobName,
                ContainerName = containerName,
                ETag = response.Value.ETag.ToString(),
                LastModified = response.Value.LastModified.DateTime,
                ContentLength = fileStream.Length,
                ContentType = contentType,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error uploading blob {BlobName} to container {ContainerName}",
                blobName,
                containerName
            );
            return new BlobUploadResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<BlobUploadResult> UploadAsync(
        byte[] fileData,
        string containerName,
        string blobName,
        string contentType,
        Dictionary<string, string>? metadata = null
    )
    {
        using var stream = new MemoryStream(fileData);
        return await UploadAsync(stream, containerName, blobName, contentType, metadata);
    }

    public async Task<BlobUploadResult> UploadChunkedAsync(
        Stream fileStream,
        string containerName,
        string blobName,
        string contentType,
        long chunkSize = 4 * 1024 * 1024
    )
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType },
                TransferOptions = new StorageTransferOptions
                {
                    MaximumConcurrency = 8,
                    InitialTransferSize = chunkSize,
                    MaximumTransferSize = chunkSize,
                },
            };

            var response = await blobClient.UploadAsync(fileStream, options);

            return new BlobUploadResult
            {
                Success = true,
                BlobUrl = blobClient.Uri.ToString(),
                BlobName = blobName,
                ContainerName = containerName,
                ETag = response.Value.ETag.ToString(),
                LastModified = response.Value.LastModified.DateTime,
                ContentLength = fileStream.Length,
                ContentType = contentType,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error uploading chunked blob {BlobName} to container {ContainerName}",
                blobName,
                containerName
            );
            return new BlobUploadResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    #endregion

    #region Download Operations

    public async Task<Stream?> DownloadAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
                return null;

            var response = await blobClient.DownloadStreamingAsync();
            return response.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error downloading blob {BlobName} from container {ContainerName}",
                blobName,
                containerName
            );
            return null;
        }
    }

    public async Task<byte[]?> DownloadBytesAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
                return null;

            var response = await blobClient.DownloadContentAsync();
            return response.Value.Content.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error downloading blob bytes {BlobName} from container {ContainerName}",
                blobName,
                containerName
            );
            return null;
        }
    }

    public async Task<BlobDownloadResult?> DownloadWithMetadataAsync(
        string containerName,
        string blobName
    )
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
                return null;

            var response = await blobClient.DownloadStreamingAsync();
            var properties = await blobClient.GetPropertiesAsync();

            return new BlobDownloadResult
            {
                Content = response.Value.Content,
                ContentType = response.Value.Details.ContentType,
                ContentLength = response.Value.Details.ContentLength,
                ETag = response.Value.Details.ETag.ToString(),
                LastModified = response.Value.Details.LastModified.DateTime,
                Metadata = (Dictionary<string, string>)properties.Value.Metadata,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error downloading blob with metadata {BlobName} from container {ContainerName}",
                blobName,
                containerName
            );
            return null;
        }
    }

    public async Task<bool> DownloadToFileAsync(
        string containerName,
        string blobName,
        string localFilePath
    )
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DownloadToAsync(localFilePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error downloading blob to file {BlobName} from container {ContainerName}",
                blobName,
                containerName
            );
            return false;
        }
    }

    #endregion

    #region Blob Management

    public async Task<bool> ExistsAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.ExistsAsync();
            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error checking if blob exists {BlobName} in container {ContainerName}",
                blobName,
                containerName
            );
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var response = await blobClient.DeleteIfExistsAsync();
            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error deleting blob {BlobName} from container {ContainerName}",
                blobName,
                containerName
            );
            return false;
        }
    }

    public async Task<bool> CopyAsync(
        string sourceContainer,
        string sourceBlobName,
        string destContainer,
        string destBlobName
    )
    {
        try
        {
            var sourceContainerClient = _blobServiceClient.GetBlobContainerClient(sourceContainer);
            var sourceBlobClient = sourceContainerClient.GetBlobClient(sourceBlobName);

            var destContainerClient = _blobServiceClient.GetBlobContainerClient(destContainer);
            var destBlobClient = destContainerClient.GetBlobClient(destBlobName);

            var operation = await destBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
            await operation.WaitForCompletionAsync();

            return operation.HasCompleted && !operation.HasValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error copying blob from {SourceBlob} to {DestBlob}",
                sourceBlobName,
                destBlobName
            );
            return false;
        }
    }

    public async Task<bool> MoveAsync(
        string sourceContainer,
        string sourceBlobName,
        string destContainer,
        string destBlobName
    )
    {
        var copied = await CopyAsync(sourceContainer, sourceBlobName, destContainer, destBlobName);
        if (copied)
        {
            return await DeleteAsync(sourceContainer, sourceBlobName);
        }
        return false;
    }

    #endregion

    #region Metadata Operations

    public async Task<Dictionary<string, string>?> GetMetadataAsync(
        string containerName,
        string blobName
    )
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var properties = await blobClient.GetPropertiesAsync();
            return (Dictionary<string, string>?)properties.Value.Metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting metadata for blob {BlobName} in container {ContainerName}",
                blobName,
                containerName
            );
            return null;
        }
    }

    public async Task<bool> SetMetadataAsync(
        string containerName,
        string blobName,
        Dictionary<string, string> metadata
    )
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.SetMetadataAsync(metadata);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error setting metadata for blob {BlobName} in container {ContainerName}",
                blobName,
                containerName
            );
            return false;
        }
    }

    #endregion

    #region Container Operations

    public async Task<bool> CreateContainerIfNotExistsAsync(
        string containerName,
        BlobContainerAccessLevel accessLevel = BlobContainerAccessLevel.Private
    )
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var azureAccessLevel = accessLevel switch
            {
                BlobContainerAccessLevel.Blob => PublicAccessType.Blob,
                BlobContainerAccessLevel.Container => PublicAccessType.BlobContainer,
                _ => PublicAccessType.None,
            };

            await containerClient.CreateIfNotExistsAsync(azureAccessLevel);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating container {ContainerName}", containerName);
            return false;
        }
    }

    public async Task<bool> DeleteContainerAsync(string containerName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var response = await containerClient.DeleteIfExistsAsync();
            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting container {ContainerName}", containerName);
            return false;
        }
    }

    public async Task<bool> ContainerExistsAsync(string containerName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var response = await containerClient.ExistsAsync();
            return response.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error checking if container exists {ContainerName}",
                containerName
            );
            return false;
        }
    }

    public async Task<IEnumerable<string>> ListContainersAsync()
    {
        try
        {
            var containers = new List<string>();
            await foreach (var container in _blobServiceClient.GetBlobContainersAsync())
            {
                containers.Add(container.Name);
            }
            return containers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing containers");
            return Enumerable.Empty<string>();
        }
    }

    #endregion

    #region Blob Listing

    public async Task<IEnumerable<BlobItem>> ListBlobsAsync(
        string containerName,
        string? prefix = null
    )
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = new List<BlobItem>();

            await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: prefix))
            {
                blobs.Add(
                    new BlobItem
                    {
                        Name = blobItem.Name,
                        ContainerName = containerName,
                        Size = blobItem.Properties.ContentLength,
                        ContentType = blobItem.Properties.ContentType,
                        LastModified = blobItem.Properties.LastModified?.DateTime,
                        ETag = blobItem.Properties.ETag?.ToString(),
                        IsDeleted = blobItem.Deleted,
                    }
                );
            }

            return blobs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing blobs in container {ContainerName}", containerName);
            return Enumerable.Empty<BlobItem>();
        }
    }

    public async Task<BlobListResult> ListBlobsPagedAsync(
        string containerName,
        string? prefix = null,
        int pageSize = 100,
        string? continuationToken = null
    )
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var result = new BlobListResult();

            var pages = containerClient
                .GetBlobsAsync(prefix: prefix)
                .AsPages(continuationToken, pageSize);

            await foreach (var page in pages)
            {
                foreach (var blobItem in page.Values)
                {
                    result.Items.Add(
                        new BlobItem
                        {
                            Name = blobItem.Name,
                            ContainerName = containerName,
                            Size = blobItem.Properties.ContentLength,
                            ContentType = blobItem.Properties.ContentType,
                            LastModified = blobItem.Properties.LastModified?.DateTime,
                            ETag = blobItem.Properties.ETag?.ToString(),
                            IsDeleted = blobItem.Deleted,
                        }
                    );
                }

                result.ContinuationToken = page.ContinuationToken;
                result.HasMore = !string.IsNullOrEmpty(page.ContinuationToken);
                break; // Only process first page
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error listing blobs paged in container {ContainerName}",
                containerName
            );
            return new BlobListResult();
        }
    }

    #endregion

    #region SAS Token Generation

    public async Task<string> GenerateSasTokenAsync(
        string containerName,
        string blobName,
        BlobSasPermissions permissions,
        DateTimeOffset expiresOn
    )
    {
        try
        {
            if (_useManagedIdentity)
            {
                // When using Managed Identity, we need to use User Delegation SAS
                return await GenerateUserDelegationSasAsync(containerName, blobName, permissions, expiresOn);
            }
            else
            {
                // Use account key based SAS
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                if (!blobClient.CanGenerateSasUri)
                {
                    throw new InvalidOperationException(
                        "Cannot generate SAS token. Check if StorageSharedKeyCredential is used."
                    );
                }

                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerName,
                    BlobName = blobName,
                    Resource = "b",
                    ExpiresOn = expiresOn,
                };

                // Set permissions
                if (permissions.HasFlag(BlobSasPermissions.Read))
                    sasBuilder.SetPermissions(Azure.Storage.Sas.BlobContainerSasPermissions.Read);
                if (permissions.HasFlag(BlobSasPermissions.Write))
                    sasBuilder.SetPermissions(Azure.Storage.Sas.BlobContainerSasPermissions.Write);
                if (permissions.HasFlag(BlobSasPermissions.Delete))
                    sasBuilder.SetPermissions(Azure.Storage.Sas.BlobContainerSasPermissions.Delete);

                return blobClient.GenerateSasUri(sasBuilder).Query;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error generating SAS token for blob {BlobName} in container {ContainerName}",
                blobName,
                containerName
            );
            throw;
        }
    }
    
    private async Task<string> GenerateUserDelegationSasAsync(
        string containerName,
        string blobName,
        BlobSasPermissions permissions,
        DateTimeOffset expiresOn
    )
    {
        // Get a user delegation key
        var startsOn = DateTimeOffset.UtcNow.AddMinutes(-5); // Account for clock skew
        var userDelegationKey = await _blobServiceClient.GetUserDelegationKeyAsync(startsOn, expiresOn);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            StartsOn = startsOn,
            ExpiresOn = expiresOn,
        };

        // Set permissions
        if (permissions.HasFlag(BlobSasPermissions.Read))
            sasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Read);
        if (permissions.HasFlag(BlobSasPermissions.Write))
            sasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Write);
        if (permissions.HasFlag(BlobSasPermissions.Delete))
            sasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Delete);

        // Create the SAS token
        var blobUriBuilder = new BlobUriBuilder(_blobServiceClient.Uri)
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Sas = sasBuilder.ToSasQueryParameters(userDelegationKey.Value, _storageAccountName)
        };

        return blobUriBuilder.ToUri().Query;
    }

    public async Task<string> GenerateContainerSasTokenAsync(
        string containerName,
        BlobContainerSasPermissions permissions,
        DateTimeOffset expiresOn
    )
    {
        try
        {
            if (_useManagedIdentity)
            {
                // When using Managed Identity, we need to use User Delegation SAS
                return await GenerateUserDelegationContainerSasAsync(containerName, permissions, expiresOn);
            }
            else
            {
                // Use account key based SAS
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                if (!containerClient.CanGenerateSasUri)
                {
                    throw new InvalidOperationException(
                        "Cannot generate SAS token. Check if StorageSharedKeyCredential is used."
                    );
                }

                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerName,
                    Resource = "c",
                    ExpiresOn = expiresOn,
                };

                // Set permissions
                if (permissions.HasFlag(BlobContainerSasPermissions.Read))
                    sasBuilder.SetPermissions(Azure.Storage.Sas.BlobContainerSasPermissions.Read);
                if (permissions.HasFlag(BlobContainerSasPermissions.Write))
                    sasBuilder.SetPermissions(Azure.Storage.Sas.BlobContainerSasPermissions.Write);
                if (permissions.HasFlag(BlobContainerSasPermissions.Delete))
                    sasBuilder.SetPermissions(Azure.Storage.Sas.BlobContainerSasPermissions.Delete);
                if (permissions.HasFlag(BlobContainerSasPermissions.List))
                    sasBuilder.SetPermissions(Azure.Storage.Sas.BlobContainerSasPermissions.List);

                return containerClient.GenerateSasUri(sasBuilder).Query;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error generating container SAS token for {ContainerName}",
                containerName
            );
            throw;
        }
    }
    
    private async Task<string> GenerateUserDelegationContainerSasAsync(
        string containerName,
        BlobContainerSasPermissions permissions,
        DateTimeOffset expiresOn
    )
    {
        // Get a user delegation key
        var startsOn = DateTimeOffset.UtcNow.AddMinutes(-5); // Account for clock skew
        var userDelegationKey = await _blobServiceClient.GetUserDelegationKeyAsync(startsOn, expiresOn);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            Resource = "c",
            StartsOn = startsOn,
            ExpiresOn = expiresOn,
        };

        // Set permissions
        if (permissions.HasFlag(BlobContainerSasPermissions.Read))
            sasBuilder.SetPermissions(Azure.Storage.Sas.BlobContainerSasPermissions.Read);
        if (permissions.HasFlag(BlobContainerSasPermissions.Write))
            sasBuilder.SetPermissions(Azure.Storage.Sas.BlobContainerSasPermissions.Write);
        if (permissions.HasFlag(BlobContainerSasPermissions.Delete))
            sasBuilder.SetPermissions(Azure.Storage.Sas.BlobContainerSasPermissions.Delete);
        if (permissions.HasFlag(BlobContainerSasPermissions.List))
            sasBuilder.SetPermissions(Azure.Storage.Sas.BlobContainerSasPermissions.List);

        // Create the SAS token
        var blobUriBuilder = new BlobUriBuilder(_blobServiceClient.Uri)
        {
            BlobContainerName = containerName,
            Sas = sasBuilder.ToSasQueryParameters(userDelegationKey.Value, _storageAccountName)
        };

        return blobUriBuilder.ToUri().Query;
    }

    #endregion

    #region URL Generation

    public string GetBlobUrl(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        return blobClient.Uri.ToString();
    }

    public string GetBlobUrlWithSasToken(string containerName, string blobName, string sasToken)
    {
        var baseUrl = GetBlobUrl(containerName, blobName);
        return $"{baseUrl}{sasToken}";
    }

    #endregion

    #region Properties

    public async Task<BlobProperties?> GetPropertiesAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var properties = await blobClient.GetPropertiesAsync();

            return new BlobProperties
            {
                ContentLength = properties.Value.ContentLength,
                ContentType = properties.Value.ContentType,
                ETag = properties.Value.ETag.ToString(),
                LastModified = properties.Value.LastModified.DateTime,
                CreatedOn = properties.Value.CreatedOn.DateTime,
                AccessTier = properties.Value.AccessTier switch
                {
                    "Hot" => BlobAccessTier.Hot,
                    "Cool" => BlobAccessTier.Cool,
                    "Archive" => BlobAccessTier.Archive,
                    _ => null,
                },
                IsServerEncrypted = properties.Value.IsServerEncrypted,
                Metadata = (Dictionary<string, string>)properties.Value.Metadata,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting properties for blob {BlobName} in container {ContainerName}",
                blobName,
                containerName
            );
            return null;
        }
    }

    public async Task<bool> SetContentTypeAsync(
        string containerName,
        string blobName,
        string contentType
    )
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var headers = new BlobHttpHeaders { ContentType = contentType };
            await blobClient.SetHttpHeadersAsync(headers);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error setting content type for blob {BlobName} in container {ContainerName}",
                blobName,
                containerName
            );
            return false;
        }
    }

    #endregion

    #region Advanced Operations

    public async Task<string> CreateSnapshotAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var snapshot = await blobClient.CreateSnapshotAsync();
            return snapshot.Value.Snapshot;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating snapshot for blob {BlobName} in container {ContainerName}",
                blobName,
                containerName
            );
            throw;
        }
    }

    public async Task<bool> RestoreFromSnapshotAsync(
        string containerName,
        string blobName,
        string snapshotId
    )
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName).WithSnapshot(snapshotId);
            var targetBlobClient = containerClient.GetBlobClient(blobName);

            var operation = await targetBlobClient.StartCopyFromUriAsync(blobClient.Uri);
            await operation.WaitForCompletionAsync();

            return operation.HasCompleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error restoring blob {BlobName} from snapshot {SnapshotId}",
                blobName,
                snapshotId
            );
            return false;
        }
    }

    public async Task<bool> SetAccessTierAsync(
        string containerName,
        string blobName,
        BlobAccessTier accessTier
    )
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var azureAccessTier = accessTier switch
            {
                BlobAccessTier.Hot => AccessTier.Hot,
                BlobAccessTier.Cool => AccessTier.Cool,
                BlobAccessTier.Archive => AccessTier.Archive,
                _ => AccessTier.Hot,
            };

            await blobClient.SetAccessTierAsync(azureAccessTier);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error setting access tier for blob {BlobName} in container {ContainerName}",
                blobName,
                containerName
            );
            return false;
        }
    }

    #endregion
}
