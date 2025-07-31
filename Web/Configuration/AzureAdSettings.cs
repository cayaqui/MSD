namespace Web.Configuration
{

    /// <summary>
    /// Configuración de Azure AD
    /// </summary>
    public class AzureAdSettings
    {
        public string Authority { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public bool ValidateAuthority { get; set; } = true;
        public string RedirectUri { get; set; } = string.Empty;
        public string PostLogoutRedirectUri { get; set; } = string.Empty;
        public string ResponseType { get; set; } = "code";
    }
}