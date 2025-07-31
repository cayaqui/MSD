namespace Web.State
{
    public class FeatureFlags
    {
        public bool EnableWBS { get; set; } = true;
        public bool EnableEVM { get; set; } = true;
        public bool EnableDocumentManagement { get; set; } = true;
        public bool EnableRiskManagement { get; set; } = true;
        public bool EnableQualityManagement { get; set; } = true;
        public bool EnableContractManagement { get; set; } = true;
        public bool EnableAdvancedReporting { get; set; } = true;
        public bool EnableOfflineMode { get; set; } = false;
        public bool EnableNotifications { get; set; } = true;
        public bool EnableAIAssistant { get; set; } = false;
    }
}
