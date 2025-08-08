using Application.Interfaces.Common;
using ClosedXML.Excel;
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
using Core.DTOs.Contracts.Contract;
using Core.DTOs.Progress.Activities;
using Core.DTOs.Progress.Milestones;
using Core.DTOs.Progress.Schedules;
using Core.DTOs.Projects.WBS;
using Core.DTOs.Risk.Risk;
using Core.DTOs.Documents.Document;
using Core.DTOs.Reports;
using System.Data;
using System.Reflection;

namespace Infrastructure.Services.Excel;

/// <summary>
/// Service for exporting data to Excel format using ClosedXML
/// </summary>
public class ExcelExportService : IExcelExportService
{
    #region Organization Exports

    public async Task<byte[]> ExportCompaniesToExcelAsync(List<CompanyDto> companies, string fileName = "Companies")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Companies");

        // Add headers
        worksheet.Cell(1, 1).Value = "Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Country";
        worksheet.Cell(1, 4).Value = "City";
        worksheet.Cell(1, 5).Value = "Address";
        worksheet.Cell(1, 6).Value = "Phone";
        worksheet.Cell(1, 7).Value = "Email";
        worksheet.Cell(1, 8).Value = "Website";
        worksheet.Cell(1, 9).Value = "Tax ID";
        worksheet.Cell(1, 10).Value = "Active";

        // Add data
        for (int i = 0; i < companies.Count; i++)
        {
            var company = companies[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = company.Code;
            worksheet.Cell(row, 2).Value = company.Name;
            worksheet.Cell(row, 3).Value = company.Country;
            worksheet.Cell(row, 4).Value = company.City;
            worksheet.Cell(row, 5).Value = company.Address;
            worksheet.Cell(row, 6).Value = company.Phone;
            worksheet.Cell(row, 7).Value = company.Email;
            worksheet.Cell(row, 8).Value = company.Website;
            worksheet.Cell(row, 9).Value = company.TaxId;
            worksheet.Cell(row, 10).Value = company.IsActive ? "Yes" : "No";
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportProjectsToExcelAsync(List<ProjectDto> projects, string fileName = "Projects")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Projects");

        // Add headers
        worksheet.Cell(1, 1).Value = "Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Company";
        worksheet.Cell(1, 4).Value = "Type";
        worksheet.Cell(1, 5).Value = "Status";
        worksheet.Cell(1, 6).Value = "Start Date";
        worksheet.Cell(1, 7).Value = "End Date";
        worksheet.Cell(1, 8).Value = "Budget";
        worksheet.Cell(1, 9).Value = "Currency";
        worksheet.Cell(1, 10).Value = "Country";
        worksheet.Cell(1, 11).Value = "City";
        worksheet.Cell(1, 12).Value = "Manager";

        // Add data
        for (int i = 0; i < projects.Count; i++)
        {
            var project = projects[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = project.Code;
            worksheet.Cell(row, 2).Value = project.Name;
            worksheet.Cell(row, 3).Value = project.CompanyName;
            worksheet.Cell(row, 4).Value = project.ProjectType;
            worksheet.Cell(row, 5).Value = project.Status;
            worksheet.Cell(row, 6).Value = project.StartDate;
            worksheet.Cell(row, 7).Value = project.EndDate;
            worksheet.Cell(row, 8).Value = project.Budget;
            worksheet.Cell(row, 9).Value = project.Currency;
            worksheet.Cell(row, 10).Value = project.Country;
            worksheet.Cell(row, 11).Value = project.City;
            worksheet.Cell(row, 12).Value = project.ProjectManagerName;
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    #endregion

    #region Cost Exports

    public async Task<byte[]> ExportBudgetsToExcelAsync(List<BudgetDto> budgets, string fileName = "Budgets")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Budgets");

        // Add headers
        worksheet.Cell(1, 1).Value = "Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Version";
        worksheet.Cell(1, 4).Value = "Status";
        worksheet.Cell(1, 5).Value = "Total Budget";
        worksheet.Cell(1, 6).Value = "Contingency";
        worksheet.Cell(1, 7).Value = "Management Reserve";
        worksheet.Cell(1, 8).Value = "Effective Date";
        worksheet.Cell(1, 9).Value = "Approved By";
        worksheet.Cell(1, 10).Value = "Approval Date";

        // Add data
        for (int i = 0; i < budgets.Count; i++)
        {
            var budget = budgets[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = budget.Code;
            worksheet.Cell(row, 2).Value = budget.Name;
            worksheet.Cell(row, 3).Value = budget.Version;
            worksheet.Cell(row, 4).Value = budget.Status;
            worksheet.Cell(row, 5).Value = budget.TotalBudget;
            worksheet.Cell(row, 6).Value = budget.ContingencyAmount;
            worksheet.Cell(row, 7).Value = budget.ManagementReserve;
            worksheet.Cell(row, 8).Value = budget.EffectiveDate;
            worksheet.Cell(row, 9).Value = budget.ApprovedBy;
            worksheet.Cell(row, 10).Value = budget.ApprovalDate;
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportControlAccountsToExcelAsync(List<ControlAccountDto> controlAccounts, string fileName = "ControlAccounts")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Control Accounts");

        // Add headers
        worksheet.Cell(1, 1).Value = "Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "WBS Element";
        worksheet.Cell(1, 4).Value = "Type";
        worksheet.Cell(1, 5).Value = "Status";
        worksheet.Cell(1, 6).Value = "Budget";
        worksheet.Cell(1, 7).Value = "Actual Cost";
        worksheet.Cell(1, 8).Value = "Committed Cost";
        worksheet.Cell(1, 9).Value = "Forecast Cost";
        worksheet.Cell(1, 10).Value = "Variance";
        worksheet.Cell(1, 11).Value = "CAM";
        worksheet.Cell(1, 12).Value = "Start Date";
        worksheet.Cell(1, 13).Value = "End Date";

        // Add data
        for (int i = 0; i < controlAccounts.Count; i++)
        {
            var ca = controlAccounts[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = ca.Code;
            worksheet.Cell(row, 2).Value = ca.Name;
            worksheet.Cell(row, 3).Value = ca.WBSCode;
            worksheet.Cell(row, 4).Value = ca.Type;
            worksheet.Cell(row, 5).Value = ca.Status;
            worksheet.Cell(row, 6).Value = ca.TotalBudget;
            worksheet.Cell(row, 7).Value = ca.ActualCost;
            worksheet.Cell(row, 8).Value = ca.CommittedCost;
            worksheet.Cell(row, 9).Value = ca.ForecastCost;
            worksheet.Cell(row, 10).Value = ca.Variance;
            worksheet.Cell(row, 11).Value = ca.CAMName;
            worksheet.Cell(row, 12).Value = ca.StartDate;
            worksheet.Cell(row, 13).Value = ca.EndDate;
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportCostItemsToExcelAsync(List<CostItemDto> costItems, string fileName = "CostItems")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Cost Items");

        // Add headers
        worksheet.Cell(1, 1).Value = "Item Code";
        worksheet.Cell(1, 2).Value = "Description";
        worksheet.Cell(1, 3).Value = "Type";
        worksheet.Cell(1, 4).Value = "Category";
        worksheet.Cell(1, 5).Value = "Control Account";
        worksheet.Cell(1, 6).Value = "WBS Element";
        worksheet.Cell(1, 7).Value = "Planned Cost";
        worksheet.Cell(1, 8).Value = "Actual Cost";
        worksheet.Cell(1, 9).Value = "Committed Cost";
        worksheet.Cell(1, 10).Value = "Forecast Cost";
        worksheet.Cell(1, 11).Value = "Variance";
        worksheet.Cell(1, 12).Value = "Status";
        worksheet.Cell(1, 13).Value = "Approved";

        // Add data
        for (int i = 0; i < costItems.Count; i++)
        {
            var item = costItems[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = item.ItemCode;
            worksheet.Cell(row, 2).Value = item.Description;
            worksheet.Cell(row, 3).Value = item.Type;
            worksheet.Cell(row, 4).Value = item.Category;
            worksheet.Cell(row, 5).Value = item.ControlAccountCode;
            worksheet.Cell(row, 6).Value = item.WBSCode;
            worksheet.Cell(row, 7).Value = item.PlannedCost;
            worksheet.Cell(row, 8).Value = item.ActualCost;
            worksheet.Cell(row, 9).Value = item.CommittedCost;
            worksheet.Cell(row, 10).Value = item.ForecastCost;
            worksheet.Cell(row, 11).Value = item.Variance;
            worksheet.Cell(row, 12).Value = item.Status;
            worksheet.Cell(row, 13).Value = item.IsApproved ? "Yes" : "No";
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportCommitmentsToExcelAsync(List<CommitmentDto> commitments, string fileName = "Commitments")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Commitments");

        // Add headers
        worksheet.Cell(1, 1).Value = "Commitment Number";
        worksheet.Cell(1, 2).Value = "Description";
        worksheet.Cell(1, 3).Value = "Type";
        worksheet.Cell(1, 4).Value = "Vendor";
        worksheet.Cell(1, 5).Value = "Control Account";
        worksheet.Cell(1, 6).Value = "Original Amount";
        worksheet.Cell(1, 7).Value = "Current Amount";
        worksheet.Cell(1, 8).Value = "Invoiced Amount";
        worksheet.Cell(1, 9).Value = "Paid Amount";
        worksheet.Cell(1, 10).Value = "Retention";
        worksheet.Cell(1, 11).Value = "Status";
        worksheet.Cell(1, 12).Value = "Start Date";
        worksheet.Cell(1, 13).Value = "End Date";

        // Add data
        for (int i = 0; i < commitments.Count; i++)
        {
            var commitment = commitments[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = commitment.CommitmentNumber;
            worksheet.Cell(row, 2).Value = commitment.Description;
            worksheet.Cell(row, 3).Value = commitment.Type;
            worksheet.Cell(row, 4).Value = commitment.VendorName;
            worksheet.Cell(row, 5).Value = commitment.ControlAccountCode;
            worksheet.Cell(row, 6).Value = commitment.OriginalAmount;
            worksheet.Cell(row, 7).Value = commitment.CurrentAmount;
            worksheet.Cell(row, 8).Value = commitment.InvoicedAmount;
            worksheet.Cell(row, 9).Value = commitment.PaidAmount;
            worksheet.Cell(row, 10).Value = commitment.RetentionAmount;
            worksheet.Cell(row, 11).Value = commitment.Status;
            worksheet.Cell(row, 12).Value = commitment.StartDate;
            worksheet.Cell(row, 13).Value = commitment.EndDate;
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportPlanningPackagesToExcelAsync(List<PlanningPackageDto> planningPackages, string fileName = "PlanningPackages")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Planning Packages");

        // Add headers
        worksheet.Cell(1, 1).Value = "Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Control Account";
        worksheet.Cell(1, 4).Value = "Description";
        worksheet.Cell(1, 5).Value = "Budget";
        worksheet.Cell(1, 6).Value = "Estimated Hours";
        worksheet.Cell(1, 7).Value = "Priority";
        worksheet.Cell(1, 8).Value = "Status";
        worksheet.Cell(1, 9).Value = "Planned Start";
        worksheet.Cell(1, 10).Value = "Planned End";
        worksheet.Cell(1, 11).Value = "Planned Conversion Date";
        worksheet.Cell(1, 12).Value = "Is Converted";

        // Add data
        for (int i = 0; i < planningPackages.Count; i++)
        {
            var pp = planningPackages[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = pp.Code;
            worksheet.Cell(row, 2).Value = pp.Name;
            worksheet.Cell(row, 3).Value = pp.ControlAccountCode;
            worksheet.Cell(row, 4).Value = pp.Description;
            worksheet.Cell(row, 5).Value = pp.EstimatedBudget;
            worksheet.Cell(row, 6).Value = pp.EstimatedHours;
            worksheet.Cell(row, 7).Value = pp.Priority;
            worksheet.Cell(row, 8).Value = pp.Status;
            worksheet.Cell(row, 9).Value = pp.PlannedStartDate;
            worksheet.Cell(row, 10).Value = pp.PlannedEndDate;
            worksheet.Cell(row, 11).Value = pp.PlannedConversionDate;
            worksheet.Cell(row, 12).Value = pp.IsConverted ? "Yes" : "No";
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    #endregion

    #region EVM Exports

    public async Task<byte[]> ExportEVMRecordsToExcelAsync(List<EVMRecordDto> evmRecords, string fileName = "EVMRecords")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("EVM Records");

        // Add headers
        worksheet.Cell(1, 1).Value = "Control Account";
        worksheet.Cell(1, 2).Value = "Data Date";
        worksheet.Cell(1, 3).Value = "Period Type";
        worksheet.Cell(1, 4).Value = "PV";
        worksheet.Cell(1, 5).Value = "EV";
        worksheet.Cell(1, 6).Value = "AC";
        worksheet.Cell(1, 7).Value = "BAC";
        worksheet.Cell(1, 8).Value = "CV";
        worksheet.Cell(1, 9).Value = "SV";
        worksheet.Cell(1, 10).Value = "CPI";
        worksheet.Cell(1, 11).Value = "SPI";
        worksheet.Cell(1, 12).Value = "EAC";
        worksheet.Cell(1, 13).Value = "VAC";
        worksheet.Cell(1, 14).Value = "Status";

        // Add data
        for (int i = 0; i < evmRecords.Count; i++)
        {
            var record = evmRecords[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = record.ControlAccountCode;
            worksheet.Cell(row, 2).Value = record.DataDate;
            worksheet.Cell(row, 3).Value = record.PeriodType;
            worksheet.Cell(row, 4).Value = record.PV;
            worksheet.Cell(row, 5).Value = record.EV;
            worksheet.Cell(row, 6).Value = record.AC;
            worksheet.Cell(row, 7).Value = record.BAC;
            worksheet.Cell(row, 8).Value = record.CV;
            worksheet.Cell(row, 9).Value = record.SV;
            worksheet.Cell(row, 10).Value = record.CPI;
            worksheet.Cell(row, 11).Value = record.SPI;
            worksheet.Cell(row, 12).Value = record.EAC;
            worksheet.Cell(row, 13).Value = record.VAC;
            worksheet.Cell(row, 14).Value = record.Status;
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportNineColumnReportToExcelAsync(NineColumnReportDto report, string fileName = "NineColumnReport")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Nine Column Report");

        // Add report header
        worksheet.Cell(1, 1).Value = "Nine Column Report";
        worksheet.Cell(2, 1).Value = "Project:";
        worksheet.Cell(2, 2).Value = report.ProjectName;
        worksheet.Cell(3, 1).Value = "Report Date:";
        worksheet.Cell(3, 2).Value = report.ReportDate;
        worksheet.Cell(4, 1).Value = "Currency:";
        worksheet.Cell(4, 2).Value = report.Currency;

        // Add column headers
        var headerRow = 6;
        worksheet.Cell(headerRow, 1).Value = "Code";
        worksheet.Cell(headerRow, 2).Value = "Activity Description";
        worksheet.Cell(headerRow, 3).Value = "Planned Value";
        worksheet.Cell(headerRow, 4).Value = "Physical Progress %";
        worksheet.Cell(headerRow, 5).Value = "Earned Value";
        worksheet.Cell(headerRow, 6).Value = "Actual Cost";
        worksheet.Cell(headerRow, 7).Value = "Cost Variance";
        worksheet.Cell(headerRow, 8).Value = "Schedule Variance";
        worksheet.Cell(headerRow, 9).Value = "CPI";
        worksheet.Cell(headerRow, 10).Value = "EAC";

        // Add data
        var dataRow = headerRow + 1;
        foreach (var line in report.Lines)
        {
            worksheet.Cell(dataRow, 1).Value = line.Code;
            worksheet.Cell(dataRow, 2).Value = new string(' ', (line.Level - 1) * 3) + line.ActivityDescription;
            worksheet.Cell(dataRow, 3).Value = line.PlannedValue;
            worksheet.Cell(dataRow, 4).Value = line.PhysicalProgressPercentage;
            worksheet.Cell(dataRow, 5).Value = line.EarnedValue;
            worksheet.Cell(dataRow, 6).Value = line.ActualCost;
            worksheet.Cell(dataRow, 7).Value = line.CostVariance;
            worksheet.Cell(dataRow, 8).Value = line.ScheduleVariance;
            worksheet.Cell(dataRow, 9).Value = line.CostPerformanceIndex;
            worksheet.Cell(dataRow, 10).Value = line.EstimateAtCompletion;
            dataRow++;
        }

        // Add totals
        dataRow++;
        worksheet.Cell(dataRow, 2).Value = "TOTALS";
        worksheet.Cell(dataRow, 3).Value = report.Totals.TotalPlannedValue;
        worksheet.Cell(dataRow, 4).Value = report.Totals.OverallPhysicalProgress;
        worksheet.Cell(dataRow, 5).Value = report.Totals.TotalEarnedValue;
        worksheet.Cell(dataRow, 6).Value = report.Totals.TotalActualCost;
        worksheet.Cell(dataRow, 7).Value = report.Totals.TotalCostVariance;
        worksheet.Cell(dataRow, 8).Value = report.Totals.TotalScheduleVariance;
        worksheet.Cell(dataRow, 9).Value = report.Totals.OverallCPI;
        worksheet.Cell(dataRow, 10).Value = report.Totals.TotalEstimateAtCompletion;

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    #endregion

    #region Progress Exports

    public async Task<byte[]> ExportActivitiesToExcelAsync(List<ActivityDto> activities, string fileName = "Activities")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Activities");

        // Add headers
        worksheet.Cell(1, 1).Value = "Activity Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "WBS Element";
        worksheet.Cell(1, 4).Value = "Type";
        worksheet.Cell(1, 5).Value = "Status";
        worksheet.Cell(1, 6).Value = "Duration";
        worksheet.Cell(1, 7).Value = "Planned Start";
        worksheet.Cell(1, 8).Value = "Planned Finish";
        worksheet.Cell(1, 9).Value = "Actual Start";
        worksheet.Cell(1, 10).Value = "Actual Finish";
        worksheet.Cell(1, 11).Value = "Progress %";
        worksheet.Cell(1, 12).Value = "Resource";
        worksheet.Cell(1, 13).Value = "Predecessor";

        // Add data
        for (int i = 0; i < activities.Count; i++)
        {
            var activity = activities[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = activity.ActivityCode;
            worksheet.Cell(row, 2).Value = activity.Name;
            worksheet.Cell(row, 3).Value = activity.WBSCode;
            worksheet.Cell(row, 4).Value = activity.ActivityType;
            worksheet.Cell(row, 5).Value = activity.Status;
            worksheet.Cell(row, 6).Value = activity.Duration;
            worksheet.Cell(row, 7).Value = activity.PlannedStartDate;
            worksheet.Cell(row, 8).Value = activity.PlannedEndDate;
            worksheet.Cell(row, 9).Value = activity.ActualStartDate;
            worksheet.Cell(row, 10).Value = activity.ActualEndDate;
            worksheet.Cell(row, 11).Value = activity.ProgressPercentage;
            worksheet.Cell(row, 12).Value = activity.ResponsibleResource;
            worksheet.Cell(row, 13).Value = string.Join(", ", activity.Predecessors?.Select(p => p.PredecessorCode) ?? new List<string>());
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportMilestonesToExcelAsync(List<MilestoneDto> milestones, string fileName = "Milestones")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Milestones");

        // Add headers
        worksheet.Cell(1, 1).Value = "Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Type";
        worksheet.Cell(1, 4).Value = "Category";
        worksheet.Cell(1, 5).Value = "Status";
        worksheet.Cell(1, 6).Value = "Planned Date";
        worksheet.Cell(1, 7).Value = "Actual Date";
        worksheet.Cell(1, 8).Value = "Days Variance";
        worksheet.Cell(1, 9).Value = "Progress %";
        worksheet.Cell(1, 10).Value = "Responsible";
        worksheet.Cell(1, 11).Value = "Is Critical";

        // Add data
        for (int i = 0; i < milestones.Count; i++)
        {
            var milestone = milestones[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = milestone.Code;
            worksheet.Cell(row, 2).Value = milestone.Name;
            worksheet.Cell(row, 3).Value = milestone.Type;
            worksheet.Cell(row, 4).Value = milestone.Category;
            worksheet.Cell(row, 5).Value = milestone.Status;
            worksheet.Cell(row, 6).Value = milestone.PlannedDate;
            worksheet.Cell(row, 7).Value = milestone.ActualDate;
            worksheet.Cell(row, 8).Value = milestone.DaysVariance;
            worksheet.Cell(row, 9).Value = milestone.ProgressPercentage;
            worksheet.Cell(row, 10).Value = milestone.ResponsibleUserName;
            worksheet.Cell(row, 11).Value = milestone.IsCritical ? "Yes" : "No";
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportScheduleToExcelAsync(ScheduleDto schedule, string fileName = "Schedule")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Schedule");

        // Add schedule info
        worksheet.Cell(1, 1).Value = "Schedule Information";
        worksheet.Cell(2, 1).Value = "Name:";
        worksheet.Cell(2, 2).Value = schedule.Name;
        worksheet.Cell(3, 1).Value = "Version:";
        worksheet.Cell(3, 2).Value = schedule.Version;
        worksheet.Cell(4, 1).Value = "Status:";
        worksheet.Cell(4, 2).Value = schedule.Status;
        worksheet.Cell(5, 1).Value = "Start Date:";
        worksheet.Cell(5, 2).Value = schedule.StartDate;
        worksheet.Cell(6, 1).Value = "End Date:";
        worksheet.Cell(6, 2).Value = schedule.EndDate;

        // Add activities
        if (schedule.Activities?.Any() == true)
        {
            worksheet.Cell(8, 1).Value = "Activities";
            var activitySheet = workbook.Worksheets.Add("Activities");
            await PopulateActivitySheet(activitySheet, schedule.Activities.ToList());
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    #endregion

    #region WBS Exports

    public async Task<byte[]> ExportWBSToExcelAsync(List<WBSElementDto> wbsElements, string fileName = "WBS")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("WBS");

        // Add headers
        worksheet.Cell(1, 1).Value = "Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Parent Code";
        worksheet.Cell(1, 4).Value = "Level";
        worksheet.Cell(1, 5).Value = "Type";
        worksheet.Cell(1, 6).Value = "Status";
        worksheet.Cell(1, 7).Value = "Start Date";
        worksheet.Cell(1, 8).Value = "End Date";
        worksheet.Cell(1, 9).Value = "Budget";
        worksheet.Cell(1, 10).Value = "Actual Cost";
        worksheet.Cell(1, 11).Value = "Progress %";
        worksheet.Cell(1, 12).Value = "Responsible";

        // Add data
        for (int i = 0; i < wbsElements.Count; i++)
        {
            var wbs = wbsElements[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = wbs.Code;
            worksheet.Cell(row, 2).Value = new string(' ', (wbs.Level - 1) * 3) + wbs.Name;
            worksheet.Cell(row, 3).Value = wbs.ParentCode;
            worksheet.Cell(row, 4).Value = wbs.Level;
            worksheet.Cell(row, 5).Value = wbs.Type;
            worksheet.Cell(row, 6).Value = wbs.Status;
            worksheet.Cell(row, 7).Value = wbs.StartDate;
            worksheet.Cell(row, 8).Value = wbs.EndDate;
            worksheet.Cell(row, 9).Value = wbs.Budget;
            worksheet.Cell(row, 10).Value = wbs.ActualCost;
            worksheet.Cell(row, 11).Value = wbs.ProgressPercentage;
            worksheet.Cell(row, 12).Value = wbs.ResponsibleUserName;
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportWBSHierarchyToExcelAsync(WBSHierarchyDto hierarchy, string fileName = "WBSHierarchy")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("WBS Hierarchy");

        // Flatten hierarchy
        var flatList = new List<WBSElementDto>();
        FlattenWBSHierarchy(hierarchy.RootElement, flatList);

        return await ExportWBSToExcelAsync(flatList, fileName);
    }

    #endregion

    #region Contract Exports

    public async Task<byte[]> ExportContractsToExcelAsync(List<ContractDto> contracts, string fileName = "Contracts")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Contracts");

        // Add headers
        worksheet.Cell(1, 1).Value = "Contract Number";
        worksheet.Cell(1, 2).Value = "Title";
        worksheet.Cell(1, 3).Value = "Type";
        worksheet.Cell(1, 4).Value = "Contractor";
        worksheet.Cell(1, 5).Value = "Status";
        worksheet.Cell(1, 6).Value = "Original Value";
        worksheet.Cell(1, 7).Value = "Current Value";
        worksheet.Cell(1, 8).Value = "Start Date";
        worksheet.Cell(1, 9).Value = "End Date";
        worksheet.Cell(1, 10).Value = "Completion %";
        worksheet.Cell(1, 11).Value = "Payment Terms";
        worksheet.Cell(1, 12).Value = "Manager";

        // Add data
        for (int i = 0; i < contracts.Count; i++)
        {
            var contract = contracts[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = contract.ContractNumber;
            worksheet.Cell(row, 2).Value = contract.Title;
            worksheet.Cell(row, 3).Value = contract.Type;
            worksheet.Cell(row, 4).Value = contract.ContractorName;
            worksheet.Cell(row, 5).Value = contract.Status;
            worksheet.Cell(row, 6).Value = contract.OriginalValue;
            worksheet.Cell(row, 7).Value = contract.CurrentValue;
            worksheet.Cell(row, 8).Value = contract.StartDate;
            worksheet.Cell(row, 9).Value = contract.EndDate;
            worksheet.Cell(row, 10).Value = contract.CompletionPercentage;
            worksheet.Cell(row, 11).Value = contract.PaymentTerms;
            worksheet.Cell(row, 12).Value = contract.ContractManagerName;
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    #endregion

    #region Risk Exports

    public async Task<byte[]> ExportRiskRegisterToExcelAsync(RiskRegisterDto riskRegister, string fileName = "RiskRegister")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Risk Register");

        // Add summary
        worksheet.Cell(1, 1).Value = "Risk Register Summary";
        worksheet.Cell(2, 1).Value = "Total Risks:";
        worksheet.Cell(2, 2).Value = riskRegister.TotalRisks;
        worksheet.Cell(3, 1).Value = "Open Risks:";
        worksheet.Cell(3, 2).Value = riskRegister.OpenRisks;
        worksheet.Cell(4, 1).Value = "High Risks:";
        worksheet.Cell(4, 2).Value = riskRegister.HighRisks;

        // Add risk details
        var headerRow = 6;
        worksheet.Cell(headerRow, 1).Value = "Risk Code";
        worksheet.Cell(headerRow, 2).Value = "Title";
        worksheet.Cell(headerRow, 3).Value = "Category";
        worksheet.Cell(headerRow, 4).Value = "Type";
        worksheet.Cell(headerRow, 5).Value = "Status";
        worksheet.Cell(headerRow, 6).Value = "Probability";
        worksheet.Cell(headerRow, 7).Value = "Impact";
        worksheet.Cell(headerRow, 8).Value = "Risk Score";
        worksheet.Cell(headerRow, 9).Value = "Cost Impact";
        worksheet.Cell(headerRow, 10).Value = "Schedule Impact";
        worksheet.Cell(headerRow, 11).Value = "Owner";
        worksheet.Cell(headerRow, 12).Value = "Identified Date";

        // Add data
        for (int i = 0; i < riskRegister.Risks.Count; i++)
        {
            var risk = riskRegister.Risks[i];
            var row = i + headerRow + 1;
            worksheet.Cell(row, 1).Value = risk.Code;
            worksheet.Cell(row, 2).Value = risk.Title;
            worksheet.Cell(row, 3).Value = risk.Category;
            worksheet.Cell(row, 4).Value = risk.Type;
            worksheet.Cell(row, 5).Value = risk.Status;
            worksheet.Cell(row, 6).Value = risk.Probability;
            worksheet.Cell(row, 7).Value = risk.Impact;
            worksheet.Cell(row, 8).Value = risk.RiskScore;
            worksheet.Cell(row, 9).Value = risk.CostImpact;
            worksheet.Cell(row, 10).Value = risk.ScheduleImpact;
            worksheet.Cell(row, 11).Value = risk.ResponseOwnerName;
            worksheet.Cell(row, 12).Value = risk.IdentifiedDate;
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportRiskMatrixToExcelAsync(RiskMatrixDto riskMatrix, string fileName = "RiskMatrix")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Risk Matrix");

        // Add title
        worksheet.Cell(1, 1).Value = "Risk Matrix";
        worksheet.Cell(2, 1).Value = "Generated Date:";
        worksheet.Cell(2, 2).Value = riskMatrix.GeneratedDate;

        // Create matrix
        var startRow = 4;
        var startCol = 2;

        // Add impact headers
        for (int i = 1; i <= 5; i++)
        {
            worksheet.Cell(startRow, startCol + i).Value = $"Impact {i}";
        }

        // Add probability rows
        for (int prob = 5; prob >= 1; prob--)
        {
            worksheet.Cell(startRow + (6 - prob), startCol).Value = $"Probability {prob}";
            
            for (int impact = 1; impact <= 5; impact++)
            {
                var cell = riskMatrix.MatrixCells.FirstOrDefault(c => c.Probability == prob && c.Impact == impact);
                if (cell != null)
                {
                    var excelCell = worksheet.Cell(startRow + (6 - prob), startCol + impact);
                    excelCell.Value = cell.RiskCount;
                    // Apply color based on risk score
                    var riskScore = prob * impact;
                    if (riskScore >= 20) excelCell.Style.Fill.BackgroundColor = XLColor.Red;
                    else if (riskScore >= 15) excelCell.Style.Fill.BackgroundColor = XLColor.Orange;
                    else if (riskScore >= 10) excelCell.Style.Fill.BackgroundColor = XLColor.Yellow;
                    else excelCell.Style.Fill.BackgroundColor = XLColor.Green;
                }
            }
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    #endregion

    #region Document Exports

    public async Task<byte[]> ExportDocumentsToExcelAsync(List<DocumentDto> documents, string fileName = "Documents")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Documents");

        // Add headers
        worksheet.Cell(1, 1).Value = "Document Number";
        worksheet.Cell(1, 2).Value = "Title";
        worksheet.Cell(1, 3).Value = "Category";
        worksheet.Cell(1, 4).Value = "Type";
        worksheet.Cell(1, 5).Value = "Version";
        worksheet.Cell(1, 6).Value = "Status";
        worksheet.Cell(1, 7).Value = "File Size";
        worksheet.Cell(1, 8).Value = "Uploaded By";
        worksheet.Cell(1, 9).Value = "Upload Date";
        worksheet.Cell(1, 10).Value = "WBS Element";
        worksheet.Cell(1, 11).Value = "Discipline";
        worksheet.Cell(1, 12).Value = "Description";

        // Add data
        for (int i = 0; i < documents.Count; i++)
        {
            var doc = documents[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = doc.DocumentNumber;
            worksheet.Cell(row, 2).Value = doc.Title;
            worksheet.Cell(row, 3).Value = doc.Category;
            worksheet.Cell(row, 4).Value = doc.DocumentType;
            worksheet.Cell(row, 5).Value = doc.Version;
            worksheet.Cell(row, 6).Value = doc.Status;
            worksheet.Cell(row, 7).Value = FormatFileSize(doc.FileSize);
            worksheet.Cell(row, 8).Value = doc.UploadedByName;
            worksheet.Cell(row, 9).Value = doc.UploadDate;
            worksheet.Cell(row, 10).Value = doc.WBSCode;
            worksheet.Cell(row, 11).Value = doc.DisciplineName;
            worksheet.Cell(row, 12).Value = doc.Description;
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    #endregion

    #region Generic Export

    public async Task<byte[]> ExportToExcelAsync<T>(List<T> data, string fileName = "Export", Dictionary<string, string>? columnMappings = null) where T : class
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(fileName);

        if (!data.Any())
        {
            worksheet.Cell(1, 1).Value = "No data to export";
            return await SaveWorkbookToBytes(workbook);
        }

        // Get properties
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        // Add headers
        var col = 1;
        foreach (var prop in properties)
        {
            var headerName = columnMappings?.ContainsKey(prop.Name) == true 
                ? columnMappings[prop.Name] 
                : prop.Name;
            worksheet.Cell(1, col).Value = headerName;
            col++;
        }

        // Add data
        for (int i = 0; i < data.Count; i++)
        {
            col = 1;
            foreach (var prop in properties)
            {
                var value = prop.GetValue(data[i]);
                worksheet.Cell(i + 2, col).Value = value?.ToString() ?? string.Empty;
                col++;
            }
        }

        FormatWorksheet(worksheet);
        return await SaveWorkbookToBytes(workbook);
    }

    public async Task<byte[]> ExportMultipleSheets(Dictionary<string, object> sheets, string fileName = "Export")
    {
        using var workbook = new XLWorkbook();

        foreach (var sheet in sheets)
        {
            var worksheet = workbook.Worksheets.Add(sheet.Key);
            
            if (sheet.Value is DataTable dataTable)
            {
                worksheet.Cell(1, 1).InsertTable(dataTable);
            }
            else
            {
                // Use reflection to handle generic lists
                var listType = sheet.Value.GetType();
                if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var method = GetType().GetMethod(nameof(PopulateWorksheetFromList), BindingFlags.NonPublic | BindingFlags.Instance);
                    var genericMethod = method!.MakeGenericMethod(listType.GetGenericArguments()[0]);
                    genericMethod.Invoke(this, new[] { worksheet, sheet.Value });
                }
            }
            
            FormatWorksheet(worksheet);
        }

        return await SaveWorkbookToBytes(workbook);
    }

    #endregion

    #region Helper Methods

    private void FormatWorksheet(IXLWorksheet worksheet)
    {
        // Auto-fit columns
        worksheet.Columns().AdjustToContents();
        
        // Format headers
        var headerRange = worksheet.Range(1, 1, 1, worksheet.LastCellUsed().Address.ColumnNumber);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        
        // Add borders
        var dataRange = worksheet.RangeUsed();
        if (dataRange != null)
        {
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }
    }

    private async Task<byte[]> SaveWorkbookToBytes(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return await Task.FromResult(stream.ToArray());
    }

    private void PopulateWorksheetFromList<T>(IXLWorksheet worksheet, List<T> data) where T : class
    {
        if (!data.Any()) return;

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        // Add headers
        var col = 1;
        foreach (var prop in properties)
        {
            worksheet.Cell(1, col).Value = prop.Name;
            col++;
        }

        // Add data
        for (int i = 0; i < data.Count; i++)
        {
            col = 1;
            foreach (var prop in properties)
            {
                var value = prop.GetValue(data[i]);
                worksheet.Cell(i + 2, col).Value = value?.ToString() ?? string.Empty;
                col++;
            }
        }
    }

    private async Task PopulateActivitySheet(IXLWorksheet worksheet, List<ActivityDto> activities)
    {
        // Add headers
        worksheet.Cell(1, 1).Value = "Activity Code";
        worksheet.Cell(1, 2).Value = "Name";
        worksheet.Cell(1, 3).Value = "Duration";
        worksheet.Cell(1, 4).Value = "Start Date";
        worksheet.Cell(1, 5).Value = "End Date";
        worksheet.Cell(1, 6).Value = "Progress %";

        // Add data
        for (int i = 0; i < activities.Count; i++)
        {
            var activity = activities[i];
            var row = i + 2;
            worksheet.Cell(row, 1).Value = activity.ActivityCode;
            worksheet.Cell(row, 2).Value = activity.Name;
            worksheet.Cell(row, 3).Value = activity.Duration;
            worksheet.Cell(row, 4).Value = activity.PlannedStartDate;
            worksheet.Cell(row, 5).Value = activity.PlannedEndDate;
            worksheet.Cell(row, 6).Value = activity.ProgressPercentage;
        }

        FormatWorksheet(worksheet);
    }

    private void FlattenWBSHierarchy(WBSElementDto element, List<WBSElementDto> flatList)
    {
        flatList.Add(element);
        if (element.Children != null)
        {
            foreach (var child in element.Children)
            {
                FlattenWBSHierarchy(child, flatList);
            }
        }
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    #endregion
}