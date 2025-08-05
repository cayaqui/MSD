namespace Domain.Entities.Documents.Core;

public class DocumentComment : BaseAuditableEntity, ISoftDelete
{
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
    
    public Guid? DocumentVersionId { get; set; }
    public virtual DocumentVersion? DocumentVersion { get; set; }
    
    public Guid? ParentCommentId { get; set; }
    public virtual DocumentComment? ParentComment { get; set; }
    
    public string Comment { get; set; } = string.Empty;
    public CommentType Type { get; set; }
    public CommentStatus Status { get; set; }
    
    // Author
    public Guid AuthorId { get; set; }
    public virtual User Author { get; set; } = null!;
    public DateTime CommentDate { get; set; }
    
    // For review comments
    public string? Section { get; set; }
    public int? PageNumber { get; set; }
    public string? Reference { get; set; }
    public ReviewCommentSeverity? Severity { get; set; }
    
    // Resolution
    public bool IsResolved { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public Guid? ResolvedById { get; set; }
    public virtual User? ResolvedBy { get; set; }
    public string? Resolution { get; set; }
    
    
    // Navigation properties
    public virtual ICollection<DocumentComment> Replies { get; set; } = new List<DocumentComment>();
    public virtual ICollection<CommentAttachment> Attachments { get; set; } = new List<CommentAttachment>();
    
    // Helper methods
    public void Resolve(Guid resolvedById, string? resolution = null)
    {
        IsResolved = true;
        ResolvedDate = DateTime.UtcNow;
        ResolvedById = resolvedById;
        Resolution = resolution;
        Status = CommentStatus.Resolved;
    }
    
    public void UpdateStatus(CommentStatus newStatus)
    {
        Status = newStatus;
        if (newStatus == CommentStatus.Resolved || newStatus == CommentStatus.Closed)
        {
            IsResolved = true;
            if (!ResolvedDate.HasValue)
                ResolvedDate = DateTime.UtcNow;
        }
    }
}
