namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown when authentication fails
/// </summary>
public class UnauthorizedException : ApplicationException
{
    public string? AuthenticationScheme { get; }

    public UnauthorizedException()
        : base("Authentication failed. Please login to access this resource.")
    {
    }

    public UnauthorizedException(string message)
        : base(message)
    {
    }

    public UnauthorizedException(string message, string authenticationScheme)
        : base(message)
    {
        AuthenticationScheme = authenticationScheme;
    }
}