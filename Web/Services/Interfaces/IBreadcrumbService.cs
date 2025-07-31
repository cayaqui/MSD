
using Web.Models.UIUX;

namespace Web.Services.Interfaces;

public interface IBreadcrumbService
{
    event Action? OnChange;
    List<BreadcrumbItem> Items { get; }
    void SetBreadcrumbs(params BreadcrumbItem[] items);
    void SetBreadcrumbs(List<BreadcrumbItem> items);
    void AddBreadcrumb(BreadcrumbItem item);
    void Clear();
    void SetFromRoute(string route);
}