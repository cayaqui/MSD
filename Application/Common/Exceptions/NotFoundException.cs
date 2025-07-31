namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found
/// </summary>
public class NotFoundException : ApplicationException
{
    public string EntityName { get; }
    public object Key { get; }

    public NotFoundException()
        : base("The requested resource was not found.")
    {
        EntityName = string.Empty;
        Key = string.Empty;
    }

    public NotFoundException(string message)
        : base(message)
    {
        EntityName = string.Empty;
        Key = string.Empty;
    }

    public NotFoundException(string entityName, object key)
        : base($"Entity \"{entityName}\" ({key}) was not found.")
    {
        EntityName = entityName;
        Key = key;
    }

    public NotFoundException(string entityName, object key, string message)
        : base(message)
    {
        EntityName = entityName;
        Key = key;
    }
}
