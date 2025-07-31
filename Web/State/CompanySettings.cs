namespace Web.State
{
    public class CompanySettings
    {
        public string DateFormat { get; set; } = "dd/MM/yyyy";
        public string TimeFormat { get; set; } = "HH:mm";
        public string NumberFormat { get; set; } = "#,##0.00";
        public int DefaultProjectDuration { get; set; } = 90;
        public bool EnableWBS { get; set; } = true;
        public bool EnableEVM { get; set; } = true;
        public bool EnableDocumentManagement { get; set; } = true;
    }
}
