namespace Core.DTOs.Documents.TransmittalDocument;

public class TransmittalDocumentItemDto
{
    public Guid DocumentId { get; set; }
    public Guid? DocumentVersionId { get; set; } // If null, use current version
    public int Copies { get; set; } = 1;
    public string? Format { get; set; }
    public string? Purpose { get; set; }
    public string? Comments { get; set; }
}
