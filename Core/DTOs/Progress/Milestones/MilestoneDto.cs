using Core.DTOs.Common;
using Core.Enums;
using Core.Enums.Projects;

namespace Core.DTOs.Progress.Milestones;

public class MilestoneDto : BaseDto
{
    public string MilestoneCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Classification
    public Guid ProjectId { get; set; }
    public Guid? PhaseId { get; set; }
    public Guid? WorkPackageId { get; set; }
    public MilestoneType Type { get; set; }
    public string TypeDisplay => Type switch
    {
        MilestoneType.ProjectStart => "Inicio del Proyecto",
        MilestoneType.ProjectEnd => "Fin del Proyecto",
        MilestoneType.PhaseGate => "Puerta de Fase",
        MilestoneType.KeyDeliverable => "Entregable Clave",
        MilestoneType.ContractMilestone => "Hito Contractual",
        MilestoneType.PaymentMilestone => "Hito de Pago",
        MilestoneType.Other => "Otro",
        _ => Type.ToString()
    };
    public bool IsCritical { get; set; }
    public bool IsContractual { get; set; }
    
    // Schedule
    public DateTime PlannedDate { get; set; }
    public DateTime? ForecastDate { get; set; }
    public DateTime? ActualDate { get; set; }
    public int? DaysVariance { get; set; }
    
    // Status
    public bool IsCompleted { get; set; }
    public decimal CompletionPercentage { get; set; }
    public string? CompletionCriteria { get; set; }
    
    // Financial
    public decimal? PaymentAmount { get; set; }
    public string? PaymentCurrency { get; set; }
    public bool IsPaymentTriggered { get; set; }
    
    // Dependencies
    public string[]? PredecessorMilestones { get; set; }
    public string[]? SuccessorMilestones { get; set; }
    
    // Approval
    public bool RequiresApproval { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? ApprovedBy { get; set; }
    
    // Documentation
    public string? Deliverables { get; set; }
    public string? AcceptanceCriteria { get; set; }
    
    // Related Info
    public string ProjectName { get; set; } = string.Empty;
    public string? PhaseName { get; set; }
    public string? WorkPackageName { get; set; }
}

public class CreateMilestoneDto
{
    public string MilestoneCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? PhaseId { get; set; }
    public Guid? WorkPackageId { get; set; }
    public MilestoneType Type { get; set; }
    public bool IsCritical { get; set; }
    public bool IsContractual { get; set; }
    public DateTime PlannedDate { get; set; }
    public decimal? PaymentAmount { get; set; }
    public string? PaymentCurrency { get; set; }
    public string? CompletionCriteria { get; set; }
    public string? AcceptanceCriteria { get; set; }
    public string[]? PredecessorMilestones { get; set; }
    public string[]? SuccessorMilestones { get; set; }
}

public class UpdateMilestoneDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCritical { get; set; }
    public DateTime? ForecastDate { get; set; }
    public Guid? PhaseId { get; set; }
    public Guid? WorkPackageId { get; set; }
    public decimal? PaymentAmount { get; set; }
    public string? PaymentCurrency { get; set; }
    public string? CompletionCriteria { get; set; }
    public string? AcceptanceCriteria { get; set; }
    public string[]? PredecessorMilestones { get; set; }
    public string[]? SuccessorMilestones { get; set; }
}

public class UpdateMilestoneProgressDto
{
    public decimal CompletionPercentage { get; set; }
    public DateTime? ForecastDate { get; set; }
    public string? Notes { get; set; }
}

public class CompleteMilestoneDto
{
    public DateTime? ActualDate { get; set; }
    public string? CompletionNotes { get; set; }
    public bool TriggerPayment { get; set; }
}

public class ApproveMilestoneDto
{
    public string? Comments { get; set; }
}

public class MilestoneFilterDto : BaseFilterDto
{
    public Guid? ProjectId { get; set; }
    public Guid? PhaseId { get; set; }
    public MilestoneType? Type { get; set; }
    public bool? IsCritical { get; set; }
    public bool? IsContractual { get; set; }
    public bool? IsCompleted { get; set; }
    public DateTime? PlannedDateFrom { get; set; }
    public DateTime? PlannedDateTo { get; set; }
    public bool? HasPayment { get; set; }
}

public class MilestoneDashboardDto
{
    public Guid ProjectId { get; set; }
    public int TotalMilestones { get; set; }
    public int CompletedMilestones { get; set; }
    public int UpcomingMilestones { get; set; }
    public int OverdueMilestones { get; set; }
    public int CriticalMilestones { get; set; }
    public int ContractualMilestones { get; set; }
    public decimal CompletionPercentage { get; set; }
    public MilestoneDto? NextMilestone { get; set; }
    public List<MilestoneDto> RecentlyCompleted { get; set; } = new();
    public List<MilestoneDto> UpcomingList { get; set; } = new();
}