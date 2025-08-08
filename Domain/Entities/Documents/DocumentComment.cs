using Domain.Common;
using Domain.Entities.Auth.Security;

namespace Domain.Entities.Documents;

/// <summary>
/// Representa un comentario en un documento
/// </summary>
public class DocumentComment : BaseAuditableEntity
{
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
    
    public Guid? DocumentVersionId { get; set; }
    public virtual DocumentVersion? DocumentVersion { get; set; }
    
    public string Comment { get; set; } = string.Empty;
    public CommentType Type { get; set; }
    public CommentStatus Status { get; set; }
    
    // Author
    public Guid AuthorId { get; set; }
    public virtual User Author { get; set; } = null!;
    
    // Parent comment for replies
    public Guid? ParentCommentId { get; set; }
    public virtual DocumentComment? ParentComment { get; set; }
    
    // Resolution
    public bool IsResolved { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public Guid? ResolvedById { get; set; }
    public virtual User? ResolvedBy { get; set; }
    public string? Resolution { get; set; }
    
    // Location in document (for drawings/PDFs)
    public string? PageNumber { get; set; }
    public string? LocationX { get; set; }
    public string? LocationY { get; set; }
    public string? LocationDescription { get; set; }
    
    // Navigation properties
    public virtual ICollection<DocumentComment> Replies { get; set; } = new List<DocumentComment>();
}

/// <summary>
/// Tipo de comentario
/// </summary>
public enum CommentType
{
    General,
    Technical,
    Editorial,
    Question,
    Suggestion,
    Issue,
    Clarification
}

/// <summary>
/// Estado del comentario
/// </summary>
public enum CommentStatus
{
    Open,
    InProgress,
    Resolved,
    Closed,
    Deferred
}