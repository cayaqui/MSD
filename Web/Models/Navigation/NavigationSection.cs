namespace Web.Models.Navigation
{
    /// <summary>
    /// Sección del menú de navegación
    /// </summary>
    public class NavigationSection
    {
        public string Title { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<NavigationItem> Items { get; set; } = new();
    }
}