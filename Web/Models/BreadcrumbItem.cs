namespace Web.Models.UIUX;

/// <summary>
/// Represents a breadcrumb navigation item
/// </summary>
public class BreadcrumbItem
{
    public string Title { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }

    public BreadcrumbItem() { }

    public BreadcrumbItem(string title, string? url = null, string? icon = null, bool isActive = false)
    {
        Title = title;
        Url = url;
        Icon = icon;
        IsActive = isActive;
    }

    public static BreadcrumbItem Home => new("Inicio", "/", "fa-home");
    public static BreadcrumbItem Projects => new("Proyectos", "/projects", "fa-project-diagram");
    public static BreadcrumbItem Setup => new("Configuración", "/setup", "fa-cog");
    public static BreadcrumbItem Reports => new("Reportes", "/reports", "fa-chart-line");
}
