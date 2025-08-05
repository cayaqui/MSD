using Core.DTOs.Common;

namespace Core.DTOs.Configuration.ProjectTypes;

/// <summary>
/// DTO for Project Type configuration
/// </summary>
public class ProjectTypeDto : BaseDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    
    // Configuration settings
    public bool RequiresWBS { get; set; } = true;
    public bool RequiresCBS { get; set; } = true;
    public bool RequiresOBS { get; set; } = true;
    public bool RequiresSchedule { get; set; } = true;
    public bool RequiresBudget { get; set; } = true;
    public bool RequiresRiskManagement { get; set; } = true;
    public bool RequiresDocumentControl { get; set; } = true;
    public bool RequiresChangeManagement { get; set; } = true;
    public bool RequiresQualityControl { get; set; } = false;
    public bool RequiresHSE { get; set; } = false; // Health, Safety, Environment
    
    // Default settings
    public int DefaultDurationUnit { get; set; } = 1; // 1=Days, 2=Weeks, 3=Months
    public string DefaultCurrency { get; set; } = "USD";
    public string DefaultProgressMethod { get; set; } = "Physical";
    public decimal DefaultContingencyPercentage { get; set; } = 10;
    public int DefaultReportingPeriod { get; set; } = 7; // Days
    
    // Workflow settings
    public List<string> ApprovalLevels { get; set; } = new();
    public List<WorkflowStageDto> WorkflowStages { get; set; } = new();
    
    // Templates
    public Guid? WBSTemplateId { get; set; }
    public Guid? CBSTemplateId { get; set; }
    public Guid? OBSTemplateId { get; set; }
    public Guid? RiskRegisterTemplateId { get; set; }
    
    public bool IsActive { get; set; } = true;
    public int ProjectCount { get; set; }
}

public class CreateProjectTypeDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    
    public bool RequiresWBS { get; set; } = true;
    public bool RequiresCBS { get; set; } = true;
    public bool RequiresOBS { get; set; } = true;
    public bool RequiresSchedule { get; set; } = true;
    public bool RequiresBudget { get; set; } = true;
    public bool RequiresRiskManagement { get; set; } = true;
    public bool RequiresDocumentControl { get; set; } = true;
    public bool RequiresChangeManagement { get; set; } = true;
    public bool RequiresQualityControl { get; set; } = false;
    public bool RequiresHSE { get; set; } = false;
    
    public int DefaultDurationUnit { get; set; } = 1;
    public string DefaultCurrency { get; set; } = "USD";
    public string DefaultProgressMethod { get; set; } = "Physical";
    public decimal DefaultContingencyPercentage { get; set; } = 10;
    public int DefaultReportingPeriod { get; set; } = 7;
}

public class UpdateProjectTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    
    public bool RequiresWBS { get; set; }
    public bool RequiresCBS { get; set; }
    public bool RequiresOBS { get; set; }
    public bool RequiresSchedule { get; set; }
    public bool RequiresBudget { get; set; }
    public bool RequiresRiskManagement { get; set; }
    public bool RequiresDocumentControl { get; set; }
    public bool RequiresChangeManagement { get; set; }
    public bool RequiresQualityControl { get; set; }
    public bool RequiresHSE { get; set; }
    
    public int DefaultDurationUnit { get; set; }
    public string DefaultCurrency { get; set; } = string.Empty;
    public string DefaultProgressMethod { get; set; } = string.Empty;
    public decimal DefaultContingencyPercentage { get; set; }
    public int DefaultReportingPeriod { get; set; }
}

public class WorkflowStageDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    public bool RequiresApproval { get; set; }
    public string? ApprovalRole { get; set; }
    public int? DurationDays { get; set; }
    public List<string> RequiredDocuments { get; set; } = new();
    public List<string> Deliverables { get; set; } = new();
}

public class ProjectTypeFilterDto
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public bool? RequiresSchedule { get; set; }
    public bool? RequiresBudget { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}