namespace Domain.Entities.Documents.Core;

public class CommentAttachment : BaseEntity
{
    public Guid CommentId { get; set; }
    public virtual DocumentComment Comment { get; set; } = null!;
    
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    
    // Azure Blob Storage
    public string BlobContainerName { get; set; } = string.Empty;
    public string BlobName { get; set; } = string.Empty;
    public string? BlobStorageUrl { get; set; }
    public DateTime UploadedDate { get; set; }
}
