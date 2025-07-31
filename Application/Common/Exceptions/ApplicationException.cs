namespace Application.Common.Exceptions;

/// <summary>
/// Base exception class for application exceptions
/// </summary>
public abstract class ApplicationException : Exception
{
    protected ApplicationException(string message) : base(message)
    {
    }

    protected ApplicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}