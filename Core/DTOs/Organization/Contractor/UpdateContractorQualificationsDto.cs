namespace Core.DTOs.Organization.Contractor;

/// <summary>
/// Update contractor qualifications data transfer object
/// </summary>
public class UpdateContractorQualificationsDto
{
    public bool IsPrequalified { get; set; }
    public string? PrequalificationNotes { get; set; }
    public string? Certifications { get; set; }
    public string? SpecialtyAreas { get; set; }
}