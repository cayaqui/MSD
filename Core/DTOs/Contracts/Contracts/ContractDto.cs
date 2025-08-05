using Core.DTOs.Common;
using Core.DTOs.Organization.Company;
using Core.DTOs.Organization.Project;
using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Contracts;

public class ContractDto : BaseDto
{
    public string ContractNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Contract Type and Status
    public ContractType Type { get; set; }
    public ContractStatus Status { get; set; }
    public ContractCategory Category { get; set; }
    public string SubCategory { get; set; } = string.Empty;
    
    // Project Information
    public Guid ProjectId { get; set; }
    public ProjectDto? Project { get; set; }
    
    // Contractor Information
    public Guid ContractorId { get; set; }
    public CompanyDto? Contractor { get; set; }
    public string ContractorReference { get; set; } = string.Empty;
    
    // Contract Value
    public decimal OriginalValue { get; set; }
    public decimal CurrentValue { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal ExchangeRate { get; set; } = 1.0m;
    
    // Dates
    public DateTime ContractDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime OriginalEndDate { get; set; }
    public DateTime CurrentEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    
    // Performance
    public decimal PercentageComplete { get; set; }
    public decimal AmountInvoiced { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal RetentionAmount { get; set; }
    public decimal RetentionPercentage { get; set; }
    
    // Payment Terms
    public PaymentTerms PaymentTerms { get; set; }
    public int PaymentDays { get; set; }
    public string PaymentSchedule { get; set; } = string.Empty;
    
    // Bonds and Insurance
    public bool RequiresPerformanceBond { get; set; }
    public decimal? PerformanceBondAmount { get; set; }
    public DateTime? PerformanceBondExpiry { get; set; }
    public bool RequiresPaymentBond { get; set; }
    public decimal? PaymentBondAmount { get; set; }
    public DateTime? PaymentBondExpiry { get; set; }
    public string InsuranceRequirements { get; set; } = string.Empty;
    
    // Change Orders
    public int ChangeOrderCount { get; set; }
    public decimal ChangeOrderValue { get; set; }
    public decimal PendingChangeOrderValue { get; set; }
    
    // Milestones and Deliverables
    public int TotalMilestones { get; set; }
    public int CompletedMilestones { get; set; }
    public DateTime? NextMilestoneDate { get; set; }
    public string NextMilestoneName { get; set; } = string.Empty;
    
    // Risk and Issues
    public ContractRiskLevel RiskLevel { get; set; }
    public int OpenIssuesCount { get; set; }
    public int OpenClaimsCount { get; set; }
    public decimal TotalClaimsValue { get; set; }
    
    // Documents
    public string ContractDocumentUrl { get; set; } = string.Empty;
    public int AttachmentCount { get; set; }
    public DateTime? LastDocumentUpdate { get; set; }
    
    // Approval
    public bool IsApproved { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
    public DateTime? ApprovalDate { get; set; }
    public string ApprovalComments { get; set; } = string.Empty;
    
    // Additional Information
    public string Scope { get; set; } = string.Empty;
    public string Exclusions { get; set; } = string.Empty;
    public string SpecialConditions { get; set; } = string.Empty;
    public string PenaltyClauses { get; set; } = string.Empty;
    public string TerminationClauses { get; set; } = string.Empty;
    
    // Audit
    public bool IsActive { get; set; } = true;
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();

    public void UpdateCurrentValue()
    {
        // This method should calculate the current value based on change orders and other adjustments // TODO: Implement logic to calculate current value based on change orders and other adjustments
        // For now, we will just set it to the original value
        CurrentValue = OriginalValue + ChangeOrderValue;
        
    }

    public void UpdateEndDate(int scheduleImpactDays)
    {
        // This method should update the current end date based on schedule impacts
        if (scheduleImpactDays > 0)
        {
            CurrentEndDate = OriginalEndDate.AddDays(scheduleImpactDays);
        }
        else
        {
            CurrentEndDate = OriginalEndDate;
        }
    }
}
