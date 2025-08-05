namespace Core.DTOs.Documents.TransmittalDocument;

public class TransmittalDocumentDto
{
    public Guid Id { get; set; }
    public Guid TransmittalId { get; set; }
    public Guid DocumentId { get; set; }
    public Guid DocumentVersionId { get; set; }
    
    // Document info
    public string DocumentNumber { get; set; } = string.Empty;
    public string DocumentTitle { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    
    // Transmittal specific
    public int Copies { get; set; } = 1;
    public string? Format { get; set; }
    public string? Purpose { get; set; }
    public string? Comments { get; set; }
    public int SortOrder { get; set; }
}
