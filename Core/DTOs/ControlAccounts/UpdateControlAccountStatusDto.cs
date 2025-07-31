using Core.Enums.Cost;

namespace Core.DTOs.ControlAccounts;

/// <summary>
/// DTO for Control Account status update
/// </summary>
public class UpdateControlAccountStatusDto
{
    public ControlAccountStatus NewStatus { get; set; }
    public string? Comments { get; set; }
}
