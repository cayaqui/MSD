using Domain.Common;
using Core.Enums.Documents;

namespace Domain.Entities.Documents.Core;

public class Document : BaseAuditableEntity, ISoftDelete
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
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
    public int CurrentRevisionNumber { get; set; } = 0;
    public Guid? CurrentVersionId { get; set; }
    public virtual DocumentVersion? CurrentVersionEntity { get; set; }
    
    // Metadata
    public string? Author { get; set; }
    public Guid? AuthorId { get; set; }
    public virtual User? AuthorUser { get; set; }
    public string? Originator { get; set; }
    public string? ClientDocumentNumber { get; set; }
    public string? ContractorDocumentNumber { get; set; }
    public DocumentConfidentiality Confidentiality { get; set; } = DocumentConfidentiality.Internal;
    
    // Review and approval
    public ReviewStatus ReviewStatus { get; set; } = ReviewStatus.NotRequired;
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
    
    // Tags and keywords (stored as JSON)
    public string TagsJson { get; set; } = "[]";
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
    public virtual ICollection<DocumentDistribution> Distributions { get; set; } = new List<DocumentDistribution>();
    public virtual ICollection<DocumentComment> Comments { get; set; } = new List<DocumentComment>();
    public virtual ICollection<DocumentRelationship> RelatedDocuments { get; set; } = new List<DocumentRelationship>();
    public virtual ICollection<DocumentRelationship> RelatedToDocuments { get; set; } = new List<DocumentRelationship>();
    public virtual ICollection<TransmittalDocument> TransmittalDocuments { get; set; } = new List<TransmittalDocument>();
    public virtual ICollection<DocumentPermission> Permissions { get; set; } = new List<DocumentPermission>();
    
    // Helper methods
    public void IncrementDownloadCount()
    {
        DownloadCount++;
        LastDownloadDate = DateTime.UtcNow;
    }
    
    public void IncrementViewCount()
    {
        ViewCount++;
        LastViewDate = DateTime.UtcNow;
    }
    
    public bool CanBeEdited()
    {
        return !IsLocked && Status != DocumentStatus.Obsolete && Status != DocumentStatus.Cancelled;
    }
    
    public bool CanBeDeleted()
    {
        return !IsLocked && Status != DocumentStatus.Issued;
    }
}
