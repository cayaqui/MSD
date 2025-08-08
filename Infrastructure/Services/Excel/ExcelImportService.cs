using Application.Interfaces.Common;
using ClosedXML.Excel;
using Core.DTOs.Common;
using Core.DTOs.Cost.Budgets;
using Core.DTOs.Cost.ControlAccounts;
using Core.DTOs.Cost.CostItems;
using Core.DTOs.Cost.Commitments;
using Core.DTOs.Cost.PlanningPackages;
using Core.DTOs.Progress.Activities;
using Core.DTOs.Progress.Milestones;
using Core.DTOs.Projects.WBS;
using Core.DTOs.Risk.Risk;
using Core.Enums.Cost;
using Core.Enums.Progress;
using Core.Enums.WBS;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Excel;

/// <summary>
/// Service for importing data from Excel format using ClosedXML
/// </summary>
public class ExcelImportService : IExcelImportService
{
    private readonly ILogger<ExcelImportService> _logger;

    public ExcelImportService(ILogger<ExcelImportService> logger)
    {
        _logger = logger;
    }

    #region Cost Imports

    public async Task<ImportResult<CreateBudgetDto>> ImportBudgetsFromExcelAsync(Stream excelStream)
    {
        var result = new ImportResult<CreateBudgetDto>();
        
        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);
            
            var rows = worksheet.RowsUsed().Skip(1); // Skip header
            result.TotalRows = rows.Count();
            
            foreach (var row in rows)
            {
                try
                {
                    var dto = new CreateBudgetDto
                    {
                        Code = row.Cell(1).GetString(),
                        Name = row.Cell(2).GetString(),
                        Version = row.Cell(3).GetString(),
                        TotalBudget = row.Cell(4).GetValue<decimal>(),
                        ContingencyAmount = row.Cell(5).GetValue<decimal>(),
                        ManagementReserve = row.Cell(6).GetValue<decimal>(),
                        EffectiveDate = row.Cell(7).GetDateTime(),
                        Comments = row.Cell(8).GetString()
                    };
                    
                    result.ImportedData.Add(dto);
                    result.SuccessfulRows++;
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = row.RowNumber(),
                        ErrorMessage = ex.Message,
                        ColumnName = "General",
                        Value = ""
                    });
                }
            }
            
            result.Success = result.FailedRows == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing budgets from Excel");
            result.Success = false;
            result.Errors.Add(new ImportError
            {
                RowNumber = 0,
                ErrorMessage = $"Error reading Excel file: {ex.Message}",
                ColumnName = "File",
                Value = ""
            });
        }
        
        return result;
    }

    public async Task<ImportResult<CreateControlAccountDto>> ImportControlAccountsFromExcelAsync(Stream excelStream, Guid projectId)
    {
        var result = new ImportResult<CreateControlAccountDto>();
        
        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);
            
            var rows = worksheet.RowsUsed().Skip(1); // Skip header
            result.TotalRows = rows.Count();
            
            foreach (var row in rows)
            {
                try
                {
                    var dto = new CreateControlAccountDto
                    {
                        ProjectId = projectId,
                        Code = row.Cell(1).GetString(),
                        Name = row.Cell(2).GetString(),
                        Type = row.Cell(3).GetString(),
                        Budget = row.Cell(4).GetValue<decimal>(),
                        StartDate = row.Cell(5).GetDateTime(),
                        EndDate = row.Cell(6).GetDateTime(),
                        WBSCode = row.Cell(7).GetString(),
                        Description = row.Cell(8).GetString()
                    };
                    
                    result.ImportedData.Add(dto);
                    result.SuccessfulRows++;
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = row.RowNumber(),
                        ErrorMessage = ex.Message,
                        ColumnName = "General",
                        Value = ""
                    });
                }
            }
            
            result.Success = result.FailedRows == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing control accounts from Excel");
            result.Success = false;
            result.Errors.Add(new ImportError
            {
                RowNumber = 0,
                ErrorMessage = $"Error reading Excel file: {ex.Message}",
                ColumnName = "File",
                Value = ""
            });
        }
        
        return result;
    }

    public async Task<ImportResult<CreateCostItemDto>> ImportCostItemsFromExcelAsync(Stream excelStream, Guid projectId)
    {
        var result = new ImportResult<CreateCostItemDto>();
        
        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);
            
            var rows = worksheet.RowsUsed().Skip(1); // Skip header
            result.TotalRows = rows.Count();
            
            foreach (var row in rows)
            {
                try
                {
                    var dto = new CreateCostItemDto
                    {
                        ProjectId = projectId,
                        ItemCode = row.Cell(1).GetString(),
                        Description = row.Cell(2).GetString(),
                        Type = Enum.Parse<CostItemType>(row.Cell(3).GetString()),
                        Category = Enum.Parse<CostCategory>(row.Cell(4).GetString()),
                        PlannedCost = row.Cell(5).GetValue<decimal>(),
                        Unit = row.Cell(6).GetString(),
                        Quantity = row.Cell(7).GetValue<decimal>(),
                        UnitCost = row.Cell(8).GetValue<decimal>()
                    };
                    
                    // Optional fields
                    var controlAccountCode = row.Cell(9).GetString();
                    if (!string.IsNullOrEmpty(controlAccountCode))
                    {
                        // Note: We would need to lookup the control account ID by code
                        // This would be done in the service layer
                    }
                    
                    result.ImportedData.Add(dto);
                    result.SuccessfulRows++;
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = row.RowNumber(),
                        ErrorMessage = ex.Message,
                        ColumnName = "General",
                        Value = ""
                    });
                }
            }
            
            result.Success = result.FailedRows == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing cost items from Excel");
            result.Success = false;
            result.Errors.Add(new ImportError
            {
                RowNumber = 0,
                ErrorMessage = $"Error reading Excel file: {ex.Message}",
                ColumnName = "File",
                Value = ""
            });
        }
        
        return result;
    }

    public async Task<ImportResult<CreateCommitmentDto>> ImportCommitmentsFromExcelAsync(Stream excelStream, Guid projectId)
    {
        var result = new ImportResult<CreateCommitmentDto>();
        
        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);
            
            var rows = worksheet.RowsUsed().Skip(1); // Skip header
            result.TotalRows = rows.Count();
            
            foreach (var row in rows)
            {
                try
                {
                    var dto = new CreateCommitmentDto
                    {
                        ProjectId = projectId,
                        CommitmentNumber = row.Cell(1).GetString(),
                        Description = row.Cell(2).GetString(),
                        Type = Enum.Parse<CommitmentType>(row.Cell(3).GetString()),
                        VendorName = row.Cell(4).GetString(),
                        OriginalAmount = row.Cell(5).GetValue<decimal>(),
                        StartDate = row.Cell(6).GetDateTime(),
                        EndDate = row.Cell(7).GetDateTime(),
                        RetentionPercentage = row.Cell(8).GetValue<decimal>(),
                        Comments = row.Cell(9).GetString()
                    };
                    
                    result.ImportedData.Add(dto);
                    result.SuccessfulRows++;
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = row.RowNumber(),
                        ErrorMessage = ex.Message,
                        ColumnName = "General",
                        Value = ""
                    });
                }
            }
            
            result.Success = result.FailedRows == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing commitments from Excel");
            result.Success = false;
            result.Errors.Add(new ImportError
            {
                RowNumber = 0,
                ErrorMessage = $"Error reading Excel file: {ex.Message}",
                ColumnName = "File",
                Value = ""
            });
        }
        
        return result;
    }

    public async Task<ImportResult<CreatePlanningPackageDto>> ImportPlanningPackagesFromExcelAsync(Stream excelStream, Guid controlAccountId)
    {
        var result = new ImportResult<CreatePlanningPackageDto>();
        
        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);
            
            var rows = worksheet.RowsUsed().Skip(1); // Skip header
            result.TotalRows = rows.Count();
            
            foreach (var row in rows)
            {
                try
                {
                    var dto = new CreatePlanningPackageDto
                    {
                        ControlAccountId = controlAccountId,
                        Code = row.Cell(1).GetString(),
                        Name = row.Cell(2).GetString(),
                        Description = row.Cell(3).GetString(),
                        Budget = row.Cell(4).GetValue<decimal>(),
                        EstimatedHours = row.Cell(5).GetValue<decimal>(),
                        PlannedStartDate = row.Cell(6).GetDateTime(),
                        PlannedEndDate = row.Cell(7).GetDateTime(),
                        Priority = row.Cell(8).GetValue<int>()
                    };
                    
                    result.ImportedData.Add(dto);
                    result.SuccessfulRows++;
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = row.RowNumber(),
                        ErrorMessage = ex.Message,
                        ColumnName = "General",
                        Value = ""
                    });
                }
            }
            
            result.Success = result.FailedRows == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing planning packages from Excel");
            result.Success = false;
            result.Errors.Add(new ImportError
            {
                RowNumber = 0,
                ErrorMessage = $"Error reading Excel file: {ex.Message}",
                ColumnName = "File",
                Value = ""
            });
        }
        
        return result;
    }

    #endregion

    #region Progress Imports

    public async Task<ImportResult<CreateActivityDto>> ImportActivitiesFromExcelAsync(Stream excelStream, Guid scheduleId)
    {
        var result = new ImportResult<CreateActivityDto>();
        
        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);
            
            var rows = worksheet.RowsUsed().Skip(1); // Skip header
            result.TotalRows = rows.Count();
            
            foreach (var row in rows)
            {
                try
                {
                    var dto = new CreateActivityDto
                    {
                        ScheduleId = scheduleId,
                        ActivityCode = row.Cell(1).GetString(),
                        Name = row.Cell(2).GetString(),
                        Description = row.Cell(3).GetString(),
                        ActivityType = Enum.Parse<ActivityType>(row.Cell(4).GetString()),
                        Duration = row.Cell(5).GetValue<int>(),
                        PlannedStartDate = row.Cell(6).GetDateTime(),
                        PlannedEndDate = row.Cell(7).GetDateTime(),
                        WBSCode = row.Cell(8).GetString(),
                        ResponsibleResource = row.Cell(9).GetString()
                    };
                    
                    // Handle predecessors (comma-separated list)
                    var predecessors = row.Cell(10).GetString();
                    if (!string.IsNullOrEmpty(predecessors))
                    {
                        dto.PredecessorCodes = predecessors.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .ToList();
                    }
                    
                    result.ImportedData.Add(dto);
                    result.SuccessfulRows++;
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = row.RowNumber(),
                        ErrorMessage = ex.Message,
                        ColumnName = "General",
                        Value = ""
                    });
                }
            }
            
            result.Success = result.FailedRows == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing activities from Excel");
            result.Success = false;
            result.Errors.Add(new ImportError
            {
                RowNumber = 0,
                ErrorMessage = $"Error reading Excel file: {ex.Message}",
                ColumnName = "File",
                Value = ""
            });
        }
        
        return result;
    }

    public async Task<ImportResult<CreateMilestoneDto>> ImportMilestonesFromExcelAsync(Stream excelStream, Guid projectId)
    {
        var result = new ImportResult<CreateMilestoneDto>();
        
        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);
            
            var rows = worksheet.RowsUsed().Skip(1); // Skip header
            result.TotalRows = rows.Count();
            
            foreach (var row in rows)
            {
                try
                {
                    var dto = new CreateMilestoneDto
                    {
                        ProjectId = projectId,
                        Code = row.Cell(1).GetString(),
                        Name = row.Cell(2).GetString(),
                        Description = row.Cell(3).GetString(),
                        Type = Enum.Parse<MilestoneType>(row.Cell(4).GetString()),
                        Category = row.Cell(5).GetString(),
                        PlannedDate = row.Cell(6).GetDateTime(),
                        IsCritical = row.Cell(7).GetString().ToLower() == "yes",
                        DeliverableDescription = row.Cell(8).GetString(),
                        AcceptanceCriteria = row.Cell(9).GetString()
                    };
                    
                    result.ImportedData.Add(dto);
                    result.SuccessfulRows++;
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = row.RowNumber(),
                        ErrorMessage = ex.Message,
                        ColumnName = "General",
                        Value = ""
                    });
                }
            }
            
            result.Success = result.FailedRows == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing milestones from Excel");
            result.Success = false;
            result.Errors.Add(new ImportError
            {
                RowNumber = 0,
                ErrorMessage = $"Error reading Excel file: {ex.Message}",
                ColumnName = "File",
                Value = ""
            });
        }
        
        return result;
    }

    #endregion

    #region WBS Imports

    public async Task<ImportResult<CreateWBSElementDto>> ImportWBSFromExcelAsync(Stream excelStream, Guid projectId)
    {
        var result = new ImportResult<CreateWBSElementDto>();
        
        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);
            
            var rows = worksheet.RowsUsed().Skip(1); // Skip header
            result.TotalRows = rows.Count();
            
            foreach (var row in rows)
            {
                try
                {
                    var dto = new CreateWBSElementDto
                    {
                        ProjectId = projectId,
                        Code = row.Cell(1).GetString(),
                        Name = row.Cell(2).GetString(),
                        Description = row.Cell(3).GetString(),
                        Type = Enum.Parse<WBSType>(row.Cell(4).GetString()),
                        ParentCode = row.Cell(5).GetString(),
                        PlannedStartDate = row.Cell(6).GetDateTime(),
                        PlannedEndDate = row.Cell(7).GetDateTime(),
                        Budget = row.Cell(8).GetValue<decimal>()
                    };
                    
                    result.ImportedData.Add(dto);
                    result.SuccessfulRows++;
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = row.RowNumber(),
                        ErrorMessage = ex.Message,
                        ColumnName = "General",
                        Value = ""
                    });
                }
            }
            
            result.Success = result.FailedRows == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing WBS from Excel");
            result.Success = false;
            result.Errors.Add(new ImportError
            {
                RowNumber = 0,
                ErrorMessage = $"Error reading Excel file: {ex.Message}",
                ColumnName = "File",
                Value = ""
            });
        }
        
        return result;
    }

    #endregion

    #region Risk Imports

    public async Task<ImportResult<CreateRiskDto>> ImportRisksFromExcelAsync(Stream excelStream, Guid projectId)
    {
        var result = new ImportResult<CreateRiskDto>();
        
        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);
            
            var rows = worksheet.RowsUsed().Skip(1); // Skip header
            result.TotalRows = rows.Count();
            
            foreach (var row in rows)
            {
                try
                {
                    var dto = new CreateRiskDto
                    {
                        ProjectId = projectId,
                        Title = row.Cell(1).GetString(),
                        Description = row.Cell(2).GetString(),
                        Category = row.Cell(3).GetString(),
                        Type = row.Cell(4).GetString(),
                        Probability = row.Cell(5).GetValue<int>(),
                        Impact = row.Cell(6).GetValue<int>(),
                        TriggerEvent = row.Cell(7).GetString(),
                        CostImpact = row.Cell(8).IsEmpty() ? null : row.Cell(8).GetValue<decimal>(),
                        ScheduleImpact = row.Cell(9).IsEmpty() ? null : row.Cell(9).GetValue<int>()
                    };
                    
                    result.ImportedData.Add(dto);
                    result.SuccessfulRows++;
                }
                catch (Exception ex)
                {
                    result.FailedRows++;
                    result.Errors.Add(new ImportError
                    {
                        RowNumber = row.RowNumber(),
                        ErrorMessage = ex.Message,
                        ColumnName = "General",
                        Value = ""
                    });
                }
            }
            
            result.Success = result.FailedRows == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing risks from Excel");
            result.Success = false;
            result.Errors.Add(new ImportError
            {
                RowNumber = 0,
                ErrorMessage = $"Error reading Excel file: {ex.Message}",
                ColumnName = "File",
                Value = ""
            });
        }
        
        return result;
    }

    #endregion

    #region Template Downloads

    public async Task<byte[]> GetBudgetImportTemplateAsync()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Budget Template");
        
        // Add headers
        worksheet.Cell(1, 1).Value = "Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Version";
        worksheet.Cell(1, 4).Value = "Total Budget";
        worksheet.Cell(1, 5).Value = "Contingency Amount";
        worksheet.Cell(1, 6).Value = "Management Reserve";
        worksheet.Cell(1, 7).Value = "Effective Date";
        worksheet.Cell(1, 8).Value = "Comments";
        
        // Add sample data
        worksheet.Cell(2, 1).Value = "BUD-2024-001";
        worksheet.Cell(2, 2).Value = "Project Alpha Budget";
        worksheet.Cell(2, 3).Value = "1.0";
        worksheet.Cell(2, 4).Value = 1000000;
        worksheet.Cell(2, 5).Value = 100000;
        worksheet.Cell(2, 6).Value = 50000;
        worksheet.Cell(2, 7).Value = DateTime.Now;
        worksheet.Cell(2, 8).Value = "Initial budget approval";
        
        FormatTemplateWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> GetControlAccountImportTemplateAsync()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Control Account Template");
        
        // Add headers
        worksheet.Cell(1, 1).Value = "Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Type";
        worksheet.Cell(1, 4).Value = "Budget";
        worksheet.Cell(1, 5).Value = "Start Date";
        worksheet.Cell(1, 6).Value = "End Date";
        worksheet.Cell(1, 7).Value = "WBS Code";
        worksheet.Cell(1, 8).Value = "Description";
        
        // Add sample data
        worksheet.Cell(2, 1).Value = "CA-001";
        worksheet.Cell(2, 2).Value = "Engineering Design";
        worksheet.Cell(2, 3).Value = "Engineering";
        worksheet.Cell(2, 4).Value = 250000;
        worksheet.Cell(2, 5).Value = DateTime.Now;
        worksheet.Cell(2, 6).Value = DateTime.Now.AddMonths(6);
        worksheet.Cell(2, 7).Value = "WBS-1.1";
        worksheet.Cell(2, 8).Value = "Engineering design phase";
        
        FormatTemplateWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> GetCostItemImportTemplateAsync()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Cost Item Template");
        
        // Add headers
        worksheet.Cell(1, 1).Value = "Item Code";
        worksheet.Cell(1, 2).Value = "Description";
        worksheet.Cell(1, 3).Value = "Type (Labor/Material/Equipment/Subcontract/Other)";
        worksheet.Cell(1, 4).Value = "Category (Direct/Indirect/Overhead)";
        worksheet.Cell(1, 5).Value = "Planned Cost";
        worksheet.Cell(1, 6).Value = "Unit";
        worksheet.Cell(1, 7).Value = "Quantity";
        worksheet.Cell(1, 8).Value = "Unit Cost";
        worksheet.Cell(1, 9).Value = "Control Account Code (Optional)";
        
        // Add sample data
        worksheet.Cell(2, 1).Value = "CI-001";
        worksheet.Cell(2, 2).Value = "Senior Engineer";
        worksheet.Cell(2, 3).Value = "Labor";
        worksheet.Cell(2, 4).Value = "Direct";
        worksheet.Cell(2, 5).Value = 50000;
        worksheet.Cell(2, 6).Value = "Hours";
        worksheet.Cell(2, 7).Value = 500;
        worksheet.Cell(2, 8).Value = 100;
        worksheet.Cell(2, 9).Value = "CA-001";
        
        FormatTemplateWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> GetActivityImportTemplateAsync()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Activity Template");
        
        // Add headers
        worksheet.Cell(1, 1).Value = "Activity Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Description";
        worksheet.Cell(1, 4).Value = "Type (Task/Milestone/Summary)";
        worksheet.Cell(1, 5).Value = "Duration (days)";
        worksheet.Cell(1, 6).Value = "Start Date";
        worksheet.Cell(1, 7).Value = "End Date";
        worksheet.Cell(1, 8).Value = "WBS Code";
        worksheet.Cell(1, 9).Value = "Responsible Resource";
        worksheet.Cell(1, 10).Value = "Predecessors (comma-separated)";
        
        // Add sample data
        worksheet.Cell(2, 1).Value = "ACT-001";
        worksheet.Cell(2, 2).Value = "Site Preparation";
        worksheet.Cell(2, 3).Value = "Prepare construction site";
        worksheet.Cell(2, 4).Value = "Task";
        worksheet.Cell(2, 5).Value = 10;
        worksheet.Cell(2, 6).Value = DateTime.Now;
        worksheet.Cell(2, 7).Value = DateTime.Now.AddDays(10);
        worksheet.Cell(2, 8).Value = "WBS-1.1.1";
        worksheet.Cell(2, 9).Value = "John Smith";
        worksheet.Cell(2, 10).Value = "";
        
        FormatTemplateWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> GetMilestoneImportTemplateAsync()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Milestone Template");
        
        // Add headers
        worksheet.Cell(1, 1).Value = "Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Description";
        worksheet.Cell(1, 4).Value = "Type (Project/Contract/Payment/Internal)";
        worksheet.Cell(1, 5).Value = "Category";
        worksheet.Cell(1, 6).Value = "Planned Date";
        worksheet.Cell(1, 7).Value = "Is Critical (Yes/No)";
        worksheet.Cell(1, 8).Value = "Deliverable Description";
        worksheet.Cell(1, 9).Value = "Acceptance Criteria";
        
        // Add sample data
        worksheet.Cell(2, 1).Value = "MS-001";
        worksheet.Cell(2, 2).Value = "Design Complete";
        worksheet.Cell(2, 3).Value = "Engineering design phase completion";
        worksheet.Cell(2, 4).Value = "Project";
        worksheet.Cell(2, 5).Value = "Engineering";
        worksheet.Cell(2, 6).Value = DateTime.Now.AddMonths(3);
        worksheet.Cell(2, 7).Value = "Yes";
        worksheet.Cell(2, 8).Value = "Complete engineering drawings and specifications";
        worksheet.Cell(2, 9).Value = "All drawings approved by client";
        
        FormatTemplateWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> GetWBSImportTemplateAsync()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("WBS Template");
        
        // Add headers
        worksheet.Cell(1, 1).Value = "Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Description";
        worksheet.Cell(1, 4).Value = "Type (Phase/Deliverable/WorkPackage/Activity)";
        worksheet.Cell(1, 5).Value = "Parent Code (blank for root)";
        worksheet.Cell(1, 6).Value = "Start Date";
        worksheet.Cell(1, 7).Value = "End Date";
        worksheet.Cell(1, 8).Value = "Budget";
        
        // Add sample data
        worksheet.Cell(2, 1).Value = "1";
        worksheet.Cell(2, 2).Value = "Project Alpha";
        worksheet.Cell(2, 3).Value = "Main project";
        worksheet.Cell(2, 4).Value = "Phase";
        worksheet.Cell(2, 5).Value = "";
        worksheet.Cell(2, 6).Value = DateTime.Now;
        worksheet.Cell(2, 7).Value = DateTime.Now.AddYears(1);
        worksheet.Cell(2, 8).Value = 1000000;
        
        worksheet.Cell(3, 1).Value = "1.1";
        worksheet.Cell(3, 2).Value = "Engineering";
        worksheet.Cell(3, 3).Value = "Engineering phase";
        worksheet.Cell(3, 4).Value = "Deliverable";
        worksheet.Cell(3, 5).Value = "1";
        worksheet.Cell(3, 6).Value = DateTime.Now;
        worksheet.Cell(3, 7).Value = DateTime.Now.AddMonths(6);
        worksheet.Cell(3, 8).Value = 300000;
        
        FormatTemplateWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> GetRiskImportTemplateAsync()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Risk Template");
        
        // Add headers
        worksheet.Cell(1, 1).Value = "Title";
        worksheet.Cell(1, 2).Value = "Description";
        worksheet.Cell(1, 3).Value = "Category";
        worksheet.Cell(1, 4).Value = "Type";
        worksheet.Cell(1, 5).Value = "Probability (1-5)";
        worksheet.Cell(1, 6).Value = "Impact (1-5)";
        worksheet.Cell(1, 7).Value = "Trigger Event";
        worksheet.Cell(1, 8).Value = "Cost Impact (Optional)";
        worksheet.Cell(1, 9).Value = "Schedule Impact Days (Optional)";
        
        // Add sample data
        worksheet.Cell(2, 1).Value = "Material Delivery Delay";
        worksheet.Cell(2, 2).Value = "Risk of delayed material delivery due to supply chain issues";
        worksheet.Cell(2, 3).Value = "Supply Chain";
        worksheet.Cell(2, 4).Value = "Threat";
        worksheet.Cell(2, 5).Value = 3;
        worksheet.Cell(2, 6).Value = 4;
        worksheet.Cell(2, 7).Value = "Supplier bankruptcy or force majeure";
        worksheet.Cell(2, 8).Value = 50000;
        worksheet.Cell(2, 9).Value = 30;
        
        FormatTemplateWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    #endregion

    #region Validation

    public async Task<ValidationResult> ValidateExcelFileAsync(Stream excelStream, string templateType)
    {
        var result = new ValidationResult { IsValid = true };
        
        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var worksheet = workbook.Worksheet(1);
            
            if (worksheet == null)
            {
                result.IsValid = false;
                result.Errors.Add("Excel file must contain at least one worksheet");
                return result;
            }
            
            // Get expected columns based on template type
            var expectedColumns = GetExpectedColumns(templateType);
            
            // Check if all required columns are present
            var headerRow = worksheet.Row(1);
            for (int i = 0; i < expectedColumns.Count; i++)
            {
                var cellValue = headerRow.Cell(i + 1).GetString();
                if (string.IsNullOrEmpty(cellValue))
                {
                    result.IsValid = false;
                    result.Errors.Add($"Missing required column header at position {i + 1}: {expectedColumns[i]}");
                }
                result.Columns.Add(cellValue);
            }
            
            // Count data rows
            result.RowCount = worksheet.RowsUsed().Count() - 1; // Exclude header
            
            if (result.RowCount == 0)
            {
                result.Warnings.Add("Excel file contains no data rows");
            }
            
            // Reset stream position
            excelStream.Position = 0;
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add($"Error reading Excel file: {ex.Message}");
        }
        
        return result;
    }

    #endregion

    #region Helper Methods

    private void FormatTemplateWorksheet(IXLWorksheet worksheet)
    {
        // Format headers
        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRow.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
        
        // Auto-fit columns
        worksheet.Columns().AdjustToContents();
        
        // Add data validation where appropriate
        // For example, for Type columns, we could add dropdown lists
    }

    private async Task<byte[]> SaveWorkbookToBytes(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return await Task.FromResult(stream.ToArray());
    }

    private List<string> GetExpectedColumns(string templateType)
    {
        return templateType.ToLower() switch
        {
            "budget" => new List<string> { "Code", "Name", "Version", "Total Budget", "Contingency Amount", "Management Reserve", "Effective Date", "Comments" },
            "controlaccount" => new List<string> { "Code", "Name", "Type", "Budget", "Start Date", "End Date", "WBS Code", "Description" },
            "costitem" => new List<string> { "Item Code", "Description", "Type", "Category", "Planned Cost", "Unit", "Quantity", "Unit Cost" },
            "commitment" => new List<string> { "Commitment Number", "Description", "Type", "Vendor Name", "Original Amount", "Start Date", "End Date", "Retention Percentage", "Comments" },
            "planningpackage" => new List<string> { "Code", "Name", "Description", "Budget", "Estimated Hours", "Start Date", "End Date", "Priority" },
            "activity" => new List<string> { "Activity Code", "Name", "Description", "Type", "Duration", "Start Date", "End Date", "WBS Code", "Responsible Resource", "Predecessors" },
            "milestone" => new List<string> { "Code", "Name", "Description", "Type", "Category", "Planned Date", "Is Critical", "Deliverable Description", "Acceptance Criteria" },
            "wbs" => new List<string> { "Code", "Name", "Description", "Type", "Parent Code", "Start Date", "End Date", "Budget" },
            "risk" => new List<string> { "Title", "Description", "Category", "Type", "Probability", "Impact", "Trigger Event", "Cost Impact", "Schedule Impact Days" },
            _ => new List<string>()
        };
    }

    #endregion
}