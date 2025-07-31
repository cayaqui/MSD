namespace Core.DTOs.Auth;

public class BulkUserOperationDto
{
    public List<Guid> UserIds { get; set; } = new();
}