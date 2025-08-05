namespace Core.DTOs.Documents.DocumentDistribution;

public class DistributionRecipientDto
{
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public Guid? CompanyId { get; set; }
    public string? Role { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsCopyTo { get; set; }
}
