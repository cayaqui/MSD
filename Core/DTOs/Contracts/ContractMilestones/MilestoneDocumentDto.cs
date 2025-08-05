namespace Core.DTOs.Contracts.ContractMilestones;

public class MilestoneDocumentDto
{
    public Guid Id { get; set; }
    public Guid MilestoneId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string DocumentType { get; set; } = string.Empty; // CompletionCertificate, ProgressReport, etc.
    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public string? StoragePath { get; set; }
    public bool IsApprovalDocument { get; set; }
}