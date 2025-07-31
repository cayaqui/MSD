using Core.Enums.Cost;

namespace Core.DTOs.EVM;

/// <summary>
/// DTO for creating an EVM Record
/// </summary>
public class CreateEVMRecordDto
{
    public Guid ControlAccountId { get; set; }
    public DateTime DataDate { get; set; }
    public EVMPeriodType PeriodType { get; set; }
    public decimal PV { get; set; }
    public decimal EV { get; set; }
    public decimal AC { get; set; }
    public decimal BAC { get; set; }
    public string? Comments { get; set; }
}
