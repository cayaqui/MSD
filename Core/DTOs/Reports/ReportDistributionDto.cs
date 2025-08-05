namespace Core.DTOs.Reports;

public class ReportDistributionDto
{
    public Guid Id { get; set; }
    public Guid ReportId { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public DateTime DistributedAt { get; set; }
    public string DistributionMethod { get; set; } = string.Empty; // Email, Portal, SharePoint
    public bool IsDelivered { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string DistributedBy { get; set; } = string.Empty;
}