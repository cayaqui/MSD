namespace Core.DTOs.EVM;

/// <summary>
/// DTO for EVM trend data
/// </summary>
public class EVMTrendDto
{
    public DateTime DataDate { get; set; }
    public decimal PV { get; set; }
    public decimal EV { get; set; }
    public decimal AC { get; set; }
    public decimal CPI { get; set; }
    public decimal SPI { get; set; }
}
