using Core.DTOs.Common;
using Core.DTOs.Organization.Company;
using Core.DTOs.Organization.Project;
using Core.DTOs.Cost;
using Core.DTOs.Cost.Budgets;
using Core.DTOs.Cost.ControlAccounts;
using Core.DTOs.Cost.CostItems;
using Core.DTOs.Cost.Commitments;
using Core.DTOs.Cost.PlanningPackages;
using Core.DTOs.EVM;
using Core.DTOs.Contracts.Contracts;
using Core.DTOs.Progress.Activities;
using Core.DTOs.Progress.Milestones;
using Core.DTOs.Progress.Schedules;
// using Core.DTOs.Projects.WBS; // Namespace doesn't exist
using Core.DTOs.Projects.WBSElement;
using Core.DTOs.Risk.Risk;
using Core.DTOs.Documents.Document;
using Core.DTOs.Reports;

namespace Application.Interfaces.Common;

/// <summary>
/// Service for exporting data to Excel format
/// </summary>
public interface IExcelExportService
{
    #region Organization Exports
    
    /// <summary>
    /// Export companies list to Excel
    /// </summary>
    Task<byte[]> ExportCompaniesToExcelAsync(List<CompanyDto> companies, string fileName = "Companies");
    
    /// <summary>
    /// Export projects list to Excel
    /// </summary>
    Task<byte[]> ExportProjectsToExcelAsync(List<ProjectDto> projects, string fileName = "Projects");
    
    #endregion
    
    #region Cost Exports
    
    /// <summary>
    /// Export budgets to Excel
    /// </summary>
    Task<byte[]> ExportBudgetsToExcelAsync(List<BudgetDto> budgets, string fileName = "Budgets");
    
    /// <summary>
    /// Export control accounts to Excel
    /// </summary>
    Task<byte[]> ExportControlAccountsToExcelAsync(List<ControlAccountDto> controlAccounts, string fileName = "ControlAccounts");
    
    /// <summary>
    /// Export cost items to Excel
    /// </summary>
    Task<byte[]> ExportCostItemsToExcelAsync(List<CostItemDto> costItems, string fileName = "CostItems");
    
    /// <summary>
    /// Export commitments to Excel
    /// </summary>
    Task<byte[]> ExportCommitmentsToExcelAsync(List<CommitmentDto> commitments, string fileName = "Commitments");
    
    /// <summary>
    /// Export planning packages to Excel
    /// </summary>
    Task<byte[]> ExportPlanningPackagesToExcelAsync(List<PlanningPackageDto> planningPackages, string fileName = "PlanningPackages");
    
    #endregion
    
    #region EVM Exports
    
    /// <summary>
    /// Export EVM records to Excel
    /// </summary>
    Task<byte[]> ExportEVMRecordsToExcelAsync(List<EVMRecordDto> evmRecords, string fileName = "EVMRecords");
    
    /// <summary>
    /// Export Nine Column Report to Excel
    /// </summary>
    Task<byte[]> ExportNineColumnReportToExcelAsync(NineColumnReportDto report, string fileName = "NineColumnReport");
    
    #endregion
    
    #region Progress Exports
    
    /// <summary>
    /// Export activities to Excel
    /// </summary>
    Task<byte[]> ExportActivitiesToExcelAsync(List<ActivityDto> activities, string fileName = "Activities");
    
    /// <summary>
    /// Export milestones to Excel
    /// </summary>
    Task<byte[]> ExportMilestonesToExcelAsync(List<Core.DTOs.Progress.Milestones.MilestoneDto> milestones, string fileName = "Milestones");
    
    /// <summary>
    /// Export schedule to Excel
    /// </summary>
    Task<byte[]> ExportScheduleToExcelAsync(Core.DTOs.Progress.Schedules.ScheduleDto schedule, string fileName = "Schedule");
    
    #endregion
    
    #region WBS Exports
    
    /// <summary>
    /// Export WBS structure to Excel
    /// </summary>
    Task<byte[]> ExportWBSToExcelAsync(List<WBSElementDto> wbsElements, string fileName = "WBS");
    
    /// <summary>
    /// Export WBS hierarchical structure to Excel
    /// </summary>
    Task<byte[]> ExportWBSHierarchyToExcelAsync(WBSHierarchyDto hierarchy, string fileName = "WBSHierarchy");
    
    #endregion
    
    #region Contract Exports
    
    /// <summary>
    /// Export contracts to Excel
    /// </summary>
    Task<byte[]> ExportContractsToExcelAsync(List<ContractDto> contracts, string fileName = "Contracts");
    
    #endregion
    
    #region Risk Exports
    
    /// <summary>
    /// Export risk register to Excel
    /// </summary>
    Task<byte[]> ExportRiskRegisterToExcelAsync(RiskRegisterDto riskRegister, string fileName = "RiskRegister");
    
    /// <summary>
    /// Export risk matrix to Excel
    /// </summary>
    Task<byte[]> ExportRiskMatrixToExcelAsync(RiskMatrixDto riskMatrix, string fileName = "RiskMatrix");
    
    #endregion
    
    #region Document Exports
    
    /// <summary>
    /// Export document list to Excel
    /// </summary>
    Task<byte[]> ExportDocumentsToExcelAsync(List<DocumentDto> documents, string fileName = "Documents");
    
    #endregion
    
    #region Generic Export
    
    /// <summary>
    /// Export any list of objects to Excel
    /// </summary>
    Task<byte[]> ExportToExcelAsync<T>(List<T> data, string fileName = "Export", Dictionary<string, string>? columnMappings = null) where T : class;
    
    /// <summary>
    /// Export data with multiple sheets
    /// </summary>
    Task<byte[]> ExportMultipleSheets(Dictionary<string, object> sheets, string fileName = "Export");
    
    #endregion
}