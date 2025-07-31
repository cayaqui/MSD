namespace Web.Models.Navigation
{
    /// <summary>
    /// Menú de navegación completo
    /// </summary>
    public class NavigationMenu
    {
        public NavigationItem? HomeItem { get; set; }
        public List<NavigationSection> Sections { get; set; } = new();
    }
}
