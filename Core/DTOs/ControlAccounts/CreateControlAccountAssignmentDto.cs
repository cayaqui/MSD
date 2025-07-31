using Core.Enums.Cost;

namespace Core.DTOs.ControlAccounts;

/// <summary>
/// DTO for creating Control Account assignment
/// </summary>
public class CreateControlAccountAssignmentDto
{
    public string UserId { get; set; } = string.Empty;
    public ControlAccountRole Role { get; set; }
    public decimal? AllocationPercentage { get; set; }
}
