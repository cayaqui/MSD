using Core.DTOs.Common;
using Core.Enums.Documents;

namespace Core.DTOs.Documents.DocumentDistribution;
public class DocumentDistributionDto : BaseDto
{
    public Guid DocumentId { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string DocumentTitle { get; set; } = string.Empty;
    public Guid DocumentVersionId { get; set; }
    public string Version { get; set; } = string.Empty;
    
    // Distribution details
    public DistributionMethod Method { get; set; }
    public DistributionPurpose Purpose { get; set; }
    public DateTime DistributionDate { get; set; }
    public string? Comments { get; set; }
    
    // Recipient
    public Guid? RecipientUserId { get; set; }
    public string? RecipientUserName { get; set; }
    public string? RecipientEmail { get; set; }
    public Guid? RecipientCompanyId { get; set; }
    public string? RecipientCompanyName { get; set; }
    public string? RecipientRole { get; set; }
    
    // Sender
    public Guid DistributedById { get; set; }
    public string DistributedByName { get; set; } = string.Empty;
    
    // Transmittal
    public Guid? TransmittalId { get; set; }
    public string? TransmittalNumber { get; set; }
    
    // Acknowledgment
    public bool RequiresAcknowledgment { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedDate { get; set; }
    public string? AcknowledgedBy { get; set; }
    public string? AcknowledgmentComments { get; set; }
    
    // Download tracking
    public bool IsDownloaded { get; set; }
    public DateTime? DownloadedDate { get; set; }
    public int DownloadCount { get; set; }
}
