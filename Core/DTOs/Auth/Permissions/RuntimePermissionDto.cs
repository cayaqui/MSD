namespace Core.DTOs.Auth.Permissions
{
    /// <summary>
    /// DTO for runtime permission checking in authentication context
    /// </summary>
    public class RuntimePermissionDto
    {
        public string Key { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
        public bool IsInherited { get; set; }

        public RuntimePermissionDto() { }

        public RuntimePermissionDto(string key, bool isGranted, bool isInherited = false)
        {
            Key = key;
            IsGranted = isGranted;
            IsInherited = isInherited;
        }
    }
}