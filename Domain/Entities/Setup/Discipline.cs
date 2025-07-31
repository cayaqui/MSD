


using Domain.Entities.Projects;

namespace Domain.Entities.Setup;

public class Discipline
    : BaseEntity,
        ISoftDelete,
        ICodeEntity,
        INamedEntity,
        IDescribable,
        IOrderable
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Order { get; set; }

    // Visual
    public string ColorHex { get; private set; } = "#000000";
    public string? Icon { get; private set; }

    // Category
    public bool IsEngineering { get; private set; }
    public bool IsManagement { get; private set; }

    // Navigation properties
    public ICollection<WorkPackageDiscipline> WorkPackageDisciplines { get; private set; } =
        new List<WorkPackageDiscipline>();
    public ICollection<BudgetItem> BudgetItems { get; private set; } =
        new List<BudgetItem>();

    // Soft Delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
   
    private Discipline() { } // EF Core

    public Discipline(string code, string name, string colorHex, int order, bool isEngineering = true)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Order = order;
        ColorHex = Color.IsValidHexColor(colorHex)
            ? colorHex
            : throw new ArgumentException("Invalid color format");
        IsEngineering = isEngineering;
        IsManagement = !isEngineering;
    }

    public void SetIcon(string? icon)
    {
        Icon = icon;
    }

    public void SetColor(string colorHex)
    {
        ColorHex = colorHex;
    }

    public void SetCategory(bool isEngineering, bool isManagement)
    {
        IsEngineering = isEngineering;
        IsManagement = isManagement;
    }

    public void UpdateBasicInfo(string name, string? description, int order)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Order = order;
    }

    public void UpdateVisual(string colorHex, string? icon)
    {
        ColorHex = Color.IsValidHexColor(colorHex)
            ? colorHex
            : throw new ArgumentException("Invalid color format");
        Icon = icon;
    }

    public void UpdateCategory(bool isEngineering, bool isManagement)
    {
        IsEngineering = isEngineering;
        IsManagement = isManagement;
    }

    private static class Color
    {
        public static bool IsValidHexColor(string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(value, @"^#[0-9A-Fa-f]{6}$");
        }
    }
}
