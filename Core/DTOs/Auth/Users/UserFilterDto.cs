namespace Core.DTOs.Auth.Users;

public class UserFilterDto
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public bool? IncludeDeleted { get; set; }
    public string? CompanyId { get; set; }
    public string? Role { get; set; }
    public Guid? ProjectId { get; set; }
    public DateTime? LastLoginFrom { get; set; }
    public DateTime? LastLoginTo { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
