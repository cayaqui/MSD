using Core.Enums.Cost;

namespace Core.DTOs.ControlAccounts;

/// <summary>
/// DTO for Control Account list view
/// </summary>
public class ControlAccountDto
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Type { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid PhaseId { get; set; }
    public string PhaseName { get; set; } = string.Empty;
    public string CAMUserId { get; set; } = string.Empty;
    public string CAMName { get; set; } = string.Empty;
    public decimal AC { get; set; }
    public decimal EV { get; set; }
    public decimal PV { get; set; }
    public decimal BAC { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal PercentComplete { get; set; }
    public MeasurementMethod MeasurementMethod { get; set; }
    public ControlAccountStatus Status { get; set; }
    public DateTime BaselineDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
