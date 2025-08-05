using Core.Enums.Documents;

namespace Core.DTOs.Documents.Transmittal;

public class CreateTransmittalRecipientDto
{
    public RecipientType Type { get; set; }
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public Guid? CompanyId { get; set; }
    public string? Role { get; set; }
    public bool RequiresAcknowledgment { get; set; }
}

