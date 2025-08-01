namespace Core.DTOs.Reports;

/// <summary>
/// DTO for report totals
/// Totales consolidados del proyecto
/// </summary>
public class NineColumnReportTotalsDto
{
    // Totales de las 9 columnas
    public decimal TotalPlannedValue { get; set; }
    public decimal TotalEarnedValue { get; set; }
    public decimal TotalActualCost { get; set; }
    public decimal TotalCostVariance { get; set; }
    public decimal TotalScheduleVariance { get; set; }
    public decimal TotalEstimateAtCompletion { get; set; }

    // Porcentajes y índices globales
    public decimal OverallPhysicalProgress { get; set; }
    public decimal OverallCPI { get; set; }
    public decimal OverallSPI { get; set; }

    // Presupuesto
    public decimal BudgetAtCompletion { get; set; } // BAC
    public decimal ContingencyReserve { get; set; }
    public decimal ManagementReserve { get; set; }
    public decimal TotalBudget { get; set; }

    // Análisis de variaciones
    public decimal ProjectedCostVariance { get; set; } // VAC
    public decimal ProjectedCostVariancePercentage { get; set; }

    // Resumen por tipo
    public Dictionary<string, decimal> CostByCategory { get; set; } = new();
    public Dictionary<string, decimal> CostByPhase { get; set; } = new();
}
