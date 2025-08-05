using Domain.Common;
using Domain.Entities.Organization.Core;

namespace Domain.Entities.Documents.Transmittals;

public class TransmittalRecipient : BaseEntity
{
    public Guid TransmittalId { get; set; }
    public virtual Transmittal Transmittal { get; set; } = null!;
    
    public RecipientType Type { get; set; } // To, CC, BCC
    
    // Recipient information
    public Guid? UserId { get; set; }
    public virtual User? User { get; set; }
    public string? Email { get; set; }
    public Guid? CompanyId { get; set; }
    public virtual Company? Company { get; set; }
    public string? Role { get; set; }
    public string? Name { get; set; } // For external recipients
    
    // Acknowledgment
    public bool RequiresAcknowledgment { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedDate { get; set; }
    public string? AcknowledgmentComments { get; set; }
    public string? AcknowledgmentToken { get; set; } // For external acknowledgment
    
    // Delivery status
    public bool IsDelivered { get; set; }
    public DateTime? DeliveredDate { get; set; }
    public string? DeliveryStatus { get; set; }
    public string? DeliveryError { get; set; }
    
    // Helper methods
    public void Acknowledge(string? comments = null)
    {
        IsAcknowledged = true;
        AcknowledgedDate = DateTime.UtcNow;
        AcknowledgmentComments = comments;
    }
    
    public string GetRecipientIdentifier()
    {
        if (User != null)
            return User.DisplayName;
        if (!string.IsNullOrEmpty(Name))
            return Name;
        return Email ?? "Unknown Recipient";
    }
}

public enum RecipientType
{
    To,
    CopyTo,
    BlindCopyTo
}