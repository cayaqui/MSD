using Domain.Common;
using Domain.Entities.Auth.Security;
using Domain.Entities.Organization.Core;

namespace Domain.Entities.Documents.Core;

public class DocumentDistribution : BaseAuditableEntity
{
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
    public Guid DocumentVersionId { get; set; }
    public virtual DocumentVersion DocumentVersion { get; set; } = null!;
    
    // Distribution details
    public DistributionMethod Method { get; set; }
    public DistributionPurpose Purpose { get; set; }
    public DateTime DistributionDate { get; set; }
    public string? Comments { get; set; }
    
    // Recipient
    public Guid? RecipientUserId { get; set; }
    public virtual User? RecipientUser { get; set; }
    public string? RecipientEmail { get; set; }
    public Guid? RecipientCompanyId { get; set; }
    public virtual Company? RecipientCompany { get; set; }
    public string? RecipientRole { get; set; }
    
    // Sender
    public Guid DistributedById { get; set; }
    public virtual User DistributedBy { get; set; } = null!;
    
    // Transmittal
    public Guid? TransmittalId { get; set; }
    public virtual Transmittal? Transmittal { get; set; }
    
    // Acknowledgment
    public bool RequiresAcknowledgment { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedDate { get; set; }
    public string? AcknowledgedBy { get; set; }
    public string? AcknowledgmentComments { get; set; }
    
    // Download tracking
    public bool IsDownloaded { get; set; }
    public DateTime? FirstDownloadedDate { get; set; }
    public DateTime? LastDownloadedDate { get; set; }
    public int DownloadCount { get; set; }
    
    // Access control
    public DateTime? ExpiryDate { get; set; }
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
    public string? AccessToken { get; set; } // For secure download links
    
    // Helper methods
    public void RecordDownload()
    {
        if (!IsDownloaded)
        {
            IsDownloaded = true;
            FirstDownloadedDate = DateTime.UtcNow;
        }
        LastDownloadedDate = DateTime.UtcNow;
        DownloadCount++;
    }
    
    public void Acknowledge(string acknowledgedBy, string? comments = null)
    {
        IsAcknowledged = true;
        AcknowledgedDate = DateTime.UtcNow;
        AcknowledgedBy = acknowledgedBy;
        AcknowledgmentComments = comments;
    }
}

public enum DistributionMethod
{
    Email,
    Download,
    Portal,
    FTP,
    CloudStorage,
    Physical,
    Other
}

public enum DistributionPurpose
{
    ForInformation,
    ForReview,
    ForApproval,
    ForConstruction,
    ForRecord,
    ForAction,
    ForCoordination,
    Other
}