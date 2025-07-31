using Core.Enums.Cost;

namespace Core.DTOs.ControlAccounts;

/// <summary>
/// DTO for updating a Control Account
/// </summary>
public class UpdateControlAccountDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CAMUserId { get; set; }
    public decimal? BAC { get; set; }
    public decimal? ContingencyReserve { get; set; }
    public decimal? ManagementReserve { get; set; }
    public MeasurementMethod? MeasurementMethod { get; set; }
}
