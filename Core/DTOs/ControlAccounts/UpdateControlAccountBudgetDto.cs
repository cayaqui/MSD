namespace Core.DTOs.ControlAccounts;

/// <summary>
/// DTO for Control Account budget update
/// </summary>
public class UpdateControlAccountBudgetDto
{
    public decimal BAC { get; set; }
    public decimal ContingencyReserve { get; set; }
    public decimal ManagementReserve { get; set; }
    public string Justification { get; set; } = string.Empty;
}
