namespace Core.DTOs.Auth.Users
{
    public class GraphUserDto
    {
        public string Id { get; set; } = string.Empty; // Entra ID / Object ID
        public string UserPrincipalName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? GivenName { get; set; }
        public string? Surname { get; set; }
        public string? Mail { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
        public string? OfficeLocation { get; set; }
        public string? MobilePhone { get; set; }
        public string? BusinessPhones { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? CompanyName { get; set; }
        public string? EmployeeId { get; set; }
        public bool AccountEnabled { get; set; }
        public string? ProfilePhoto { get; set; } // Base64 encoded photo
    }
}