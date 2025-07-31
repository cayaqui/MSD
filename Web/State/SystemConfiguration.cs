namespace Web.State
{
    public class SystemConfiguration
    {
        public string AppName { get; set; } = "EzPro SD";
        public string AppTitle { get; set; } = "Sistema de Control de Proyectos";
        public string Version { get; set; } = "1.0.0";
        public string DefaultLanguage { get; set; } = "es-CL";
        public string DefaultCurrency { get; set; } = "USD";
        public ThemeSettings Theme { get; set; } = new();
        public FeatureFlags Features { get; set; } = new();
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }
}
