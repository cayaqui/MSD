using Core.Enums.Documents;

namespace Core.DTOs.Documents.DocumentComment;

public class CreateDocumentCommentDto
{
    public Guid DocumentId { get; set; }
    public Guid? DocumentVersionId { get; set; }
    public Guid? ParentCommentId { get; set; }
    
    public string Comment { get; set; } = string.Empty;
    public CommentType Type { get; set; } = CommentType.General;
    
    // For review comments
    public string? Section { get; set; }
    public int? PageNumber { get; set; }
    public string? Reference { get; set; }
    public ReviewCommentSeverity? Severity { get; set; }
    
    // Attachments
    public List<string> AttachmentIds { get; set; } = new();
}
