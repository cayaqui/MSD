using Core.Enums.Documents;

namespace Core.DTOs.Documents.TransmittalRecipient;

public class TransmittalRecipientDto
{
    public Guid Id { get; set; }
    public Guid TransmittalId { get; set; }
    public RecipientType Type { get; set; }
    
    // Recipient info
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string? Role { get; set; }
    
    // Acknowledgment
    public bool RequiresAcknowledgment { get; set; }
    public bool IsAcknowledged { get; set; }
    public DateTime? AcknowledgedDate { get; set; }
    public string? AcknowledgmentComments { get; set; }
}
