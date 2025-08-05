using Core.Enums.Contracts;

namespace Core.DTOs.Contracts.Contracts;

public class CreateContractDto
{
    public string ContractNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public ContractType Type { get; set; }
    public ContractCategory Category { get; set; }
    public string SubCategory { get; set; } = string.Empty;
    
    public Guid ProjectId { get; set; }
    public Guid ContractorId { get; set; }
    public string ContractorReference { get; set; } = string.Empty;
    
    public decimal OriginalValue { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal ExchangeRate { get; set; } = 1.0m;
    
    public DateTime ContractDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public PaymentTerms PaymentTerms { get; set; }
    public int PaymentDays { get; set; }
    public string PaymentSchedule { get; set; } = string.Empty;
    public decimal RetentionPercentage { get; set; }
    
    public bool RequiresPerformanceBond { get; set; }
    public decimal? PerformanceBondAmount { get; set; }
    public DateTime? PerformanceBondExpiry { get; set; }
    public bool RequiresPaymentBond { get; set; }
    public decimal? PaymentBondAmount { get; set; }
    public DateTime? PaymentBondExpiry { get; set; }
    public string InsuranceRequirements { get; set; } = string.Empty;
    
    public string Scope { get; set; } = string.Empty;
    public string Exclusions { get; set; } = string.Empty;
    public string SpecialConditions { get; set; } = string.Empty;
    public string PenaltyClauses { get; set; } = string.Empty;
    public string TerminationClauses { get; set; } = string.Empty;
    
    public string Notes { get; set; } = string.Empty;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
