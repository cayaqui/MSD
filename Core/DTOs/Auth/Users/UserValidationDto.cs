namespace Core.DTOs.Auth.Users
{
    public class UserValidationDto
    {
        public bool IsValid { get; set; }
        public string? Reason { get; set; }
        public bool RequiresRegistration { get; set; }
        public Guid? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? UserDisplayName { get; set; }
    }
}