using Core.Enums.Documents;

namespace Core.DTOs.Documents.DocumentDistribution;

public class CreateDocumentDistributionDto
{
    public Guid DocumentId { get; set; }
    public Guid? DocumentVersionId { get; set; } // If null, use current version
    public DistributionMethod Method { get; set; }
    public DistributionPurpose Purpose { get; set; }
    public string? Comments { get; set; }
    
    // Recipients
    public List<DistributionRecipientDto> Recipients { get; set; } = new();
    
    // Options
    public bool RequiresAcknowledgment { get; set; }
    public bool SendNotification { get; set; }
    public bool IncludeInTransmittal { get; set; }
    public Guid? TransmittalId { get; set; }
}
