using Core.Enums.Projects;

namespace Core.DTOs.Organization.Contractor;

/// <summary>
/// Contractor data transfer object
/// </summary>
public class ContractorDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? TradeName { get; set; }
    public string TaxId { get; set; } = string.Empty;
    public ContractorType Type { get; set; }
    public ContractorClassification Classification { get; set; }
    
    // Contact Information
    public string? ContactName { get; set; }
    public string? ContactTitle { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? MobilePhone { get; set; }
    public string? Website { get; set; }
    
    // Address
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    
    // Financial Information
    public string? BankName { get; set; }
    public string? BankAccount { get; set; }
    public string? BankRoutingNumber { get; set; }
    public string? PaymentTerms { get; set; }
    public decimal CreditLimit { get; set; }
    public string DefaultCurrency { get; set; } = "USD";
    
    // Qualification
    public bool IsPrequalified { get; set; }
    public DateTime? PrequalificationDate { get; set; }
    public string? PrequalificationNotes { get; set; }
    public ContractorStatus Status { get; set; }
    public decimal? PerformanceRating { get; set; }
    
    // Insurance & Compliance
    public bool HasInsurance { get; set; }
    public DateTime? InsuranceExpiryDate { get; set; }
    public decimal? InsuranceAmount { get; set; }
    public string? InsuranceCompany { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    
    // Certifications
    public string? Certifications { get; set; }
    public string? SpecialtyAreas { get; set; }
    
    // Status
    public bool IsActive { get; set; }
    
    // Computed Properties
    public bool IsInsuranceExpired { get; set; }
    public bool CanBeAwarded { get; set; }
    public decimal TotalCommitments { get; set; }
    public decimal OpenCommitments { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}