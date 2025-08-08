using Core.DTOs.Visualization;

namespace Web.Services.Interfaces.Visualization;

public interface IVisualizationApiService
{
    Task<GanttChartDto?> GenerateProjectGanttAsync(Guid projectId, GanttConfigDto? config = null);
    Task<CostSCurveDto?> GenerateCostSCurveAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ProgressSCurveDto?> GenerateProgressSCurveAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null);
    Task<EVMChartDto?> GenerateEVMChartAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ResourceUtilizationChartDto?> GenerateResourceUtilizationAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null);
    Task<CashFlowChartDto?> GenerateCashFlowChartAsync(Guid projectId, DateTime? startDate = null, DateTime? endDate = null);
    Task<MilestoneChartDto?> GenerateMilestoneChartAsync(Guid projectId);
    Task<RiskMatrixDto?> GenerateRiskMatrixAsync(Guid projectId);
    Task<ContractStatusChartDto?> GenerateContractStatusChartAsync(Guid projectId);
    Task<DashboardMetricsDto?> GetDashboardMetricsAsync(Guid projectId);
}