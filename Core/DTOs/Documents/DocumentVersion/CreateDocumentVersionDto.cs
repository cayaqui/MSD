namespace Core.DTOs.Documents.DocumentVersion;

public class CreateDocumentVersionDto
{
    public Guid DocumentId { get; set; }
    public string Version { get; set; } = string.Empty;
    public string? IssuePurpose { get; set; }
    public string? Comments { get; set; }
    public DateTime? IssueDate { get; set; }
    
    // File upload info
    public string TemporaryFileId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
}