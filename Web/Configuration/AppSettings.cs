namespace Web.Configuration
{
    /// <summary>
    /// Configuración general de la aplicación
    /// </summary>
    public class AppSettings
    {
        public string ApplicationName { get; set; } = "EzPro";
        public string Version { get; set; } = "1.0.0";
        public string CompanyName { get; set; } = string.Empty;
        public string SupportEmail { get; set; } = string.Empty;
        public long MaxFileUploadSize { get; set; } = 10485760; // 10MB
        public string[] AllowedFileExtensions { get; set; } = Array.Empty<string>();
        public int DefaultPageSize { get; set; } = 20;
        public int SessionTimeoutMinutes { get; set; } = 30;
        public bool EnableOfflineMode { get; set; } = false;
        public FeatureSettings Features { get; set; } = new();
    }
}