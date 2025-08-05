using Core.DTOs.Common;
using Core.DTOs.Documents.TransmittalAttachment;
using Core.DTOs.Documents.TransmittalDocument;
using Core.DTOs.Documents.TransmittalRecipient;
using Core.Enums.Documents;

namespace Core.DTOs.Documents.Transmittal;

public class TransmittalDto : BaseDto
{
    public string TransmittalNumber { get; set; } = string.Empty;
    public DateTime TransmittalDate { get; set; }
    public TransmittalStatus Status { get; set; }
    public TransmittalPriority Priority { get; set; }
    
    // Project information
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    
    // From/To information
    public Guid FromCompanyId { get; set; }
    public string FromCompanyName { get; set; } = string.Empty;
    public string? FromContact { get; set; }
    public string? FromEmail { get; set; }
    public string? FromPhone { get; set; }
    
    public Guid ToCompanyId { get; set; }
    public string ToCompanyName { get; set; } = string.Empty;
    public string? ToContact { get; set; }
    public string? ToEmail { get; set; }
    public string? ToPhone { get; set; }
    
    // Transmittal details
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Purpose { get; set; }
    public string? ResponseRequired { get; set; }
    public DateTime? ResponseDueDate { get; set; }
    
    // Delivery information
    public DeliveryMethod DeliveryMethod { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public bool IsDelivered { get; set; }
    public DateTime? DeliveryConfirmedDate { get; set; }
    public string? DeliveryConfirmedBy { get; set; }
    
    // Prepared by
    public Guid PreparedById { get; set; }
    public string PreparedByName { get; set; } = string.Empty;
    public DateTime PreparedDate { get; set; }
    
    // Approval
    public bool RequiresApproval { get; set; }
    public Guid? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovalComments { get; set; }
    
    // Documents
    public int DocumentCount { get; set; }
    public List<TransmittalDocumentDto> Documents { get; set; } = new();
    
    // Recipients
    public List<TransmittalRecipientDto> Recipients { get; set; } = new();
    
    // Attachments (non-document files)
    public List<TransmittalAttachmentDto> Attachments { get; set; } = new();
}

