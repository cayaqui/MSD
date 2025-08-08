using Domain.Common;
using Domain.Entities.Auth.Security;

namespace Domain.Entities.Documents;

/// <summary>
/// Representa la distribución de un documento
/// </summary>
public class DocumentDistribution : BaseAuditableEntity
{
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
    
    public Guid? DocumentVersionId { get; set; }
    public virtual DocumentVersion? DocumentVersion { get; set; }
    
    // Distribution details
    public DateTime DistributionDate { get; set; }
    public DistributionMethod Method { get; set; }
    public DistributionStatus Status { get; set; }
    
    // Recipient
    public Guid? RecipientUserId { get; set; }
    public virtual User? RecipientUser { get; set; }
    public string? RecipientEmail { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientCompany { get; set; }
    
    // Distribution metadata
    public string? Purpose { get; set; }
    public string? Comments { get; set; }
    public bool RequiresAcknowledgment { get; set; }
    public DateTime? AcknowledgedDate { get; set; }
    public string? AcknowledgmentComments { get; set; }
    
    // Transmittal
    public Guid? TransmittalId { get; set; }
    public string? TransmittalNumber { get; set; }
    
    // Distributed by
    public Guid DistributedById { get; set; }
    public virtual User DistributedBy { get; set; } = null!;
}

/// <summary>
/// Método de distribución
/// </summary>
public enum DistributionMethod
{
    Email,
    Portal,
    FTP,
    USB,
    Print,
    CloudShare,
    Other
}

/// <summary>
/// Estado de la distribución
/// </summary>
public enum DistributionStatus
{
    Pending,
    Sent,
    Delivered,
    Viewed,
    Acknowledged,
    Failed,
    Cancelled
}