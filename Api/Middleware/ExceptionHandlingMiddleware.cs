using Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            TraceId = context.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case ValidationException validationException:
                response.Status = (int)HttpStatusCode.BadRequest;
                response.Title = "Validation error";
                response.Errors = validationException.Errors;
                break;

            case NotFoundException notFoundException:
                response.Status = (int)HttpStatusCode.NotFound;
                response.Title = "Resource not found";
                response.Detail = notFoundException.Message;
                break;

            case UnauthorizedException unauthorizedException:
                response.Status = (int)HttpStatusCode.Unauthorized;
                response.Title = "Unauthorized";
                response.Detail = unauthorizedException.Message;
                break;

            case ForbiddenAccessException forbiddenException:
                response.Status = (int)HttpStatusCode.Forbidden;
                response.Title = "Access forbidden";
                response.Detail = forbiddenException.Message;
                break;

            case BadRequestException badRequestException:
                response.Status = (int)HttpStatusCode.BadRequest;
                response.Title = "Bad request";
                response.Detail = badRequestException.Message;
                break;

            case ConflictException conflictException:
                response.Status = (int)HttpStatusCode.Conflict;
                response.Title = "Conflict";
                response.Detail = conflictException.Message;
                break;

            default:
                response.Status = (int)HttpStatusCode.InternalServerError;
                response.Title = "An error occurred";
                response.Detail = _environment.IsDevelopment()
                    ? exception.ToString()
                    : "An internal server error occurred";
                break;
        }

        context.Response.StatusCode = response.Status;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

public class ErrorResponse
{
    public int Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Detail { get; set; }
    public string TraceId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }
}