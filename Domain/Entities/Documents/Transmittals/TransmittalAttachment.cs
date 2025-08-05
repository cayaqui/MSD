using Domain.Common;

namespace Domain.Entities.Documents.Transmittals;

public class TransmittalAttachment : BaseEntity
{
    public Guid TransmittalId { get; set; }
    public virtual Transmittal Transmittal { get; set; } = null!;
    
    // File information
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Azure Blob Storage
    public string BlobContainerName { get; set; } = string.Empty;
    public string BlobName { get; set; } = string.Empty;
    public string? BlobStorageUrl { get; set; }
    public DateTime UploadedDate { get; set; }
    public Guid UploadedById { get; set; }
}