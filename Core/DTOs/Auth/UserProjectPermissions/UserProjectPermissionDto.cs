namespace Core.DTOs.Auth.UserProjectPermissions
{

    public class GrantPermissionDto
    {
        public Guid PermissionId { get; set; }
    }

    public class RevokePermissionDto
    {
        public Guid PermissionId { get; set; }
    }

    public class GrantProjectPermissionDto
    {
        public string Role { get; set; } = string.Empty;
    }

    public class RevokeProjectPermissionDto
    {
        public string Role { get; set; } = string.Empty;
    }

    public class UserProjectPermissionDto
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserDisplayName { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime GrantedAt { get; set; }
        public string GrantedBy { get; set; } = string.Empty;
    }

    public class BulkPermissionOperationDto
    {
        public List<UserPermissionAssignment> UserPermissions { get; set; } = new();
    }

    public class UserPermissionAssignment
    {
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
    }
}