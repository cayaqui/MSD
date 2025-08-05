using Domain.Common;

namespace Domain.Entities.Security;

/// <summary>
/// Represents a role in the system
/// </summary>
public class Role : BaseEntity
{
    private readonly List<Permission> _permissions = new();
    private readonly List<User> _users = new();
    private readonly List<Role> _childRoles = new();

    /// <summary>
    /// Role code (unique identifier)
    /// </summary>
    public string Code { get; private set; }

    /// <summary>
    /// Role name
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Role description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Role type (e.g., System, Project, Custom)
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    /// Indicates if the role is active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Indicates if this is a system role (cannot be deleted)
    /// </summary>
    public bool IsSystem { get; private set; }

    /// <summary>
    /// Parent role ID for hierarchical roles
    /// </summary>
    public Guid? ParentRoleId { get; private set; }

    /// <summary>
    /// Parent role navigation property
    /// </summary>
    public virtual Role? ParentRole { get; private set; }

    /// <summary>
    /// Role level in hierarchy (0 = root)
    /// </summary>
    public int Level { get; private set; }

    /// <summary>
    /// Hierarchy path for efficient queries
    /// </summary>
    public string HierarchyPath { get; private set; }

    /// <summary>
    /// Permissions assigned to this role
    /// </summary>
    public virtual IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    /// <summary>
    /// Users assigned to this role
    /// </summary>
    public virtual IReadOnlyCollection<User> Users => _users.AsReadOnly();

    /// <summary>
    /// Child roles in hierarchy
    /// </summary>
    public virtual IReadOnlyCollection<Role> ChildRoles => _childRoles.AsReadOnly();

    /// <summary>
    /// Constructor for EF Core
    /// </summary>
    protected Role() { }

    /// <summary>
    /// Creates a new role
    /// </summary>
    public Role(string code, string name, string type, bool isSystem = false)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type ?? throw new ArgumentNullException(nameof(type));
        IsSystem = isSystem;
        IsActive = true;
        Level = 0;
        HierarchyPath = code;
    }

    /// <summary>
    /// Updates basic role information
    /// </summary>
    public void UpdateBasicInfo(string name, string? description, string type)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }

    /// <summary>
    /// Sets the parent role
    /// </summary>
    public void SetParent(Role? parent)
    {
        if (parent != null)
        {
            ParentRoleId = parent.Id;
            ParentRole = parent;
            Level = parent.Level + 1;
            HierarchyPath = $"{parent.HierarchyPath}/{Code}";
        }
        else
        {
            ParentRoleId = null;
            ParentRole = null;
            Level = 0;
            HierarchyPath = Code;
        }
    }

    /// <summary>
    /// Assigns a permission to the role
    /// </summary>
    public void AssignPermission(Permission permission)
    {
        if (!_permissions.Any(p => p.Id == permission.Id))
        {
            _permissions.Add(permission);
        }
    }

    /// <summary>
    /// Removes a permission from the role
    /// </summary>
    public void RemovePermission(Permission permission)
    {
        _permissions.RemoveAll(p => p.Id == permission.Id);
    }

    /// <summary>
    /// Clears all permissions
    /// </summary>
    public void ClearPermissions()
    {
        _permissions.Clear();
    }

    /// <summary>
    /// Activates the role
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the role
    /// </summary>
    public void Deactivate()
    {
        if (IsSystem)
            throw new InvalidOperationException("System roles cannot be deactivated");
        
        IsActive = false;
    }

    /// <summary>
    /// Checks if this role is a descendant of another role
    /// </summary>
    public bool IsDescendantOf(Role potentialAncestor)
    {
        if (potentialAncestor == null) return false;
        return HierarchyPath.StartsWith(potentialAncestor.HierarchyPath + "/");
    }

    /// <summary>
    /// Gets all permissions including inherited ones
    /// </summary>
    public IEnumerable<Permission> GetAllPermissions()
    {
        var permissions = new List<Permission>(_permissions);
        
        if (ParentRole != null)
        {
            permissions.AddRange(ParentRole.GetAllPermissions());
        }
        
        return permissions.Distinct();
    }
}