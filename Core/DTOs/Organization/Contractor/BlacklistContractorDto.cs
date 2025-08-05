namespace Core.DTOs.Organization.Contractor;

/// <summary>
/// Blacklist contractor data transfer object
/// </summary>
public class BlacklistContractorDto
{
    public string Reason { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SupportingDocuments { get; set; }
}