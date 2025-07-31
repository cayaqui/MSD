namespace Core.DTOs.EVM;

/// <summary>
/// DTO for updating EVM actuals
/// </summary>
public class UpdateEVMActualsDto
{
    public decimal EV { get; set; }
    public decimal AC { get; set; }
    public string? Comments { get; set; }
}
