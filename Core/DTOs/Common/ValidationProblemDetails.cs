namespace Core.DTOs.Common;

/// <summary>
/// Validation Problem Details for validation errors
/// </summary>
public class ValidationProblemDetails : ProblemDetails
{
    /// <summary>
    /// Gets the validation errors associated with this instance
    /// </summary>
    public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();

    public ValidationProblemDetails()
    {
        Title = "One or more validation errors occurred.";
        Status = 400;
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
    }

    public ValidationProblemDetails(IDictionary<string, string[]> errors) : this()
    {
        Errors = errors ?? new Dictionary<string, string[]>();
    }
}