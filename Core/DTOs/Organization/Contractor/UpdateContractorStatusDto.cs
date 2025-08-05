using Core.Enums.Projects;

namespace Core.DTOs.Organization.Contractor;

/// <summary>
/// DTO for updating contractor status
/// </summary>
public class UpdateContractorStatusDto
{
    public ContractorStatus Status { get; set; }
    public string? Notes { get; set; }
}