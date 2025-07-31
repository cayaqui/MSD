namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown when there's a conflict with the current state
/// </summary>
public class ConflictException : ApplicationException
{
    public string? Code { get; }
    public object? ConflictingEntity { get; }

    public ConflictException()
        : base("A conflict occurred with the current state of the resource.")
    {
    }

    public ConflictException(string message)
        : base(message)
    {
    }

    public ConflictException(string message, string code)
        : base(message)
    {
        Code = code;
    }

    public ConflictException(string message, object conflictingEntity)
        : base(message)
    {
        ConflictingEntity = conflictingEntity;
    }
}
