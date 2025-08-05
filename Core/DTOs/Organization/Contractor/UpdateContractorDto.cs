using Core.Enums.Projects;

namespace Core.DTOs.Organization.Contractor;

/// <summary>
/// DTO for updating contractor information
/// </summary>
public class UpdateContractorDto
{
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
    public string? PrequalificationNotes { get; set; }
    public ContractorStatus Status { get; set; }
    
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
}