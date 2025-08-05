using System;
using System.Collections.Generic;
using Core.Enums.Cost;

namespace Core.DTOs.Cost.ControlAccounts;

/// <summary>
/// Summary DTO for EVM metrics
/// </summary>
public class EVMSummaryDto
{
    public DateTime DataDate { get; set; }
    public decimal PV { get; set; }
    public decimal EV { get; set; }
    public decimal AC { get; set; }
    public decimal CV { get; set; }
    public decimal SV { get; set; }
    public decimal CPI { get; set; }
    public decimal SPI { get; set; }
    public decimal EAC { get; set; }
    public decimal VAC { get; set; }
    public EVMStatus Status { get; set; }
}