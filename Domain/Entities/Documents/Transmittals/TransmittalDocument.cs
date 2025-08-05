using Domain.Common;


namespace Domain.Entities.Documents.Transmittals;

public class TransmittalDocument : BaseEntity
{
    public Guid TransmittalId { get; set; }
    public virtual Transmittal Transmittal { get; set; } = null!;
    
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
    
    public Guid DocumentVersionId { get; set; }
    public virtual DocumentVersion DocumentVersion { get; set; } = null!;
    
    // Transmittal specific information
    public int Copies { get; set; } = 1;
    public string? Format { get; set; } // e.g., "Hard Copy", "Electronic", "Both"
    public string? Purpose { get; set; } // e.g., "For Review", "For Approval", etc.
    public string? Comments { get; set; }
    public int SortOrder { get; set; }
    
    // Tracking
    public bool IsIncluded { get; set; } = true; // Can be excluded without removing
    public DateTime AddedDate { get; set; }
    public Guid AddedById { get; set; }
}