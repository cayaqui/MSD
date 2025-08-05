using Core.Enums.Projects;

namespace Core.DTOs.Organization.Contractor;

/// <summary>
/// DTO for filtering contractors
/// </summary>
public class ContractorFilterDto
{
    public string? SearchTerm { get; set; }
    public ContractorType? Type { get; set; }
    public ContractorClassification? Classification { get; set; }
    public ContractorStatus? Status { get; set; }
    public bool? IsPrequalified { get; set; }
    public bool? HasInsurance { get; set; }
    public bool? IsActive { get; set; }
    public decimal? MinPerformanceRating { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
}