namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown when user does not have permission to access a resource
/// </summary>
public class ForbiddenAccessException : ApplicationException
{
    public string? Resource { get; }
    public string? Action { get; }

    public ForbiddenAccessException()
        : base("Access forbidden. You do not have permission to access this resource.")
    {
    }

    public ForbiddenAccessException(string message)
        : base(message)
    {
    }

    public ForbiddenAccessException(string resource, string action)
        : base($"Access forbidden. You do not have permission to {action} {resource}.")
    {
        Resource = resource;
        Action = action;
    }

    public ForbiddenAccessException(string message, string resource, string action)
        : base(message)
    {
        Resource = resource;
        Action = action;
    }
}
