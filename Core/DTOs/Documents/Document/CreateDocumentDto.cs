using Core.Enums.Documents;

namespace Core.DTOs.Documents.Document;

public class CreateDocumentDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DocumentType Type { get; set; }
    public DocumentCategory Category { get; set; }
    
    // Project and location
    public Guid ProjectId { get; set; }
    public Guid? DisciplineId { get; set; }
    public Guid? PhaseId { get; set; }
    public Guid? PackageId { get; set; }
    
    // Document identifiers
    public string? DocumentNumber { get; set; }
    public string? ClientDocumentNumber { get; set; }
    public string? ContractorDocumentNumber { get; set; }
    
    // Initial version info
    public string InitialVersion { get; set; } = "A";
    public string? IssuePurpose { get; set; }
    public DateTime? IssueDate { get; set; }
    
    // Metadata
    public string? Author { get; set; }
    public string? Originator { get; set; }
    public DocumentConfidentiality Confidentiality { get; set; } = DocumentConfidentiality.Internal;
    
    // Review requirements
    public bool RequiresReview { get; set; }
    public DateTime? ReviewDueDate { get; set; }
    public List<Guid> ReviewerIds { get; set; } = new();
    
    // Access control
    public bool IsPublic { get; set; }
    public List<Guid> AllowedUserIds { get; set; } = new();
    public List<Guid> AllowedRoleIds { get; set; } = new();
    
    // Tags and keywords
    public List<string> Tags { get; set; } = new();
    public string? Keywords { get; set; }
    
    // Related documents
    public List<Guid> RelatedDocumentIds { get; set; } = new();
    public List<Guid> SupersededDocumentIds { get; set; } = new();
    
    // File upload (handled separately)
    public bool HasFile { get; set; }
    public string? TemporaryFileId { get; set; }
}