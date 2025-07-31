namespace Web.State
{
    public class ThemeSettings
    {
        public string ColorScheme { get; set; } = "light";
        public string PrimaryColor { get; set; } = "#3b82f6";
        public string SecondaryColor { get; set; } = "#64748b";
        public bool CompactMode { get; set; }
        public bool ShowAnimations { get; set; } = true;
        public string FontFamily { get; set; } = "Inter, sans-serif";
    }
}
