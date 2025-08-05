using Core.DTOs.Common;
using Core.DTOs.Documents.DocumentComment;
using Core.DTOs.Documents.DocumentDistribution;
using Core.DTOs.Documents.DocumentVersion;
using Core.Enums.Documents;

namespace Core.DTOs.Documents.Document;

public class DocumentDto : BaseDto
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DocumentType Type { get; set; }
    public DocumentCategory Category { get; set; }
    public DocumentStatus Status { get; set; }
    
    // Project and location
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public Guid? DisciplineId { get; set; }
    public string? DisciplineName { get; set; }
    public Guid? PhaseId { get; set; }
    public string? PhaseName { get; set; }
    public Guid? PackageId { get; set; }
    public string? PackageName { get; set; }
    
    // Version control
    public string CurrentVersion { get; set; } = "A";
    public int RevisionNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public string? IssuePurpose { get; set; }
    
    // File information
    public string? FileName { get; set; }
    public string? FileExtension { get; set; }
    public long? FileSize { get; set; }
    public string? ContentType { get; set; }
    public string? BlobStorageUrl { get; set; }
    public string? BlobContainerName { get; set; }
    public string? BlobName { get; set; }
    
    // Metadata
    public string? Author { get; set; }
    public Guid? AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public string? Originator { get; set; }
    public string? ClientDocumentNumber { get; set; }
    public string? ContractorDocumentNumber { get; set; }
    public DocumentConfidentiality Confidentiality { get; set; }
    
    // Review and approval
    public ReviewStatus ReviewStatus { get; set; }
    public DateTime? ReviewDueDate { get; set; }
    public DateTime? ReviewCompletedDate { get; set; }
    public Guid? ReviewedById { get; set; }
    public string? ReviewedByName { get; set; }
    public string? ReviewComments { get; set; }
    
    // Access control
    public bool IsPublic { get; set; }
    public bool IsLocked { get; set; }
    public Guid? LockedById { get; set; }
    public string? LockedByName { get; set; }
    public DateTime? LockedDate { get; set; }
    
    // Related documents
    public List<Guid> RelatedDocumentIds { get; set; } = new();
    public List<Guid> SupersededDocumentIds { get; set; } = new();
    public Guid? SupersededById { get; set; }
    
    // Tags and keywords
    public List<string> Tags { get; set; } = new();
    public string? Keywords { get; set; }
    
    // Retention
    public DateTime? RetentionDate { get; set; }
    public string? RetentionPolicy { get; set; }
    
    // Statistics
    public int DownloadCount { get; set; }
    public DateTime? LastDownloadDate { get; set; }
    public int ViewCount { get; set; }
    public DateTime? LastViewDate { get; set; }
    
    // Collections
    public List<DocumentVersionDto> Versions { get; set; } = new();
    public List<DocumentDistributionDto> Distributions { get; set; } = new();
    public List<DocumentCommentDto> Comments { get; set; } = new();
}

