using Core.DTOs.Common;
using Core.Enums.Documents;

namespace Core.DTOs.Documents.DocumentVersion;

public class DocumentVersionDto : BaseDto
{
    public Guid DocumentId { get; set; }
    public string Version { get; set; } = string.Empty;
    public int RevisionNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public string? IssuePurpose { get; set; }
    public string? Comments { get; set; }
    
    // File information
    public string FileName { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string BlobStorageUrl { get; set; } = string.Empty;
    public string BlobName { get; set; } = string.Empty;
    
    // Version metadata
    public Guid UploadedById { get; set; }
    public string UploadedByName { get; set; } = string.Empty;
    public DateTime UploadedDate { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsSuperseded { get; set; }
    public Guid? SupersededById { get; set; }
    
    // Review status
    public ReviewStatus ReviewStatus { get; set; }
    public DateTime? ReviewDate { get; set; }
    public Guid? ReviewedById { get; set; }
    public string? ReviewedByName { get; set; }
    public string? ReviewComments { get; set; }
    
    // Statistics
    public int DownloadCount { get; set; }
    public DateTime? LastDownloadDate { get; set; }
}
