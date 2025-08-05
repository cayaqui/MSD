using Domain.Common;
using Domain.Entities.Auth.Security;
using Domain.Entities.Documents.Core;

namespace Domain.Entities.Documents.Transmittals;

public class Transmittal : BaseAuditableEntity, ISoftDelete
{
    public string TransmittalNumber { get; set; } = string.Empty;
    public DateTime TransmittalDate { get; set; }
    public TransmittalStatus Status { get; set; } = TransmittalStatus.Draft;
    public TransmittalPriority Priority { get; set; } = TransmittalPriority.Normal;
    
    // Project information
    public Guid ProjectId { get; set; }
    public virtual Project Project { get; set; } = null!;
    
    // From/To information
    public Guid FromCompanyId { get; set; }
    public virtual Company FromCompany { get; set; } = null!;
    public string? FromContact { get; set; }
    public string? FromEmail { get; set; }
    public string? FromPhone { get; set; }
    
    public Guid ToCompanyId { get; set; }
    public virtual Company ToCompany { get; set; } = null!;
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
    public virtual User PreparedBy { get; set; } = null!;
    public DateTime PreparedDate { get; set; }
    
    // Approval
    public bool RequiresApproval { get; set; }
    public Guid? ApprovedById { get; set; }
    public virtual User? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovalComments { get; set; }
    
    // Sent information
    public DateTime? SentDate { get; set; }
    public Guid? SentById { get; set; }
    public virtual User? SentBy { get; set; }
    
    
    // Navigation properties
    public virtual ICollection<TransmittalDocument> Documents { get; set; } = new List<TransmittalDocument>();
    public virtual ICollection<TransmittalRecipient> Recipients { get; set; } = new List<TransmittalRecipient>();
    public virtual ICollection<TransmittalAttachment> Attachments { get; set; } = new List<TransmittalAttachment>();
    public virtual ICollection<DocumentDistribution> DocumentDistributions { get; set; } = new List<DocumentDistribution>();
    
    // Helper methods
    public bool CanBeEdited()
    {
        return Status == TransmittalStatus.Draft || Status == TransmittalStatus.PendingApproval;
    }
    
    public bool CanBeSent()
    {
        return Status == TransmittalStatus.Approved || Status == TransmittalStatus.Draft && !RequiresApproval;
    }
    
    public void Send(Guid sentById)
    {
        Status = TransmittalStatus.Sent;
        SentDate = DateTime.UtcNow;
        SentById = sentById;
    }
    
    public void Deliver(string confirmedBy)
    {
        Status = TransmittalStatus.Delivered;
        IsDelivered = true;
        DeliveryDate = DateTime.UtcNow;
        DeliveryConfirmedDate = DateTime.UtcNow;
        DeliveryConfirmedBy = confirmedBy;
    }
}

public enum TransmittalStatus
{
    Draft,
    PendingApproval,
    Approved,
    Sent,
    Delivered,
    Acknowledged,
    Closed,
    Cancelled
}

public enum TransmittalPriority
{
    Low,
    Normal,
    High,
    Urgent
}

public enum DeliveryMethod
{
    Email,
    Portal,
    FTP,
    CloudStorage,
    Courier,
    HandDelivery,
    RegisteredMail,
    Other
}