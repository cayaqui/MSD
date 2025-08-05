using Core.Enums.Documents;

namespace Core.DTOs.Documents.DocumentComment;

public class UpdateDocumentCommentDto
{
    public string Comment { get; set; } = string.Empty;
    public CommentStatus Status { get; set; }
    public ReviewCommentSeverity? Severity { get; set; }
    
    // Resolution
    public bool IsResolved { get; set; }
    public string? Resolution { get; set; }
}
