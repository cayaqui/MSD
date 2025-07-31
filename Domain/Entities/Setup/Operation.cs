

using Domain.Entities.Projects;

namespace Domain.Entities.Setup;

public class Operation : BaseEntity, ISoftDelete, ICodeEntity, INamedEntity, IDescribable
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Location
    public string? Location { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Country { get; private set; }

    // Management
    public string? ManagerName { get; private set; }
    public string? ManagerEmail { get; private set; }
    public string? CostCenter { get; private set; }

    // Foreign Keys
    public Guid CompanyId { get; private set; }

    // Navigation properties
    public Company Company { get; private set; } = null!;
    public ICollection<Project> Projects { get; private set; } = new List<Project>();

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    private Operation() { } // EF Core

    public Operation(Guid companyId, string code, string name)
    {
        CompanyId = companyId;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void SetLocation(
        string? location,
        string? address,
        string? city,
        string? state,
        string country
    )
    {
        Location = location;
        Address = address;
        City = city;
        State = state;
        Country = country;
    }

    public void SetCostCenter(string? costCenter)
    {
        CostCenter = costCenter;
    }

    public void UpdateBasicInfo(string name, string? description, string? location)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Location = location;
    }

    public void UpdateAddress(string? address, string? city, string? state, string? country)
    {
        Address = address;
        City = city;
        State = state;
        Country = country;
    }

    public void UpdateManager(string? managerName, string? managerEmail)
    {
        ManagerName = managerName;
        ManagerEmail = managerEmail;
    }

    public void UpdateCostCenter(string? costCenter)
    {
        CostCenter = costCenter;
    }
}
