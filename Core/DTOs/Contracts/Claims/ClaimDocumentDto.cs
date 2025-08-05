namespace Core.DTOs.Contracts.Claims;

public class ClaimDocumentDto
{
    public Guid Id { get; set; }
    public Guid ClaimId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string DocumentType { get; set; } = string.Empty; // Notice, Evidence, Correspondence, etc.
    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public string? StoragePath { get; set; }
    public bool IsConfidential { get; set; }
}