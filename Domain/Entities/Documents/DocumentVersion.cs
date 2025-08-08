using Domain.Common;
using Domain.Entities.Auth.Security;

namespace Domain.Entities.Documents;

/// <summary>
/// Representa una versión de un documento
/// </summary>
public class DocumentVersion : BaseAuditableEntity
{
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
    
    public string Version { get; set; } = string.Empty;
    public int RevisionNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public string? IssuePurpose { get; set; }
    
    // File information
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string BlobStorageUrl { get; set; } = string.Empty;
    public string? BlobContainerName { get; set; }
    public string? BlobName { get; set; }
    
    // Version metadata
    public string? Comments { get; set; }
    public string? ChangeDescription { get; set; }
    public bool IsCurrent { get; set; }
    
    // Review status
    public ReviewStatus ReviewStatus { get; set; }
    public DateTime? ReviewedDate { get; set; }
    public Guid? ReviewedById { get; set; }
    public virtual User? ReviewedBy { get; set; }
    public string? ReviewComments { get; set; }
    
    // Statistics
    public int DownloadCount { get; set; }
    public DateTime? LastDownloadDate { get; set; }
}