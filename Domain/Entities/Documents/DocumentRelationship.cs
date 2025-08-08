using Domain.Common;

namespace Domain.Entities.Documents;

/// <summary>
/// Representa la relación entre documentos
/// </summary>
public class DocumentRelationship : BaseAuditableEntity
{
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
    
    public Guid RelatedDocumentId { get; set; }
    public virtual Document RelatedDocument { get; set; } = null!;
    
    public DocumentRelationshipType RelationshipType { get; set; }
    public string? Description { get; set; }
}

/// <summary>
/// Tipo de relación entre documentos
/// </summary>
public enum DocumentRelationshipType
{
    Reference,
    Attachment,
    Parent,
    Child,
    Related,
    Supersedes,
    SupersededBy,
    Supports,
    ConflictsWith,
    Updates,
    UpdatedBy
}