namespace Web.Configuration
{
    /// <summary>
    /// Configuración de características habilitadas
    /// </summary>
    public class FeatureSettings
    {
        public bool EnableKPIs { get; set; } = true;
        public bool EnableAdvancedReports { get; set; } = true;
        public bool EnableDocumentManagement { get; set; } = true;
        public bool EnableEVM { get; set; } = true;
        public bool EnableGanttChart { get; set; } = true;
    }
}