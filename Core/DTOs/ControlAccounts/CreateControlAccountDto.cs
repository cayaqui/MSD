using Core.Enums.Cost;

namespace Core.DTOs.ControlAccounts;

/// <summary>
/// DTO for creating a Control Account
/// </summary>
public class CreateControlAccountDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public Guid PhaseId { get; set; }
    public string CAMUserId { get; set; } = string.Empty;
    public decimal BAC { get; set; }
    public decimal ContingencyReserve { get; set; }
    public decimal ManagementReserve { get; set; }
    public MeasurementMethod MeasurementMethod { get; set; }
}
