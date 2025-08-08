using Domain.Common;
using Domain.Entities.Cost.Commitments;
using Domain.Entities.Cost.Core;
using Core.Enums.Cost;
using Core.Enums.Projects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities.Organization.Core;

/// <summary>
/// Contractor/Vendor entity
/// Represents external companies that provide services or materials
/// </summary>
public class Contractor : BaseEntity, IAuditable, ISoftDelete, ICodeEntity, INamedEntity, IActivatable
{
    // Basic Information
    public string Code { get; set; } = string.Empty; // Contractor code
    public string Name { get; set; } = string.Empty; // Legal name
    public string? TradeName { get; set; } // DBA/Commercial name
    public string TaxId { get; set; } = string.Empty; // RFC, RUT, etc.
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
    public decimal? PerformanceRating { get; set; } // 0-5 scale

    // Insurance & Compliance
    public bool HasInsurance { get; set; }
    public DateTime? InsuranceExpiryDate { get; set; }
    public decimal? InsuranceAmount { get; set; }
    public string? InsuranceCompany { get; set; }
    public string? InsurancePolicyNumber { get; set; }

    // Certifications
    public string? Certifications { get; set; } // JSON array of certifications
    public string? SpecialtyAreas { get; set; } // JSON array of specialties

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Activatable
    public bool IsActive { get; set; }

    // Navigation Properties
    public ICollection<Commitment> Commitments { get; set; } = new List<Commitment>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    // Constructor for EF Core
    private Contractor() { }

    public Contractor(string code, string name, string taxId, ContractorType type)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        TaxId = taxId ?? throw new ArgumentNullException(nameof(taxId));
        Type = type;
        Classification = ContractorClassification.Standard;
        Status = ContractorStatus.Active;
        IsActive = true;
        DefaultCurrency = "USD";
    }

    // Domain Methods
    public void UpdateContactInfo(string? contactName, string? email, string? phone)
    {
        ContactName = contactName;
        Email = email;
        Phone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAddress(string? address, string? city, string? state, string? country, string? postalCode)
    {
        Address = address;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFinancialInfo(string? bankName, string? bankAccount, string? paymentTerms, decimal creditLimit)
    {
        BankName = bankName;
        BankAccount = bankAccount;
        PaymentTerms = paymentTerms;
        CreditLimit = creditLimit;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Prequalify(string notes, string approvedBy)
    {
        IsPrequalified = true;
        PrequalificationDate = DateTime.UtcNow;
        PrequalificationNotes = notes;
        UpdatedBy = approvedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateInsurance(DateTime expiryDate, decimal amount, string company, string policyNumber)
    {
        HasInsurance = true;
        InsuranceExpiryDate = expiryDate;
        InsuranceAmount = amount;
        InsuranceCompany = company;
        InsurancePolicyNumber = policyNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ContractorStatus status, string updatedBy)
    {
        if (Status == ContractorStatus.Blacklisted && status != ContractorStatus.Blacklisted)
            throw new InvalidOperationException("Blacklisted contractors must go through a review process to change status");

        Status = status;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;

        if (status == ContractorStatus.Inactive || status == ContractorStatus.Blacklisted)
        {
            IsActive = false;
        }
    }

    public void UpdatePerformanceRating(decimal rating, string updatedBy)
    {
        if (rating < 0 || rating > 5)
            throw new ArgumentException("Performance rating must be between 0 and 5");

        PerformanceRating = rating;
        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;

        // Auto-update classification based on rating
        if (rating >= 4.5m)
            Classification = ContractorClassification.Preferred;
        else if (rating < 2.0m)
            Classification = ContractorClassification.Restricted;
        else
            Classification = ContractorClassification.Standard;
    }

    public bool IsInsuranceExpired()
    {
        return HasInsurance && InsuranceExpiryDate.HasValue && InsuranceExpiryDate.Value < DateTime.UtcNow;
    }

    public bool CanBeAwarded()
    {
        return IsActive &&
               IsPrequalified &&
               Status == ContractorStatus.Active &&
               !IsInsuranceExpired();
    }

    public decimal GetTotalCommitments()
    {
        return Commitments.Where(c => !c.IsDeleted && c.Status != CommitmentStatus.Cancelled)
                         .Sum(c => c.TotalAmount);
    }

    public decimal GetOpenCommitments()
    {
        return Commitments.Where(c => !c.IsDeleted &&
                                     c.Status != CommitmentStatus.Cancelled &&
                                     c.Status != CommitmentStatus.Closed)
                         .Sum(c => c.TotalAmount - c.InvoicedAmount);
    }
}