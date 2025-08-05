using Core.Enums.Cost;

namespace Core.DTOs.Cost.ControlAccounts;

/// <summary>
/// DTO for Control Account assignment
/// </summary>
public class ControlAccountAssignmentDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public ControlAccountRole Role { get; set; }
    public DateTime AssignedDate { get; set; }
    public bool IsActive { get; set; }
    public decimal? AllocationPercentage { get; set; }
}
