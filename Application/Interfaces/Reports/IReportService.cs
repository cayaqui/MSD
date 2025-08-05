using Core.DTOs.Reports;
using Core.Enums.Documents;
using Core.Enums.Reports;

namespace Application.Interfaces.Reports;

public interface IReportService : IBaseService<ReportDto, GenerateReportDto, GenerateReportDto>
{
    // Report Generation
    Task<ReportDto> GenerateReportAsync(GenerateReportDto dto);
    Task<ReportDto> RegenerateReportAsync(Guid reportId);
    
    // Report Retrieval
    Task<IEnumerable<ReportDto>> GetByProjectAsync(Guid projectId);
    Task<IEnumerable<ReportDto>> GetByTemplateAsync(Guid templateId);
    Task<IEnumerable<ReportDto>> GetByTypeAsync(ReportType type);
    Task<byte[]?> GetReportContentAsync(Guid reportId);
    
    // Report Distribution
    Task<bool> DistributeReportAsync(Guid reportId, List<string> recipients);
    Task<IEnumerable<ReportDistributionDto>> GetDistributionsAsync(Guid reportId);
    
    // Executive Dashboard
    Task<ExecutiveDashboardDto> GenerateExecutiveDashboardAsync(Guid projectId, DateTime? asOfDate = null);
    
    // Project Status Reports
    Task<ProjectStatusReportDto> GenerateProjectStatusReportAsync(Guid projectId, DateTime? reportDate = null);
    
    // Cost Reports
    Task<CostControlReportDto> GenerateCostControlReportAsync(Guid projectId, DateTime periodStart, DateTime periodEnd);
    Task<EarnedValueReportDto> GenerateEarnedValueReportAsync(Guid projectId, DateTime? asOfDate = null);
    
    // Schedule Reports
    Task<ScheduleProgressReportDto> GenerateScheduleProgressReportAsync(Guid projectId, DateTime? asOfDate = null);
    Task<MilestoneReportDto> GenerateMilestoneReportAsync(Guid projectId);
    
    // Risk Reports
    Task<RiskRegisterReportDto> GenerateRiskRegisterReportAsync(Guid projectId);
    Task<RiskMatrixReportDto> GenerateRiskMatrixReportAsync(Guid projectId);
    
    // Quality Reports
    Task<QualityMetricsReportDto> GenerateQualityMetricsReportAsync(Guid projectId, DateTime periodStart, DateTime periodEnd);
    
    // Resource Reports
    Task<ResourceUtilizationReportDto> GenerateResourceUtilizationReportAsync(Guid projectId, DateTime periodStart, DateTime periodEnd);
    
    // Export Methods
    Task<byte[]> ExportToPdfAsync(Guid reportId);
    Task<byte[]> ExportToExcelAsync(Guid reportId);
    Task<byte[]> ExportToWordAsync(Guid reportId);
    Task<byte[]> ExportToPowerPointAsync(Guid reportId);
    Task<byte[]> ExportToCsvAsync(Guid reportId);
    
    // Bulk Export
    Task<byte[]> ExportMultipleReportsAsync(List<Guid> reportIds, ExportFormat format, bool combineIntoOne = false);
}