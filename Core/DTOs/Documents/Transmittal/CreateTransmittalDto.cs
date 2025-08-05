using Core.DTOs.Documents.TransmittalDocument;
using Core.Enums.Documents;

namespace Core.DTOs.Documents.Transmittal;

public class CreateTransmittalDto
{
    public Guid ProjectId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TransmittalPriority Priority { get; set; } = TransmittalPriority.Normal;
    
    // From/To information
    public Guid FromCompanyId { get; set; }
    public string? FromContact { get; set; }
    public string? FromEmail { get; set; }
    public string? FromPhone { get; set; }
    
    public Guid ToCompanyId { get; set; }
    public string? ToContact { get; set; }
    public string? ToEmail { get; set; }
    public string? ToPhone { get; set; }
    
    // Transmittal details
    public string? Purpose { get; set; }
    public string? ResponseRequired { get; set; }
    public DateTime? ResponseDueDate { get; set; }
    
    // Delivery
    public DeliveryMethod DeliveryMethod { get; set; }
    public string? TrackingNumber { get; set; }
    
    // Documents to include
    public List<Guid> DocumentIds { get; set; } = new();
    public List<TransmittalDocumentItemDto> DocumentItems { get; set; } = new();
    
    // Recipients
    public List<CreateTransmittalRecipientDto> Recipients { get; set; } = new();
    
    // Options
    public bool RequiresApproval { get; set; }
    public bool SendNotification { get; set; }
}

