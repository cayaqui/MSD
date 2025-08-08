using Core.DTOs.Common;
using Core.DTOs.Organization.Company;
using Core.DTOs.Organization.Project;
using Core.DTOs.Cost.Budgets;
using Core.DTOs.Cost.ControlAccounts;
using Core.DTOs.Cost.CostItems;
using Core.DTOs.Cost.Commitments;
using Core.DTOs.Cost.PlanningPackages;
using Core.DTOs.Progress.Activities;
using Core.DTOs.Progress.Milestones;
// using Core.DTOs.Projects.WBS; // Namespace doesn't exist
using Core.DTOs.Projects.WBSElement;
using Core.DTOs.Risk.Risk;

namespace Application.Interfaces.Common;

/// <summary>
/// Service for importing data from Excel format
/// </summary>
public interface IExcelImportService
{
    #region Cost Imports
    
    /// <summary>
    /// Import budgets from Excel
    /// </summary>
    Task<ImportResult<CreateBudgetDto>> ImportBudgetsFromExcelAsync(Stream excelStream);
    
    /// <summary>
    /// Import control accounts from Excel
    /// </summary>
    Task<ImportResult<CreateControlAccountDto>> ImportControlAccountsFromExcelAsync(Stream excelStream, Guid projectId);
    
    /// <summary>
    /// Import cost items from Excel
    /// </summary>
    Task<ImportResult<CreateCostItemDto>> ImportCostItemsFromExcelAsync(Stream excelStream, Guid projectId);
    
    /// <summary>
    /// Import commitments from Excel
    /// </summary>
    Task<ImportResult<CreateCommitmentDto>> ImportCommitmentsFromExcelAsync(Stream excelStream, Guid projectId);
    
    /// <summary>
    /// Import planning packages from Excel
    /// </summary>
    Task<ImportResult<CreatePlanningPackageDto>> ImportPlanningPackagesFromExcelAsync(Stream excelStream, Guid controlAccountId);
    
    #endregion
    
    #region Progress Imports
    
    /// <summary>
    /// Import activities from Excel
    /// </summary>
    Task<ImportResult<CreateActivityDto>> ImportActivitiesFromExcelAsync(Stream excelStream, Guid scheduleId);
    
    /// <summary>
    /// Import milestones from Excel
    /// </summary>
    Task<ImportResult<CreateMilestoneDto>> ImportMilestonesFromExcelAsync(Stream excelStream, Guid projectId);
    
    #endregion
    
    #region WBS Imports
    
    /// <summary>
    /// Import WBS structure from Excel
    /// </summary>
    Task<ImportResult<CreateWBSElementDto>> ImportWBSFromExcelAsync(Stream excelStream, Guid projectId);
    
    #endregion
    
    #region Risk Imports
    
    /// <summary>
    /// Import risks from Excel
    /// </summary>
    Task<ImportResult<CreateRiskDto>> ImportRisksFromExcelAsync(Stream excelStream, Guid projectId);
    
    #endregion
    
    #region Template Downloads
    
    /// <summary>
    /// Get Excel template for budget import
    /// </summary>
    Task<byte[]> GetBudgetImportTemplateAsync();
    
    /// <summary>
    /// Get Excel template for control account import
    /// </summary>
    Task<byte[]> GetControlAccountImportTemplateAsync();
    
    /// <summary>
    /// Get Excel template for cost item import
    /// </summary>
    Task<byte[]> GetCostItemImportTemplateAsync();
    
    /// <summary>
    /// Get Excel template for activity import
    /// </summary>
    Task<byte[]> GetActivityImportTemplateAsync();
    
    /// <summary>
    /// Get Excel template for milestone import
    /// </summary>
    Task<byte[]> GetMilestoneImportTemplateAsync();
    
    /// <summary>
    /// Get Excel template for WBS import
    /// </summary>
    Task<byte[]> GetWBSImportTemplateAsync();
    
    /// <summary>
    /// Get Excel template for risk import
    /// </summary>
    Task<byte[]> GetRiskImportTemplateAsync();
    
    #endregion
    
    #region Validation
    
    /// <summary>
    /// Validate Excel file format
    /// </summary>
    Task<ValidationResult> ValidateExcelFileAsync(Stream excelStream, string templateType);
    
    #endregion
}

/// <summary>
/// Result of an import operation
/// </summary>
public class ImportResult<T> where T : class
{
    public bool Success { get; set; }
    public int TotalRows { get; set; }
    public int SuccessfulRows { get; set; }
    public int FailedRows { get; set; }
    public List<T> ImportedData { get; set; } = new();
    public List<ImportError> Errors { get; set; } = new();
}

/// <summary>
/// Import error details
/// </summary>
public class ImportError
{
    public int RowNumber { get; set; }
    public string ColumnName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// Validation result for Excel file
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public int RowCount { get; set; }
    public List<string> Columns { get; set; } = new();
}