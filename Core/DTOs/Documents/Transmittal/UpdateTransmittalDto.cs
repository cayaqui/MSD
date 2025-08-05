using Core.Enums.Documents;

namespace Core.DTOs.Documents.Transmittal;

public class UpdateTransmittalDto
{
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TransmittalPriority Priority { get; set; }
    public TransmittalStatus Status { get; set; }
    
    // From/To updates
    public string? FromContact { get; set; }
    public string? FromEmail { get; set; }
    public string? FromPhone { get; set; }
    public string? ToContact { get; set; }
    public string? ToEmail { get; set; }
    public string? ToPhone { get; set; }
    
    // Details
    public string? Purpose { get; set; }
    public string? ResponseRequired { get; set; }
    public DateTime? ResponseDueDate { get; set; }
    
    // Delivery
    public DeliveryMethod DeliveryMethod { get; set; }
    public string? TrackingNumber { get; set; }
}
