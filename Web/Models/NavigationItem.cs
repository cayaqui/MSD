namespace Web.Models
{
    /// <summary>
    /// Menú de navegación completo
    /// </summary>
    public class NavigationMenu
    {
        public NavigationItem? HomeItem { get; set; }
        public List<NavigationSection> Sections { get; set; } = new();
    }

    /// <summary>
    /// Sección del menú de navegación
    /// </summary>
    public class NavigationSection
    {
        public string Title { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<NavigationItem> Items { get; set; } = new();
    }

    /// <summary>
    /// Item del menú de navegación
    /// </summary>
    public class NavigationItem
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Href { get; set; }
        public NavigationItemType Type { get; set; } = NavigationItemType.Link;
        public string? RequiredPolicy { get; set; }
        public string? RequiredRole { get; set; }
        public NavigationBadge? Badge { get; set; }
        public bool IsExternal { get; set; }
        public bool IsDisabled{ get; set; }
        public int Order { get; set; }
        public List<NavigationItem>? Children { get; set; }
    }

    /// <summary>
    /// Tipo de item de navegación
    /// </summary>
    public enum NavigationItemType
    {
        Link,
        Section,
        Divider,
        Header
    }

    /// <summary>
    /// Badge para mostrar en items de navegación
    /// </summary>
    public class NavigationBadge
    {
        public string Text { get; set; } = string.Empty;
        public string? Variant { get; set; }
    }

    /// <summary>
    /// Item de breadcrumb
    /// </summary>
    public class BreadcrumbItem
    {
        public string Title { get; set; } = string.Empty;
        public string? Href { get; set; }
        public string? Icon { get; set; }
        public bool IsActive { get; set; }
    }
}