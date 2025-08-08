using Domain.Common;
using Domain.Entities.Auth.Security;
using Domain.Entities.Organization;

namespace Domain.Entities.Documents;

/// <summary>
/// Representa un documento del proyecto
/// </summary>
public class Document : BaseAuditableEntity
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DocumentType Type { get; set; }
    public DocumentCategory Category { get; set; }
    public DocumentStatus Status { get; set; }
    
    // Project and location
    public Guid ProjectId { get; set; }
    public virtual Project Project { get; set; } = null!;
    public Guid? DisciplineId { get; set; }
    public virtual Discipline? Discipline { get; set; }
    public Guid? PhaseId { get; set; }
    public virtual Phase? Phase { get; set; }
    public Guid? PackageId { get; set; }
    public virtual Package? Package { get; set; }
    
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
    public virtual User? AuthorUser { get; set; }
    public string? Originator { get; set; }
    public string? ClientDocumentNumber { get; set; }
    public string? ContractorDocumentNumber { get; set; }
    public DocumentConfidentiality Confidentiality { get; set; }
    
    // Review and approval
    public ReviewStatus ReviewStatus { get; set; }
    public DateTime? ReviewDueDate { get; set; }
    public DateTime? ReviewCompletedDate { get; set; }
    public Guid? ReviewedById { get; set; }
    public virtual User? ReviewedBy { get; set; }
    public string? ReviewComments { get; set; }
    
    // Access control
    public bool IsPublic { get; set; }
    public bool IsLocked { get; set; }
    public Guid? LockedById { get; set; }
    public virtual User? LockedBy { get; set; }
    public DateTime? LockedDate { get; set; }
    
    // Superseded documents
    public Guid? SupersededById { get; set; }
    public virtual Document? SupersededBy { get; set; }
    public virtual ICollection<Document> SupersededDocuments { get; set; } = new List<Document>();
    
    // Tags and keywords
    public string? Tags { get; set; } // Stored as comma-separated values
    public string? Keywords { get; set; }
    
    // Retention
    public DateTime? RetentionDate { get; set; }
    public string? RetentionPolicy { get; set; }
    
    // Statistics
    public int DownloadCount { get; set; }
    public DateTime? LastDownloadDate { get; set; }
    public int ViewCount { get; set; }
    public DateTime? LastViewDate { get; set; }
    
    // Navigation properties
    public virtual ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
    public virtual ICollection<DocumentComment> Comments { get; set; } = new List<DocumentComment>();
    public virtual ICollection<DocumentDistribution> Distributions { get; set; } = new List<DocumentDistribution>();
    public virtual ICollection<DocumentPermission> Permissions { get; set; } = new List<DocumentPermission>();
    public virtual ICollection<DocumentRelationship> RelatedDocuments { get; set; } = new List<DocumentRelationship>();
    public virtual ICollection<DocumentRelationship> RelatedToDocuments { get; set; } = new List<DocumentRelationship>();
    public virtual ICollection<DocumentView> Views { get; set; } = new List<DocumentView>();
    public virtual ICollection<DocumentDownload> Downloads { get; set; } = new List<DocumentDownload>();
}

/// <summary>
/// Tipo de documento
/// </summary>
public enum DocumentType
{
    Drawing,
    Specification,
    Report,
    Procedure,
    Form,
    Letter,
    Minutes,
    Calculation,
    Schedule,
    Model,
    Photo,
    Other
}

/// <summary>
/// Categoría del documento
/// </summary>
public enum DocumentCategory
{
    Engineering,
    Procurement,
    Construction,
    Commissioning,
    Quality,
    Safety,
    Environmental,
    Contract,
    Commercial,
    Administrative,
    Other
}

/// <summary>
/// Estado del documento
/// </summary>
public enum DocumentStatus
{
    Draft,
    InReview,
    Approved,
    Issued,
    Superseded,
    Obsolete,
    OnHold,
    Cancelled
}

/// <summary>
/// Nivel de confidencialidad
/// </summary>
public enum DocumentConfidentiality
{
    Public,
    Internal,
    Confidential,
    Restricted,
    Secret
}

/// <summary>
/// Estado de revisión
/// </summary>
public enum ReviewStatus
{
    NotRequired,
    Pending,
    InProgress,
    Approved,
    ApprovedWithComments,
    Rejected,
    OnHold
}