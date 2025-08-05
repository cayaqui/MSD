using Domain.Common;
using Domain.Entities.Organization.Core;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Domain.Entities.Configuration.Core;

/// <summary>
/// Represents a project type configuration
/// </summary>
public class ProjectType : BaseAuditableEntity, ISoftDelete
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Icon { get; private set; }
    public string? Color { get; private set; }

    // Module requirements
    public bool RequiresWBS { get; private set; } = true;
    public bool RequiresCBS { get; private set; } = true;
    public bool RequiresOBS { get; private set; } = true;
    public bool RequiresSchedule { get; private set; } = true;
    public bool RequiresBudget { get; private set; } = true;
    public bool RequiresRiskManagement { get; private set; } = true;
    public bool RequiresDocumentControl { get; private set; } = true;
    public bool RequiresChangeManagement { get; private set; } = true;
    public bool RequiresQualityControl { get; private set; } = false;
    public bool RequiresHSE { get; private set; } = false;

    // Default settings
    public int DefaultDurationUnit { get; private set; } = 1; // 1=Days, 2=Weeks, 3=Months
    public string DefaultCurrency { get; private set; } = "USD";
    public string DefaultProgressMethod { get; private set; } = "Physical";
    public decimal DefaultContingencyPercentage { get; private set; } = 10;
    public int DefaultReportingPeriod { get; private set; } = 7; // Days

    // Workflow settings (stored as JSON)
    public string? ApprovalLevelsJson { get; private set; }
    public string? WorkflowStagesJson { get; private set; }

    // Templates
    public Guid? WBSTemplateId { get; private set; }
    public Guid? CBSTemplateId { get; private set; }
    public Guid? OBSTemplateId { get; private set; }
    public Guid? RiskRegisterTemplateId { get; private set; }

    // Status
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual ICollection<Project> Projects { get; private set; } =
        new List<Project>();

    private ProjectType() { } // EF Core

    public ProjectType(string code, string name)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void UpdateBasicInfo(string name, string? description, string? icon, string? color)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Icon = icon;
        Color = color;
    }

    public void UpdateModuleRequirements(
        bool requiresWBS,
        bool requiresCBS,
        bool requiresOBS,
        bool requiresSchedule,
        bool requiresBudget,
        bool requiresRiskManagement,
        bool requiresDocumentControl,
        bool requiresChangeManagement,
        bool requiresQualityControl,
        bool requiresHSE
    )
    {
        RequiresWBS = requiresWBS;
        RequiresCBS = requiresCBS;
        RequiresOBS = requiresOBS;
        RequiresSchedule = requiresSchedule;
        RequiresBudget = requiresBudget;
        RequiresRiskManagement = requiresRiskManagement;
        RequiresDocumentControl = requiresDocumentControl;
        RequiresChangeManagement = requiresChangeManagement;
        RequiresQualityControl = requiresQualityControl;
        RequiresHSE = requiresHSE;
    }

    public void UpdateDefaultSettings(
        int defaultDurationUnit,
        string defaultCurrency,
        string defaultProgressMethod,
        decimal defaultContingencyPercentage,
        int defaultReportingPeriod
    )
    {
        if (defaultDurationUnit < 1 || defaultDurationUnit > 3)
            throw new ArgumentException("Invalid duration unit");

        if (defaultContingencyPercentage < 0 || defaultContingencyPercentage > 100)
            throw new ArgumentException("Contingency percentage must be between 0 and 100");

        if (defaultReportingPeriod < 1)
            throw new ArgumentException("Reporting period must be at least 1 day");

        DefaultDurationUnit = defaultDurationUnit;
        DefaultCurrency =
            defaultCurrency ?? throw new ArgumentNullException(nameof(defaultCurrency));
        DefaultProgressMethod =
            defaultProgressMethod ?? throw new ArgumentNullException(nameof(defaultProgressMethod));
        DefaultContingencyPercentage = defaultContingencyPercentage;
        DefaultReportingPeriod = defaultReportingPeriod;
    }

    public void SetApprovalLevels(List<string> approvalLevels)
    {
        ApprovalLevelsJson =
            approvalLevels?.Any() == true ? JsonSerializer.Serialize(approvalLevels) : null;
    }

    public List<string> GetApprovalLevels()
    {
        return string.IsNullOrEmpty(ApprovalLevelsJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(ApprovalLevelsJson) ?? new List<string>();
    }

    public void SetWorkflowStages<T>(List<T> stages)
    {
        WorkflowStagesJson = stages?.Any() == true ? JsonSerializer.Serialize(stages) : null;
    }

    public void SetTemplates(
        Guid? wbsTemplateId,
        Guid? cbsTemplateId,
        Guid? obsTemplateId,
        Guid? riskRegisterTemplateId
    )
    {
        WBSTemplateId = wbsTemplateId;
        CBSTemplateId = cbsTemplateId;
        OBSTemplateId = obsTemplateId;
        RiskRegisterTemplateId = riskRegisterTemplateId;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public static class DurationUnits
    {
        public const int Hours = 1;
        public const int Days = 2;
        public const int Weeks = 3;
        public const int Months = 4;
    }

    public static class ProgressMethods
    {
        public const string Physical = "Physical";
        public const string Duration = "Duration";
        public const string Units = "Units";
        public const string Cost = "Cost";
        public const string Milestone = "Milestone";
    }
}
