


namespace Domain.Entities.Setup;

public class Company : BaseEntity, ISoftDelete, ICodeEntity, INamedEntity, IDescribable
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Business Information
    public string TaxId { get; private set; } = string.Empty;
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Country { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Website { get; private set; }

    // Logo
    public byte[]? Logo { get;  set; }
    public string? LogoContentType { get;  set; }

    // Configuration
    public string DefaultCurrency { get; private set; } = "USD";
    public string? FiscalYearStart { get; private set; } // MM-DD format

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation properties
    public ICollection<Operation> Operations { get; private set; } = new List<Operation>();

    private Company() { } // EF Core

    public Company(string code, string name, string taxId)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        TaxId = taxId ?? throw new ArgumentNullException(nameof(taxId));
    }



    public void UpdateBusinessInfo(string name, string? description, string taxId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        TaxId = taxId ?? throw new ArgumentNullException(nameof(taxId));
    }

    public void UpdateAddress(
        string? address,
        string? city,
        string? state,
        string? country,
        string? postalCode
    )
    {
        Address = address;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
    }

    public void UpdateContactInfo(string? phone, string? email, string? website)
    {
        Phone = phone;
        Email = email;
        Website = website;
    }

    public void UpdateLogo(byte[] logo, string contentType)
    {
        Logo = logo ?? throw new ArgumentNullException(nameof(logo));
        LogoContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
    }
    public void SetCurrency(string currency)
    {
        DefaultCurrency = currency ?? throw new ArgumentNullException(nameof(currency));
    }
    public void UpdateConfiguration(string defaultCurrency, string? fiscalYearStart)
    {
        DefaultCurrency =
            defaultCurrency ?? throw new ArgumentNullException(nameof(defaultCurrency));
        FiscalYearStart = fiscalYearStart;
    }
}
