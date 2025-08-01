namespace Web.Models.Navigation
{
    /// <summary>
    /// Badge para mostrar en items de navegación
    /// </summary>
    public class NavigationBadge
    {
        public string Text { get; set; } = string.Empty;
        public string? Variant { get; set; }
        public int? Count { get; set; }
        public string? Color { get; set; }
    }
}
