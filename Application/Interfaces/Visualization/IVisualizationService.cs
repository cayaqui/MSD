using Core.DTOs.Visualization;

namespace Application.Interfaces.Visualization;

public interface IVisualizationService
{
    // Gantt Chart
    Task<GanttChartDto> GenerateProjectGanttAsync(Guid projectId, GanttConfigDto? config = null);
    Task<GanttChartDto> GenerateWBSGanttAsync(Guid projectId, string? wbsFilter = null, GanttConfigDto? config = null);
    Task<GanttChartDto> GenerateScheduleGanttAsync(Guid scheduleId, GanttConfigDto? config = null);
    Task<GanttChartDto> GenerateContractGanttAsync(Guid contractId, GanttConfigDto? config = null);
    Task<GanttChartDto> GenerateResourceGanttAsync(Guid projectId, List<Guid>? resourceIds = null, GanttConfigDto? config = null);
    
    // S-Curves
    Task<CostSCurveDto> GenerateCostSCurveAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ProgressSCurveDto> GenerateProgressSCurveAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null);
    Task<SCurveDto> GenerateEarnedValueSCurveAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null);
    Task<SCurveDto> GenerateResourceSCurveAsync(Guid projectId, string resourceType, DateTime? startDate = null, DateTime? endDate = null);
    Task<SCurveDto> GenerateCashFlowSCurveAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null);
    Task<SCurveDto> GenerateContractSCurveAsync(Guid contractId, DateTime? startDate = null, DateTime? endDate = null);
    
    // Combined Views
    Task<Dictionary<string, object>> GenerateDashboardChartsAsync(Guid projectId);
    Task<byte[]> ExportGanttToPdfAsync(Guid projectId, GanttConfigDto? config = null);
    Task<byte[]> ExportSCurveToPdfAsync(Guid projectId, string curveType, SCurveConfigDto? config = null);
    
    // Data Export
    Task<byte[]> ExportGanttDataToExcelAsync(Guid projectId);
    Task<byte[]> ExportSCurveDataToExcelAsync(Guid projectId, string curveType);
}