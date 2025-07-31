namespace Web.Models.Navigation
{
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
        public bool IsDisabled { get; set; }
        public int Order { get; set; }
        public List<NavigationItem>? Children { get; set; }
    }
}
