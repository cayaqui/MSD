using Domain.Entities.Projects;
using System.Diagnostics.Contracts;

namespace Domain.Entities.Setup;

/// <summary>
/// Enhanced Contractor entity with complete vendor management capabilities
/// </summary>
public class Contractor : BaseEntity, ISoftDelete, ICodeEntity, INamedEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    // Business Information
    public string? TaxId { get; private set; }
    public string? DunsNumber { get; private set; } // Dun & Bradstreet Number
    public ContractorType Type { get; private set; } // Vendor, Subcontractor, Supplier, Consultant
    public string? BusinessCategory { get; private set; } // Small Business, MBE, WBE, etc.

    // Address Information
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Country { get; private set; }
    public string? PostalCode { get; private set; }

    // Contact Information
    public string? PrimaryContactName { get; private set; }
    public string? PrimaryContactEmail { get; private set; }
    public string? PrimaryContactPhone { get; private set; }
    public string? SecondaryContactName { get; private set; }
    public string? SecondaryContactEmail { get; private set; }
    public string? SecondaryContactPhone { get; private set; }

    // Financial Information
    public string? BankName { get; private set; }
    public string? BankAccountNumber { get; private set; }
    public string? BankRoutingNumber { get; private set; }
    public string? PaymentTerms { get; private set; } // Net 30, 2/10 Net 30, etc.
    public string PreferredCurrency { get; private set; } = "USD";
    public decimal? CreditLimit { get; private set; }

    // Qualification & Compliance
    public bool IsPrequalified { get; private set; }
    public DateTime? PrequalificationDate { get; private set; }
    public DateTime? PrequalificationExpiry { get; private set; }
    public string? InsuranceCertificate { get; private set; }
    public DateTime? InsuranceExpiry { get; private set; }
    public decimal? GeneralLiabilityLimit { get; private set; }
    public decimal? WorkersCompLimit { get; private set; }
    public bool HasValidW9 { get; private set; } // For US contractors

    // Performance Metrics
    public decimal? OverallRating { get; private set; } // 1-5 scale
    public decimal? QualityRating { get; private set; }
    public decimal? DeliveryRating { get; private set; }
    public decimal? SafetyRating { get; private set; }
    public decimal? CostRating { get; private set; }
    public int CompletedProjects { get; private set; }
    public int ActiveProjects { get; private set; }
    public decimal TotalContractValue { get; private set; }

    // Service Categories
    public string? ServiceCategories { get; private set; } // JSON array of categories
    public string? Certifications { get; private set; } // JSON array of certifications
    public string? SpecialCapabilities { get; private set; }

    // Status & Risk
    public ContractorStatus Status { get; private set; } // Active, Suspended, Blacklisted
    public string? StatusReason { get; private set; }
    public DateTime? StatusDate { get; private set; }
    public RiskLevel RiskLevel { get; private set; } // Low, Medium, High
    public string? Notes { get; private set; }

    // Navigation properties
    public ICollection<Package> Packages { get; private set; } = new List<Package>();
    public ICollection<Invoice> Invoices { get; private set; } = new List<Invoice>();
    public ICollection<ContractorEvaluation> Evaluations { get; private set; } = new List<ContractorEvaluation>();

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    private Contractor() { } // EF Core

    public Contractor(string code, string name, ContractorType type)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type;
        Status = ContractorStatus.Active;
        RiskLevel = RiskLevel.Low;
        CompletedProjects = 0;
        ActiveProjects = 0;
        TotalContractValue = 0;
    }

    // Methods
    public void UpdateBusinessInfo(string name, string? taxId, string? dunsNumber)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        TaxId = taxId;
        DunsNumber = dunsNumber;
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

    public void UpdatePrimaryContact(string? name, string? email, string? phone)
    {
        PrimaryContactName = name;
        PrimaryContactEmail = email;
        PrimaryContactPhone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSecondaryContact(string? name, string? email, string? phone)
    {
        SecondaryContactName = name;
        SecondaryContactEmail = email;
        SecondaryContactPhone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFinancialInfo(string? bankName, string? paymentTerms, decimal? creditLimit)
    {
        BankName = bankName;
        PaymentTerms = paymentTerms;
        CreditLimit = creditLimit;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBankingInfo(string? accountNumber, string? routingNumber)
    {
        BankAccountNumber = accountNumber;
        BankRoutingNumber = routingNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPrequalification(DateTime qualificationDate, DateTime expiryDate)
    {
        IsPrequalified = true;
        PrequalificationDate = qualificationDate;
        PrequalificationExpiry = expiryDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateInsurance(string certificateNumber, DateTime expiryDate, decimal generalLiability, decimal workersComp)
    {
        InsuranceCertificate = certificateNumber;
        InsuranceExpiry = expiryDate;
        GeneralLiabilityLimit = generalLiability;
        WorkersCompLimit = workersComp;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRatings(decimal? quality, decimal? delivery, decimal? safety, decimal? cost)
    {
        ValidateRating(quality);
        ValidateRating(delivery);
        ValidateRating(safety);
        ValidateRating(cost);

        QualityRating = quality;
        DeliveryRating = delivery;
        SafetyRating = safety;
        CostRating = cost;

        // Calculate overall rating
        var ratings = new[] { quality, delivery, safety, cost }.Where(r => r.HasValue).ToList();
        if (ratings.Any())
        {
            OverallRating = ratings.Average(r => r!.Value);
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ContractorStatus status, string? reason)
    {
        Status = status;
        StatusReason = reason;
        StatusDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        // Update risk level based on status
        if (status == ContractorStatus.Blacklisted)
        {
            RiskLevel = RiskLevel.High;
        }
        else if (status == ContractorStatus.Suspended)
        {
            RiskLevel = RiskLevel.Medium;
        }
    }

    public void UpdateRiskLevel(RiskLevel level, string? notes)
    {
        RiskLevel = level;
        if (!string.IsNullOrEmpty(notes))
        {
            Notes = $"{DateTime.UtcNow:yyyy-MM-dd}: {notes}\n{Notes}";
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementProjectCount(bool isCompleted)
    {
        if (isCompleted)
        {
            CompletedProjects++;
            ActiveProjects = Math.Max(0, ActiveProjects - 1);
        }
        else
        {
            ActiveProjects++;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateContractValue(decimal additionalValue)
    {
        TotalContractValue += additionalValue;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsInsuranceExpired() => InsuranceExpiry.HasValue && InsuranceExpiry.Value < DateTime.UtcNow;

    public bool IsPrequalificationExpired() => PrequalificationExpiry.HasValue && PrequalificationExpiry.Value < DateTime.UtcNow;

    public bool CanBidOnProjects() => Status == ContractorStatus.Active &&
                                      IsPrequalified &&
                                      !IsPrequalificationExpired() &&
                                      !IsInsuranceExpired();

    private void ValidateRating(decimal? rating)
    {
        if (rating.HasValue && (rating.Value < 1 || rating.Value > 5))
            throw new ArgumentException("Rating must be between 1 and 5");
    }
}

// Supporting Enums
public enum ContractorType
{
    Vendor = 1,
    Subcontractor = 2,
    Supplier = 3,
    Consultant = 4,
    ServiceProvider = 5
}

public enum ContractorStatus
{
    Active = 1,
    Inactive = 2,
    Suspended = 3,
    Blacklisted = 4,
    Pending = 5
}

public enum RiskLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

// Contractor Evaluation Entity
public class ContractorEvaluation : BaseEntity
{
    public Guid ContractorId { get; private set; }
    public Guid ProjectId { get; private set; }
    public DateTime EvaluationDate { get; private set; }
    public string EvaluatedBy { get; private set; } = string.Empty;

    // Ratings
    public decimal QualityScore { get; private set; }
    public decimal DeliveryScore { get; private set; }
    public decimal SafetyScore { get; private set; }
    public decimal CostScore { get; private set; }
    public decimal CommunicationScore { get; private set; }
    public decimal OverallScore { get; private set; }

    // Comments
    public string? Strengths { get; private set; }
    public string? AreasForImprovement { get; private set; }
    public string? GeneralComments { get; private set; }
    public bool WouldRehire { get; private set; }

    // Navigation
    public Contractor Contractor { get; private set; } = null!;
    public Project Project { get; private set; } = null!;

    private ContractorEvaluation() { }

    public ContractorEvaluation(
        Guid contractorId,
        Guid projectId,
        string evaluatedBy,
        decimal quality,
        decimal delivery,
        decimal safety,
        decimal cost,
        decimal communication)
    {
        ContractorId = contractorId;
        ProjectId = projectId;
        EvaluatedBy = evaluatedBy;
        EvaluationDate = DateTime.UtcNow;

        QualityScore = quality;
        DeliveryScore = delivery;
        SafetyScore = safety;
        CostScore = cost;
        CommunicationScore = communication;

        OverallScore = new[] { quality, delivery, safety, cost, communication }.Average();
        WouldRehire = OverallScore >= 3.5m;
    }
}