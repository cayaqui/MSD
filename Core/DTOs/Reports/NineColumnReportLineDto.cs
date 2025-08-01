namespace Core.DTOs.Reports;

/// <summary>
/// DTO for each line in the Nine Column Report
/// Cada línea representa un Control Account o Work Package
/// </summary>
public class NineColumnReportLineDto
{
    // Identificación
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty; // WBS/CBS Code
    public int Level { get; set; } // Nivel jerárquico para indentación
    public bool IsControlAccount { get; set; }
    public bool IsWorkPackage { get; set; }
    public bool HasChildren { get; set; }

    // Las 9 columnas principales del reporte

    /// <summary>
    /// Columna 1: Descripción de Actividad/Partida
    /// </summary>
    public string ActivityDescription { get; set; } = string.Empty;

    /// <summary>
    /// Columna 2: Presupuesto Original (PV) - Valor Planificado/BCWS
    /// </summary>
    public decimal PlannedValue { get; set; }

    /// <summary>
    /// Columna 3: Avance Físico % - Porcentaje de completitud real
    /// </summary>
    public decimal PhysicalProgressPercentage { get; set; }

    /// <summary>
    /// Columna 4: Valor Ganado (EV) - BCWP
    /// </summary>
    public decimal EarnedValue { get; set; }

    /// <summary>
    /// Columna 5: Costo Real (AC) - ACWP
    /// </summary>
    public decimal ActualCost { get; set; }

    /// <summary>
    /// Columna 6: Variación de Costo (CV) = EV - AC
    /// </summary>
    public decimal CostVariance { get; set; }

    /// <summary>
    /// Columna 7: Variación de Cronograma (SV) = EV - PV
    /// </summary>
    public decimal ScheduleVariance { get; set; }

    /// <summary>
    /// Columna 8: Índice de Desempeño de Costo (CPI) = EV/AC
    /// </summary>
    public decimal CostPerformanceIndex { get; set; }

    /// <summary>
    /// Columna 9: Estimación al Completar (EAC)
    /// </summary>
    public decimal EstimateAtCompletion { get; set; }

    // Columnas adicionales opcionales
    public decimal? EstimateToComplete { get; set; } // ETC
    public decimal? VarianceAtCompletion { get; set; } // VAC = BAC - EAC
    public decimal? SchedulePerformanceIndex { get; set; } // SPI = EV/PV
    public decimal? ToCompletePerformanceIndex { get; set; } // TCPI

    // Información del responsable
    public string ResponsibleUserId { get; set; } = string.Empty;
    public string ResponsibleName { get; set; } = string.Empty;

    // Estado y alertas
    public string Status { get; set; } = string.Empty; // "On Track", "At Risk", "Critical"
    public bool HasCostOverrun => CostVariance < 0;
    public bool IsBehindSchedule => ScheduleVariance < 0;
    public string? AlertMessage { get; set; }
}
