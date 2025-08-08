using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Services;

public class CustomHttpMessageHandler : DelegatingHandler
{
    private readonly ILogger<CustomHttpMessageHandler> _logger;

    public CustomHttpMessageHandler(ILogger<CustomHttpMessageHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Sending {request.Method} request to {request.RequestUri}");
        
        // Log headers
        foreach (var header in request.Headers)
        {
            _logger.LogDebug($"Header: {header.Key} = {string.Join(", ", header.Value)}");
        }

        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            _logger.LogDebug($"Received response: {response.StatusCode}");
            return response;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timed out");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request failed");
            throw;
        }
    }
}