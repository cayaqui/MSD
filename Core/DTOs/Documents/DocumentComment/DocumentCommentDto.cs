using Core.DTOs.Common;
using Core.Enums.Documents;

namespace Core.DTOs.Documents.DocumentComment;

public class DocumentCommentDto : BaseDto
{
    public Guid DocumentId { get; set; }
    public Guid? DocumentVersionId { get; set; }
    public Guid? ParentCommentId { get; set; }
    
    public string Comment { get; set; } = string.Empty;
    public CommentType Type { get; set; }
    public CommentStatus Status { get; set; }
    
    // Author
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorEmail { get; set; }
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
    public string? ResolvedByName { get; set; }
    public string? Resolution { get; set; }
    
    // Replies
    public List<DocumentCommentDto> Replies { get; set; } = new();
    public int ReplyCount { get; set; }
    
    // Attachments
    public List<CommentAttachmentDto> Attachments { get; set; } = new();
}
