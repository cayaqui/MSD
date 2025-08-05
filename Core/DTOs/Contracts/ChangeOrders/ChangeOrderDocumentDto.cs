namespace Core.DTOs.Contracts.ChangeOrders;

public class ChangeOrderDocumentDto
{
    public Guid Id { get; set; }
    public Guid ChangeOrderId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string DocumentType { get; set; } = string.Empty; // Supporting, CostBreakdown, ScheduleAnalysis, etc.
    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public string? StoragePath { get; set; }
    public bool IsPublic { get; set; }
}