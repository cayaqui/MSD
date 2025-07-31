using Core.Enums.Cost;

namespace Core.DTOs.EVM;

/// <summary>
/// DTO for EVM Record list view
/// </summary>
public class EVMRecordDto
{
    public Guid Id { get; set; }
    public Guid ControlAccountId { get; set; }
    public string ControlAccountCode { get; set; } = string.Empty;
    public string ControlAccountName { get; set; } = string.Empty;
    public DateTime DataDate { get; set; }
    public EVMPeriodType PeriodType { get; set; }
    public int PeriodNumber { get; set; }
    public int Year { get; set; }
    public decimal PV { get; set; }
    public decimal EV { get; set; }
    public decimal AC { get; set; }
    public decimal CV { get; set; }
    public decimal SV { get; set; }
    public decimal CPI { get; set; }
    public decimal SPI { get; set; }
    public EVMStatus Status { get; set; }
}
