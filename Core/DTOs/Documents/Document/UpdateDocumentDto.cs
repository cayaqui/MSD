using Core.Enums.Documents;

namespace Core.DTOs.Documents.Document;

public class UpdateDocumentDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DocumentType Type { get; set; }
    public DocumentCategory Category { get; set; }
    public DocumentStatus Status { get; set; }
    
    // Location updates
    public Guid? DisciplineId { get; set; }
    public Guid? PhaseId { get; set; }
    public Guid? PackageId { get; set; }
    
    // Document identifiers
    public string? ClientDocumentNumber { get; set; }
    public string? ContractorDocumentNumber { get; set; }
    
    // Metadata
    public string? Author { get; set; }
    public string? Originator { get; set; }
    public DocumentConfidentiality Confidentiality { get; set; }
    
    // Review requirements
    public bool RequiresReview { get; set; }
    public DateTime? ReviewDueDate { get; set; }
    
    // Tags and keywords
    public List<string> Tags { get; set; } = new();
    public string? Keywords { get; set; }
    
    // Related documents
    public List<Guid> RelatedDocumentIds { get; set; } = new();
    
    // Retention
    public DateTime? RetentionDate { get; set; }
    public string? RetentionPolicy { get; set; }
}