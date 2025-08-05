

namespace Domain.Entities.Documents.Core;

public class DocumentVersion : BaseAuditableEntity
{
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
    
    public string Version { get; set; } = string.Empty;
    public int RevisionNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public string? IssuePurpose { get; set; }
    public string? VersionComments { get; set; }
    
    // File information - stored in Azure Blob Storage
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string? FileHash { get; set; } // SHA256 hash for integrity
    
    // Azure Blob Storage information
    public string BlobContainerName { get; set; } = string.Empty;
    public string BlobName { get; set; } = string.Empty;
    public string? BlobStorageUrl { get; set; }
    public DateTime? BlobUploadedDate { get; set; }
    
    // Version metadata
    public Guid UploadedById { get; set; }
    public virtual User UploadedBy { get; set; } = null!;
    public DateTime UploadedDate { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsSuperseded { get; set; }
    public Guid? SupersededById { get; set; }
    public virtual DocumentVersion? SupersededBy { get; set; }
    
    // Review status
    public ReviewStatus ReviewStatus { get; set; } = ReviewStatus.NotRequired;
    public DateTime? ReviewDate { get; set; }
    public Guid? ReviewedById { get; set; }
    public virtual User? ReviewedBy { get; set; }
    public string? ReviewComments { get; set; }
    
    // Statistics
    public int DownloadCount { get; set; }
    public DateTime? LastDownloadDate { get; set; }
    
    // Navigation properties
    public virtual ICollection<DocumentDistribution> Distributions { get; set; } = new List<DocumentDistribution>();
    public virtual ICollection<DocumentComment> Comments { get; set; } = new List<DocumentComment>();
    public virtual ICollection<TransmittalDocument> TransmittalDocuments { get; set; } = new List<TransmittalDocument>();
    
    // Helper methods
    public void IncrementDownloadCount()
    {
        DownloadCount++;
        LastDownloadDate = DateTime.UtcNow;
    }
    
    public string GetVersionString()
    {
        return RevisionNumber > 0 ? $"{Version}.{RevisionNumber}" : Version;
    }
}