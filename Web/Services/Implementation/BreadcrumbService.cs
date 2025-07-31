
using Web.Models;
using Web.Models.Navigation;
using Web.Services.Interfaces;

namespace Web.Services.Implementation;

/// <summary>
/// Service implementation for breadcrumb navigation
/// </summary>
public class BreadcrumbService : IBreadcrumbService
{
    private readonly List<BreadcrumbItem> _items = new();

    public event Action? OnChange;
    public List<BreadcrumbItem> Items => _items.ToList();

    public void SetBreadcrumbs(params BreadcrumbItem[] items)
    {
        _items.Clear();
        _items.AddRange(items);
        OnChange?.Invoke();
    }

    public void SetBreadcrumbs(List<BreadcrumbItem> items)
    {
        _items.Clear();
        _items.AddRange(items);
        OnChange?.Invoke();
    }

    public void AddBreadcrumb(BreadcrumbItem item)
    {
        _items.Add(item);
        OnChange?.Invoke();
    }

    public void Clear()
    {
        _items.Clear();
        OnChange?.Invoke();
    }

    public void SetFromRoute(string route)
    {
        _items.Clear();
        _items.Add(BreadcrumbItem.Home);

        if (string.IsNullOrWhiteSpace(route) || route == "/")
        {
            OnChange?.Invoke();
            return;
        }

        var segments = route.Trim('/').Split('/');
        var currentPath = "";

        foreach (var segment in segments)
        {
            currentPath += $"/{segment}";
            var breadcrumb = GetBreadcrumbFromSegment(segment, currentPath);

            if (breadcrumb != null)
            {
                _items.Add(breadcrumb);
            }
        }

        // Mark last item as active
        if (_items.Count > 0)
        {
            _items[^1].IsActive = true;
        }

        OnChange?.Invoke();
    }

    private BreadcrumbItem? GetBreadcrumbFromSegment(string segment, string path)
    {
        return segment.ToLower() switch
        {
            "projects" => new BreadcrumbItem("Proyectos", path, "fa-project-diagram"),
            "setup" => new BreadcrumbItem("Configuración", path, "fa-cog"),
            "companies" => new BreadcrumbItem("Empresas", path, "fa-building"),
            "operations" => new BreadcrumbItem("Operaciones", path, "fa-network-wired"),
            "disciplines" => new BreadcrumbItem("Disciplinas", path, "fa-shapes"),
            "contractors" => new BreadcrumbItem("Contratistas", path, "fa-hard-hat"),
            "users" => new BreadcrumbItem("Usuarios", path, "fa-users"),
            "roles" => new BreadcrumbItem("Roles", path, "fa-user-shield"),
            "reports" => new BreadcrumbItem("Reportes", path, "fa-chart-line"),
            "cost" => new BreadcrumbItem("Costos", path, "fa-dollar-sign"),
            "progress" => new BreadcrumbItem("Progreso", path, "fa-tasks"),
            "new" => new BreadcrumbItem("Nuevo", null, "fa-plus"),
            "edit" => new BreadcrumbItem("Editar", null, "fa-edit"),
            "details" => new BreadcrumbItem("Detalles", null, "fa-info-circle"),
            _ => null
        };
    }
}