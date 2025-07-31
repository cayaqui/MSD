namespace Web.Configuration
{
    /// <summary>
    /// Configuración de la API
    /// </summary>
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = "https://localhost:7001/";
        public int Timeout { get; set; } = 30;
        public int RetryCount { get; set; } = 3;
        public int RetryDelay { get; set; } = 1000;
    }
}